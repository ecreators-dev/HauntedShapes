using Assets.Script.BaseCoroutinesYields;
using Assets.Script.Model;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TMPro;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		public partial class FlashlightBehaviour : HuntingInteractionMonoBehaviour, IFlashlight, IEquipment, IInteractible
		{
				public event Action<IEquipment> LetFallEvent;
				public event Action<IEquipment> UselessEvent;

				[SerializeField] private Transform toggleActive;
				[SerializeField] private bool startActive = false;
				[SerializeField] [Min(10)] private float secondsOfLife = (float)TimeSpan.FromMinutes(3).TotalSeconds;
				[SerializeField] [Min(10)] private float uselessCoolDownSeconds = (float)TimeSpan.FromSeconds(30).TotalSeconds;
				[SerializeField] [Min(1)] private int uselessBreakDown = 10;
				[SerializeField] private bool canToggleOn = true;
				[SerializeField] private HeatCellBehaviour[] heatBodies;
				[SerializeField] private Animator huntingAnimationController;

				[Header("Sounds")]
				[SerializeField] private AudioSource soundPlayer;
				[SerializeField] private AudioClip toggleOnSound;
				[SerializeField] private AudioClip toggleOffSound;
				[SerializeField] private AudioClip breakDownSound;
				[SerializeField] private AudioClip powerUpSound;
				[SerializeField] private AudioClip equipSound;
				[SerializeField] private AudioClip crashSound;
				private readonly Dictionary<string, FieldInfo> soundFieldsCache = new Dictionary<string, FieldInfo>();

				[Header("UI 3D")]
				[SerializeField]
				private TMP_Text textInWorld;

				private bool activePowerStatus;
				private Rigidbody body;
				private int lifeRestore = -1;
				private CoroutineCommand coroutineMoveToHand;
				private float startTime;
				private HuntingAnimationProxy huntingAnimator;
				/// <summary>
				/// The player wanted to turn off the light during a hunt
				/// </summary>
				private bool pendingTurnOff;
				private bool toggledByPlayer;

				/// <summary>
				/// Time in seconds (see Update) before the rotation finds a new random angle target.
				/// <br/>See also: <b><seealso cref="randomLocalEuler"/></b>
				/// </summary>
				private float turnTimeoutSec;

				/// <summary>
				/// Simplified Eulers to turn to after <b><see cref="turnTimeoutSec"/></b> reached 0 seconds
				/// </summary>
				private Vector2 randomLocalEuler;
				private PlayerBehaviour textInWorldTarget;
				private float touchTimestamp;

				public string Name => "Taschenlampe";
				/// <summary>
				/// Next pending power status. This does not represent <see cref="IsPowered"/>. This represents only a wish to power on or off.
				/// </summary>
				public bool NextPowerStatus { get; private set; }
				public float Price => 0;
				public float SellPrice => 0;
				public PlayerBehaviour OwnedByPlayer { get; private set; }
				public PlayerBehaviour InUseOfPlayer { get; private set; }
				public bool IsUseless { get; private set; }

				/// <summary>
				/// Active status, after solving <see cref="NextPowerStatus"/>
				/// </summary>
				public bool IsPowered => activePowerStatus;

				public bool CanToggleOnOff => canToggleOn;

				private bool IsTextInWorldActive => textInWorld is { } && textInWorld.enabled;

				public string GameObjectName => this.GetGameObjectName();

				public string ImplementationTypeName => this.GetImplementationTypeName();

				private void OnGUI()
				{
#if UNITY_EDITOR
						EditorGUILayout.HelpBox($"{secondsOfLife:0.1} sec to drop", MessageType.Info);
#endif
				}

				protected override void Start()
				{
						body.isKinematic = true;
						body.useGravity = true;
						body.mass = 0.45f; // 450g bzw. 0.45 kg

						base.Start();

						if (startActive)
						{
								// fix: force turn on
								if (IsPowered)
										activePowerStatus = false;

								Debug.Log($"{gameObject.name}  {nameof(startActive)} = true");
								TryActivate();
						}

						foreach (HeatCellBehaviour cell in heatBodies)
						{
								cell.CooledDownEvent -= OnCellCooled;
								cell.CooledDownEvent += OnCellCooled;

								cell.HeatedUpEvent -= OnCellHeated;
								cell.HeatedUpEvent += OnCellHeated;
						}

						RegisterHuntingAnimator();
				}

				[ContextMenu("Hunting/Animator Register (Lazy)")]
				public void RegisterHuntingAnimator()
				{
						if (huntingAnimationController is { })
						{
								huntingAnimator ??= new HuntingAnimationProxy(huntingAnimationController, "Hunting");
						}
				}

				private void LateUpdate()
				{
						// fixit!!!! - welches Script ‰nderte die Werte permanent?
						if (transform.parent is { })
						{
								transform.localPosition = Vector3.zero;
								transform.localEulerAngles = new Vector3(90, 0, 0);
						}
				}

				private void Update()
				{
						Update_TextInWorld_LookAtPlayer();

						RotateRandomLocally();

						FirePowerChange();

						TryCoolDown();

						if (IsUseless is false)
						{
								float lifetime = Time.timeSinceLevelLoad - startTime;
								if (lifetime > secondsOfLife)
								{
										BreakDown();
								}
								else
								{
										// Taschenlampen-Baterie wird immer w‰rmer - bis zu heiﬂ
										lifetime += Time.deltaTime;
								}
						}

						OnEndOfFrameUpdate();
				}

				private void Update_TextInWorld_LookAtPlayer()
				{
						// show "Aufheben"(pickup) if not used by player and not fallen (on ground)
						if (IsFallenEnd && InUseOfPlayer is null && Time.time - touchTimestamp < 5)
						{
								var player = this.FindComponentAroundRadiusOf<PlayerBehaviour>(radius: 2);
								ShowTextInWorld(player, "Aufheben");
						}
						else
						{
								// hide: not FallenEnd or In Use or Timeout touched
								textInWorld.enabled = false;
						}

						if (IsTextInWorldActive)
						{
								PlayerBehaviour player = textInWorldTarget;
								if (player is { })
								{
										Quaternion canvasRotation = textInWorld.transform.parent.transform.rotation;
										// stay always rolled as world rotation
										textInWorld.transform.rotation = Quaternion.Inverse(canvasRotation);
										textInWorld.transform.LookAt(player.transform, player.transform.up);
								}
						}
				}

				private void RotateRandomLocally()
				{
						// FIXME not working correctly!!


						// time in seconds to turn towards new local euler direction
						float xSpeed = 1f, ySpeed = 1f;

						if (turnTimeoutSec <= 0)
						{
								Debug.Log("Flashlight local rotation randomly update");
								turnTimeoutSec = 0.8f;
								float xRange = 1.5f, yRange = 1.5f;
								randomLocalEuler = new Vector2
								{
										x = UnityEngine.Random.Range(-xRange, xRange),
										y = UnityEngine.Random.Range(-yRange, yRange)
								};
						}
						else if (turnTimeoutSec > 0)
						{
								turnTimeoutSec -= Time.deltaTime;
						}

						transform.localEulerAngles =
								new Vector3
								{
										// x = tilt, localEulerAngles.x, randomLocalEuler.y (vertical rotation)
										x = Mathf.LerpAngle(transform.localEulerAngles.x, randomLocalEuler.y, Time.deltaTime * ySpeed),

										// y = pan, localEulerAngles.y, randomLocalEuler.x (horizontal rotation)
										y = Mathf.LerpAngle(transform.localEulerAngles.y, randomLocalEuler.x, Time.deltaTime * xSpeed),

										// roll
										z = 0
								};
				}

				private void OnEndOfFrameUpdate()
				{
						// reset at end of frame
						toggledByPlayer = false;
				}

				private void OnCellHeated(IHeatUp cell)
				{
						if (heatBodies.All(c => c.FullyHeated))
						{
								BreakDown();
						}
				}

				private void OnCellCooled(IHeatUp cell)
				{

				}

				private void BreakDown()
				{
						if (IsUseless)
								return;

						PlayBreakDownSound();
						TryDeactivate();
						Drop();
						IsUseless = true;
						UselessEvent?.Invoke(this);
				}

				private void TryCoolDown()
				{
						// cooldown: only if power is off
						if (IsUseless && lifeRestore < uselessBreakDown && IsPowered is false)
						{
								StartCoroutine(WaitCoolDown());

								IEnumerator WaitCoolDown()
								{
										yield return new WaitForSeconds(uselessCoolDownSeconds);
										RestoreLife();
										yield break;
								}
						}
				}

				private void RestoreLife()
				{
						IsUseless = false;
						lifeRestore++;

						if (lifeRestore >= this.uselessBreakDown)
						{
								// dead forever
								Debug.LogWarning($"{gameObject.name} canToggleOn now false. Cannot be activated again, instead must be powered up. {nameof(lifeRestore)} > {nameof(uselessBreakDown)} (max is {uselessBreakDown})");
								canToggleOn = false;
						}
				}

				[ContextMenu("Anschalten")]
				public void TryPowerOn()
				{
						NextPowerStatus = true;
				}

				[ContextMenu("Ausschalten")]
				public void TryPowerOff()
				{
						NextPowerStatus = false;
				}

				private void Awake()
				{
						coroutineMoveToHand = new MoveLocalPositionToLocalPosition(transform, Vector3.zero, 10,
									 OnAnimationEquipUpdate,
									 OnAnimationEquipped);

						body = GetComponent<Rigidbody>();
						NoFall();
				}

				[ContextMenu("Jetzt Schalten - Editor")]
				public void FirePowerChange()
				{
						// fix: try deactivate at start because NextPowerStatus is false
						// fix: less changes (performance)
						// only if a hunt is running (toggle on) or if player pressed button, then allowed to change
						bool canChange = Hunting.InHunt || toggledByPlayer;

						if (canChange is false)
								return;

						if (Hunting.InHunt is false && pendingTurnOff && IsPowered)
						{
								Debug.Log($"Turn off {gameObject.name} by pending, after hunt");
								TryPowerOff();
						}
						else if (Hunting.InHunt && IsPowered is false)
						{
								Debug.Log($"Turn on {gameObject.name} by hunt is running");
								TryPowerOn();
						}

						// wish to toggle status = NextPowerStatus
						if (NextPowerStatus)
						{
								TryActivate();
						}
						else
						{
								TryDeactivate();
						}
				}

				private bool TryActivate()
				{
						Debug.Log($"Try activate {gameObject.name}, {nameof(IsPowered)}={IsPowered}");
						return IsPowered is false && ActivateNow();

						bool ActivateNow()
						{
								// can activate if hunt is active: canToggleOn must ignored
								if (Hunting.InHunt is false && canToggleOn is false)
								{
										Debug.Log($"Unable to Turn on for {gameObject.name} (false)");
										return false;
								}

								StartLight();

								// during hunt, the life drain is not effected
								if (Hunting.InHunt)
								{
										// during a hunt the player wanted to activate the flashlight again
										if (toggledByPlayer)
										{
												pendingTurnOff = false;
										}
								}
								else
								{
										StartLightLifeDrain();
								}

								Debug.Log($"Valid to Turn on for {gameObject.name} (true)");
								return true;
						}
				}

				private void StartLight()
				{
						Debug.Log("Flashlight ON");
						activePowerStatus = true;
						toggleActive.gameObject.SetActive(true);
						PlayToggleOnSound();
				}

				private void StartLightLifeDrain()
				{
						startTime = Time.timeSinceLevelLoad;

						foreach (var cell in heatBodies)
								cell.HeatUp();

						RestoreLife();
				}

				private bool TryDeactivate()
				{
						Debug.Log($"Try deactivate {gameObject.name}, {nameof(IsPowered)}={IsPowered}");
						return IsPowered is true && InactiveNow();

						bool InactiveNow()
						{
								// the player is not allowed to deactivate the flashlight, b/c it should be active during a hunt
								if (Hunting.InHunt)
								{
										pendingTurnOff = true;
										Debug.Log($"Pending Turn off for {gameObject.name} (false)");
										return false;
								}

								StopLight();
								StopLightLifeDrain();

								Debug.Log($"Valid to Turn off for {gameObject.name} (true)");
								return true;
						}
				}

				private void StopLightLifeDrain()
				{
						foreach (var cell in heatBodies)
								cell.CoolDown();
				}

				private void StopLight()
				{
						Debug.Log("Flashlight OFF");
						activePowerStatus = false;
						toggleActive.gameObject.SetActive(false);

						PlayToggleOffSound();
				}

				/// <summary>
				/// Flashlight equipped to player
				/// </summary>
				/// <param name="player"></param>
				public void OnEquip(PlayerBehaviour player)
				{
						StopFalling();

						// bring in hand from ground or elsewhere (animated move)
						StartCoroutine(coroutineMoveToHand.Run());

						PlayOnEquipSound();
				}

				private void OnAnimationEquipUpdate()
				{
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10);
				}

				private void OnAnimationEquipped()
				{
						transform.rotation = Quaternion.identity;
				}

				public void PowerUp(PlayerBehaviour player)
				{
						if (IsInUseOfPlayer(player))
						{
								if (IsUseless)
								{
										canToggleOn = true;
										PlayPowerUpSound();
								}
						}
				}

				private bool IsInUseOfPlayer(PlayerBehaviour player) => this.InUseOfPlayer?.GetInstanceID() == player.GetInstanceID();

				public void ToggleOn(PlayerBehaviour player)
				{
						toggledByPlayer = true;
						TryPowerOn();
				}

				public void ToggleOff(PlayerBehaviour player)
				{
						toggledByPlayer = true;
						TryPowerOff();
				}

				public void Sell(PlayerBehaviour player) => player.GiveMoney(SellPrice);

				public void Buy(PlayerBehaviour player) => player.TakeMoney(SellPrice);

				private bool IsFallenEnd => body.useGravity is false && Vector3.zero.Equals(body.velocity) && Vector3.zero.Equals(body.angularVelocity);

				public bool IsEquipped => transform.parent != null;

				public bool IsPickable => IsEquipped is false;

				private void OnCollisionEnter(Collision collision)
				{
						TryDeactivate();
						PlayCrashSound();
				}

				public void Drop()
				{
						body.isKinematic = false;

						AddPlayerMovement(body);

						body.AddForce(Camera.main.transform.forward.normalized * 3, ForceMode.Impulse);
						body.AddForce(Camera.main.transform.up.normalized * 2, ForceMode.Impulse);

						LetFallEvent?.Invoke(this);
				}

				private void AddPlayerMovement(Rigidbody body)
				{
						if (InUseOfPlayer)
						{
								Rigidbody playerBody = InUseOfPlayer.GetComponent<Rigidbody>();
								body.velocity = playerBody.velocity;
						}
				}

				private void StopFalling()
				{
						body.isKinematic = true;
						body.velocity = Vector3.zero;
						body.angularVelocity = Vector3.zero;
				}

				private void PlayCrashSound()
				{
						PlaySoundOnce(nameof(crashSound));
				}

				public void NoFall()
				{
						body.useGravity = false;
				}

				protected override void OnHuntStart()
				{
						base.OnHuntStart();
						huntingAnimator.StartHuntAnimation();
				}

				protected override void OnHuntStopped()
				{
						base.OnHuntStopped();
						huntingAnimator.StopHuntAnimation();
				}

				private void PlayBreakDownSound()
				{
						// technical sound - on let fall crash sound (hit ground)
						PlaySoundOnce(nameof(breakDownSound));
				}

				private void PlayPowerUpSound()
				{
						PlaySoundOnce(nameof(powerUpSound));
				}

				private void PlayOnEquipSound()
				{
						PlaySoundOnce(nameof(equipSound));
				}

				private void PlayToggleOnSound()
				{
						PlaySoundOnce(nameof(toggleOnSound));
				}

				private void PlayToggleOffSound()
				{
						PlaySoundOnce(nameof(toggleOffSound));
				}

				/// <summary>
				/// Plays an AudioClip, if set and reports any issues to console
				/// </summary>
				private void PlaySoundOnce(string fieldName)
				{
						if (soundPlayer is null)
						{
								Debug.LogWarning("Missing AudioSource property instance! Cannot play any sound clips.");
								return;
						}

						if (!soundFieldsCache.TryGetValue(fieldName, out FieldInfo field))
						{
								field = GetType().GetRuntimeFields().Where(f => f.Name.Equals(fieldName)).FirstOrDefault();
								soundFieldsCache[fieldName] = field;
						}

						AudioClip clip = field.GetValue(this) as AudioClip;
						if (clip)
						{
								Debug.Log($"AudioClip playing once: {fieldName}");
								soundPlayer.PlayOneShot(clip);
						}
						else
						{
								Debug.LogWarning($"Missing AudioClip: {fieldName}");
						}
				}

				public void ShowTextInWorld(PlayerBehaviour sender, string text)
				{
						if (textInWorld is { })
						{
								textInWorld.text = text;
								textInWorld.enabled = !string.IsNullOrEmpty(text);
								textInWorldTarget = IsTextInWorldActive is false ? null : sender;
						}
				}

				public void TouchClickUpdate()
				{
						touchTimestamp = Time.time;
				}

				public void TouchOverUpdate()
				{
						// pickup done in player script
				}

				public void OnPickup(PlayerBehaviour player)
				{
						// is an equipment! -> nothing
				}
		}
}