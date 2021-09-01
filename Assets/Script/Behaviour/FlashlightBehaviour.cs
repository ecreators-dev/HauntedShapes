using Assets.Script.Behaviour.FirstPerson;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		[DisallowMultipleComponent]
		public class FlashlightBehaviour : Equipment, ILightSource
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private bool activeState = true;
				private bool? activeStateOld = null;

				[SerializeField] private Animator animator;
				[Min(1)]
				[SerializeField] private float activeMultiplier = 2;

				private bool canToggleActiveState = true;
				private bool activeStateBeforeHunt;
				private float activeSeconds;
				private float breakCoolDownSeconds;

				private Transform Transform { get; set; }

				private Rigidbody RigidBody { get; set; }

				public bool IsActive => activeState;

				public float ActiveMultiplier => activeMultiplier;

				private void Awake()
				{
						Transform = transform;
						RigidBody = GetComponent<Rigidbody>();
				}

				private void Start()
				{
						if (ShopInfo != null)
								shopInfo = ShopInfo;

						SetShopInfo(shopInfo, this);
				}

				protected override void Update()
				{
						bool togggled = activeStateOld != IsActive;
						base.Update();
						animator.SetBool("Hunting", IsHuntingActive);
						if (IsBroken)
						{
								animator.SetBool("PowerOn", false);
								if (togggled) ShopInfo.SwitchOffAnimation?.ResetOnce();
								ShopInfo.SwitchOffAnimation?.PlayAudio(Transform);
						}
						else
						{
								if (togggled)
								{
										ShopInfo.SwitchOffAnimation?.ResetOnce();
										ShopInfo.SwitchOnAnimation?.ResetOnce();
								}

								animator.SetBool("PowerOn", IsActive);
								if (IsActive)
								{
										ShopInfo.SwitchOnAnimation?.PlayAudio(Transform);
								}
								else
								{
										ShopInfo.SwitchOffAnimation?.PlayAudio(Transform);
								}
						}

						// look at hittarget or zero
						if (IsTakenByPlayer)
						{
								UpdatePointToTarget();
						}

						if (IsHuntingActive || IsTakenByPlayer is false)
						{
								return;
						}

						if (activeState)
						{
								if (IsBroken is false)
								{
										// heat up
										activeSeconds += Time.deltaTime;

										// overheaten:
										if (activeSeconds > ShopInfo.ActiveSeconds)
										{
												// switch off
												activeState = false;

												// after usal playback, set broken
												// if setbroken is called before play animation -> no animation would show up, but only sound!
												// the light would stay active
												breakCoolDownSeconds = ShopInfo.CoolDownSeconds;
												SetBroken();
												// do not play below code!
												return;
										}
								}
								else
								{
										breakCoolDownSeconds -= Time.deltaTime;
										// cool down over:
										if (breakCoolDownSeconds <= 0)
										{
												// no animation: player must interact by hisself

												SetFixed();

												// reset
												breakCoolDownSeconds = 0;
												activeSeconds = 0;
										}
								}
						}

						activeStateOld = activeState;
				}

				private void UpdatePointToTarget()
				{
						ICrosshairUI instance = CrosshairHitVisual.Instance;
						(bool actualHit, Vector3 point) = instance.UpdateHitPointFarAway(CameraMoveType.Instance.GetCamera());
						if (actualHit)
						{
								Vector3 targetDir = point - Transform.position;
								Transform.rotation = Quaternion
										.Slerp(Transform.rotation, Quaternion.LookRotation(targetDir, Transform.up),
										 Time.deltaTime * 10);
						}
				}

				public override bool CanInteract(PlayerBehaviour sender) => !base.IsLocked;

				public override void Interact(PlayerBehaviour sender)
				{
						if (canToggleActiveState)
						{
								ToggleOnOff();
						}
				}

				private void ToggleOnOff()
				{
						activeState = !activeState;
				}

				protected override void OnEquip()
				{
						// show
						gameObject.SetActive(true);

						// status action and animation is solved by Update-Method!
						activeState = true;
				}

				protected override void OnInventory()
				{
						// hide
						gameObject.SetActive(false);
				}

				protected override void OnPickedUp()
				{
						Debug.Log($"{gameObject.name} pick up");
				}

				protected override void PerformDrop()
				{
						RigidBody.isKinematic = false;
						Debug.Log($"{gameObject.name} drop");
				}

				protected override void OnHuntStart()
				{
						if (isActiveAndEnabled)
						{
								activeStateBeforeHunt = activeState;
								activeState = true;
								ShopInfo.HuntAnimation?.Play(Transform, animator);
						}
				}

				protected override void OnHuntStop()
				{
						if (isActiveAndEnabled)
						{
								activeState = activeStateBeforeHunt;
								if (activeState)
								{
										ShopInfo.SwitchOnAnimation?.Play(Transform, animator);
								}
								else
								{
										ShopInfo.SwitchOffAnimation?.Play(Transform, animator);
								}
						}
				}

				protected override void OnOwnerOwnedEquipment()
				{
						// nothing
				}

				private void ToggleOn() => activeState = true;

				private void ToggleOff() => activeState = false;

				protected override void OnEditMode_ToggleOn() => ToggleOn();

				protected override void OnEditMode_ToggleOff() => ToggleOff();
		}
}