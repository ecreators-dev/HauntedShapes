using Assets.Script.Behaviour.GhostTypes;
using Assets.Script.Model;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

using Debug = UnityEngine.Debug;

namespace Assets.Script.Behaviour
{
		public class PlayerBehaviour : MonoBehaviour, IPlayerCam, IStepSoundProvider
		{
				public string playerFirstName;
				public string playerLastName;
				public int playerAge;
				public Gender gender;
				public DateTime birthDay;
				public DateTime deathDay;
				public string murderName;
				public string motherName;
				public string fatherName;
				public List<(string firstName, bool male)> sisters;
				public MoodType mood;
				public AgeType ageType;
				public bool reserectable = true;

				private GhostEntity ghostType;
				private bool isDead;

				[SerializeField]
				private Transform equipmentHolder;

				[SerializeField]
				private Camera playerCam;

				[SerializeField]
				private AudioSource playerAudioSource3d;

				[SerializeField]
				private AudioClip[] silentHurtClipsRandom;

				private float money;
				private IEquipment activeEquipment;
				private float toggleTimeout;
				private bool toggle;
				private AudioClip stepSoundClip;

				public Camera Cam => playerCam;

				public IEquipment ActiveEquipment
				{
						get => activeEquipment ??= FetchEquipment();
						private set => activeEquipment = value;
				}

				private void Start()
				{
						FetchEquipment();
						OnEquip_EquipmentStart();
				}

				/// <summary>
				/// Remark: Do not call <see cref="ActiveEquipment"/> because of recursion
				/// </summary>
				private IEquipment FetchEquipment()
				{
						foreach (Transform item in equipmentHolder.transform)
						{
								IEquipment old = activeEquipment;
								activeEquipment = item.GetComponent<IEquipment>();
								if (old is { })
								{
										old.UselessEvent -= OnUselessEquipment;
								}
								if (activeEquipment is { })
								{
										activeEquipment.UselessEvent += OnUselessEquipment;
								}
								break;
						}
						return activeEquipment;
				}

				private void Update()
				{
						HandleHuntToggleDebug();
						HandleClickObject();
						HandleEquipmentToggleButton();
				}

				private void HandleHuntToggleDebug()
				{
						if (this.InputControls().DebugHuntToggleOnOff)
						{
								HuntingStateBean hunt = HuntingStateBean.Instance;
								if (hunt.InHunt is false)
								{
										hunt.StartHunt();
								}
								else
								{
										hunt.StopHunt();
								}
						}
				}

				private void HandleEquipmentToggleButton()
				{
						IInputControls keyboardKeys = this.InputControls();
						IEquipment equipment = ActiveEquipment;
						if (equipment is null)
						{
								FetchEquipment();
						}

						// try to toggle on/off held equipment
						/*
						 Way To go: player is not able to toggle during timeout or if not released after last toggle
						            player can toggle if not hold key and if not in timeout (toggle time / key on-time)
						 Player press key -> timeout reached -> key was released -> toggle ON/OFF
						 Player hold key -> timeout is running or key was not released -> nothing
						 if Timeout > 0 Timeout ran out
						 Player release key -> key was released = true
						 */
						if (keyboardKeys.PlayerToggleEquipmentOnOff)
						{
								if (toggleTimeout <= 0 && toggle is false)
								{
										// 30 ms wait
										toggleTimeout = 30 / 1000f;

										if (equipment is { })
										{
												Debug.Log($"Triggered by Keyboard Hotkey: {nameof(keyboardKeys.PlayerToggleEquipmentOnOff)}");
												OnEquipmentToggleButton();
										}
										else
										{
												Debug.LogWarning("No item in hand!");
										}
										toggle = true;
								}
						}
						else
						{
								toggle = false;
						}

						if (toggleTimeout > 0)
						{
								toggleTimeout -= Time.deltaTime;
						}
				}

				public void PickUp(IInteractible item)
				{
						if (item is Component cmp)
						{
								// Handle: is already in hand of any player?
								if (item.IsPickable)
								{
										OnPickUp_HandleActivePickupItem(item);

										Transform parent = GetPickupTransfrom();
										// TODO - TEST: 2nd parameter: move to local position zero?
										cmp.transform.SetParent(parent, true);
										
										// event: disable gravity for example!
										item.OnPickup(this);

										Debug.Log($"Item pickup: {cmp.gameObject.name}");
								}
								else
								{
										Debug.LogWarning($"Cannot pickup item: {cmp.gameObject.name}. Already pickup!");
								}
						}
				}

				private void OnPickUp_HandleActivePickupItem(IInteractible item)
				{
						Transform parent = GetPickupTransfrom();
						Transform pickupActive = parent.childCount > 0 ? parent.GetChild(0) : default;
						if (pickupActive != null)
						{
								var active = pickupActive.GetComponent<IInteractible>();
								if (active != null && item != active)
								{
										// player can only carry one item at the same time!
										active.Drop();
								}
						}
				}

				[SerializeField] private LayerMask interactibleDoorMask;

				private void HandleClickObject()
				{
						const float maxDistance = 2.25f; // war 2.25

						InteractionEnum equipmentAction = ClickInteractible(maxDistance, out IEquipment equipment);
						HandleEquipmentInteraction(equipmentAction, equipment);

						// prio for equipment picking
						if (equipmentAction != InteractionEnum.CLICKED_ACTIVE)
						{
								InteractionEnum interactibleAction = ClickInteractible(maxDistance, out IInteractible worldInteractible);
								HandleInteractibleInteraction(interactibleAction, worldInteractible);
						}

						return;
						/*
						if (TryClickObject(out IInteractable worldInteractible, out bool hover, maxDistance, out var hit, out var farHit))
						{
								worldInteractible.TouchClickUpdate();
						}
						else if (hover)
						{
								worldInteractible.TouchOverUpdate();
						}

						if (TryClickObject(hit, farHit, out IEquipment equipment, out hover))
						{
								Equip(equipment);
						}
						else if (hover)
						{
								if (farHit is null)
								{
										equipment.ShowTextInWorld(this, "Zu weit");
								}
								else if (hit.HasValue)
								{
										equipment.ShowTextInWorld(this, "Aufheben");
								}
						}*/
				}

				private void HandleEquipmentInteraction(InteractionEnum result, IEquipment equipment)
				{
						switch (result)
						{
								case InteractionEnum.NONE:
										break;
								case InteractionEnum.HOVER_ACTIVE:
										equipment.ShowTextInWorld(this, "Aufheben");
										break;
								case InteractionEnum.CLICKED_ACTIVE:
										Equip(equipment);
										break;
								case InteractionEnum.CLICKED_TOO_FAR:
										equipment.ShowTextInWorld(this, "Zu weit");
										break;
								case InteractionEnum.HOVER_TOO_FAR:
										break;
								default:
										break;
						}
				}

				private void HandleInteractibleInteraction(InteractionEnum result, IInteractible worldInteractible)
				{
						switch (result)
						{
								case InteractionEnum.NONE:
										break;
								case InteractionEnum.HOVER_ACTIVE:
										worldInteractible.TouchOverUpdate();
										break;
								case InteractionEnum.CLICKED_ACTIVE:
										worldInteractible.TouchClickUpdate();
										Debug.Log($"Object clicked! {worldInteractible.GameObjectName}");
										break;
								case InteractionEnum.CLICKED_TOO_FAR:
										Debug.Log($"Object close, but still too far! {worldInteractible.GameObjectName}");
										break;
								case InteractionEnum.HOVER_TOO_FAR:
										break;
								default:
										throw new MissingSwitchCaseException(result);
						}
				}

				private InteractionEnum ClickInteractible<T>(float maxDistance, out T instance)
						where T : IMonoBehaviour
				{
						if (Mouse.current.leftButton.isPressed)
						{
								float nearByDistance = maxDistance + maxDistance * 0.5f;
								if (CrosshairShoot(transform.position, maxDistance, out var inRangeHit, out Vector3 target))
								{
										if (inRangeHit.collider is { } && inRangeHit.collider.GetComponent<T>() is Component you)
										{
												Debug.Log("Clicked Object: " + you.gameObject.name);
												instance = you is T t ? t : default;
												return InteractionEnum.CLICKED_ACTIVE;
										}
								}
								else if (CrosshairShoot(transform.position, nearByDistance, out inRangeHit, out target))
								{
										if (inRangeHit.collider is { } && inRangeHit.collider.GetComponent<T>() is Component you)
										{
												Debug.Log("Clicked Object: " + you.gameObject.name);
												instance = you is T t ? t : default;
												return InteractionEnum.CLICKED_ACTIVE;
										}
								}

						}
						else if (CrosshairShoot(transform.position, maxDistance, out var inRangeHit, out var target))
						{
								if (inRangeHit.collider is { } && inRangeHit.collider.GetComponent<T>() is Component you)
								{
										Debug.Log("Not clickable Object: " + you.gameObject.name);
										instance = you is T t ? t : default;
										return InteractionEnum.CLICKED_TOO_FAR;
								}
						}

						instance = default;
						return InteractionEnum.NONE;

						bool CrosshairShoot(Vector3 toolPosition, float dist, out RaycastHit hit, out Vector3 aimPoint)
						{
								// Unless you've mucked with the projection matrix, firing through
								// the center of the screen is the same as firing from the camera itself.
								var camera = Camera.main.transform;
								Ray crosshair = new Ray(camera.position, camera.forward);

								// Cast a ray forward from the camera to see what's 
								// under the crosshair from the player's point of view.
								if (Physics.Raycast(crosshair, out hit, dist))
								{
										aimPoint = hit.point;
								}
								else
								{
										aimPoint = crosshair.origin + crosshair.direction * dist;
								}

								// Now we know what to aim at, do a second ray cast from the tool.
								Ray beam = new Ray(toolPosition, aimPoint - toolPosition);

								// If we don't hit anything, just go straight to the aim point.
								if (!Physics.Raycast(beam, out hit, dist))
								{
										return false;
								}

								// Otherwise, stop at whatever we hit on the way.
								return false;
						}
				}

#if UNITY_EDITOR
				private void OnGUI()
				{
						EditorGUILayout.HelpBox("Toggle On/Off = T", MessageType.Info);
						EditorGUILayout.HelpBox("Hunt Fx On/Off = H", MessageType.Info);
						EditorGUILayout.HelpBox("Mouse On/Off = Backspace", MessageType.Info);
				}
#endif

				private void OnEquip_EquipmentStart()
				{
						if (activeEquipment is { })
						{
								// event: what does item to on equip? toggle off may be?
								activeEquipment.OnEquip(this);

								activeEquipment.LetFallEvent -= OnEquipmentFallOffHand;
								activeEquipment.LetFallEvent += OnEquipmentFallOffHand;

								activeEquipment.UselessEvent -= OnUselessEquipment;
								activeEquipment.UselessEvent += OnUselessEquipment;
						}
				}

				public void Equip(IEquipment item)
				{
						if (item is Component cmp)
						{
								if (item != activeEquipment)
								{
										Debug.Log($"Equip item: {cmp.gameObject.name}");
										item.NoFall();

										// make new tool active
										OnEquip_HandleActiveEquipment();

										// accept new tool and carry with player
										activeEquipment = item;
										cmp.transform.SetParent(equipmentHolder);

										// start on equip
										OnEquip_EquipmentStart();
								}
								else
								{
										Debug.LogError($"Cannot equip item: {cmp.gameObject.name}");
								}
						}
				}

				private void OnEquip_HandleActiveEquipment()
				{
						// TODO drop or hide in inventory
						// TODO animation

						bool forTestOnly = true;
						if (forTestOnly)
						{
								DropActiveEquipment();
						}
				}

				[ContextMenu("Drop Equipped Item")]
				public void DropActiveEquipment()
				{
						if (ActiveEquipment is { })
						{
								ActiveEquipment.Drop();
						}
				}

				private void OnUselessEquipment(IEquipment obj)
				{
						if (ReferenceEquals(obj, ActiveEquipment))
						{
								if (silentHurtClipsRandom.Length > 0)
								{
										AudioClip clip = silentHurtClipsRandom[UnityEngine.Random.Range(0, silentHurtClipsRandom.Length)];
										if (clip is { } && clip.length > 0)
										{
												PlayOnceFromPosition(clip);
										}
								}
						}
				}

				private void PlayOnceFromPosition(AudioClip clip)
				{
						if (playerAudioSource3d is { })
						{
								playerAudioSource3d.spatialBlend = 1;
								playerAudioSource3d.PlayOneShot(clip);
								Debug.LogWarning("Play Audio Clip");
						}
						else
						{
								Debug.LogWarning("Missing AudioSource");
						}
				}

				private void OnEquipmentFallOffHand(IEquipment obj)
				{
						if (obj == ActiveEquipment)
						{
								var behaviour = (MonoBehaviour)ActiveEquipment;
								behaviour.transform.SetParent(null); // do not move with player anymore

								ActiveEquipment.LetFallEvent -= OnEquipmentFallOffHand;
								ActiveEquipment.UselessEvent -= OnUselessEquipment;
								ActiveEquipment = null;
						}
				}

				[ContextMenu("Toggle On | Off")]
				public void OnEquipmentToggleButton()
				{
						IEquipment equipment = ActiveEquipment;
						if (equipment is null)
								return;

						if (equipment.IsPowered)
								equipment.ToggleOff(this);
						else
								equipment.ToggleOn(this);
				}

				public void Die()
				{
						if (isDead is false && ghostType is null)
						{
								isDead = true;
								ghostType = gameObject.AddComponent<GhostEntity>();
								ghostType.CopyPersonality(this);
						}
				}

				public void Reserect()
				{
						if (isDead && reserectable)
						{
								isDead = false;
								Destroy(ghostType);

								// das geht nur einmal bei einem Ritual
								reserectable = false;
						}
				}

				public void GiveMoney(float money)
				{
						this.money += Mathf.Abs(money);
				}

				public bool TakeMoney(float price)
				{
						price = Mathf.Abs(price);
						if (price <= this.money)
						{
								this.money -= price;
								return true;
						}
						return false;
				}

				public Transform GetPickupTransfrom()
				{
						return this.equipmentHolder;
				}

				public void SetStepSound(AudioClip clip)
				{
						var oldName = GetSoundName(stepSoundClip);
						this.stepSoundClip = clip;
						Debug.Log($"Changed step sound for {gameObject.name} from {oldName} to {GetSoundName(stepSoundClip)}");
				}

				private string GetSoundName(AudioClip clip)
				{
						if (clip == null || clip.name is null)
						{
								return "<null>";
						}
						if (string.IsNullOrWhiteSpace(clip.name))
						{
								return "<no name>";
						}
						return $"'{clip.name}'";
				}
		}
}