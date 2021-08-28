using Assets.Script.Components;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		public class FlashlightBehaviour : Equipment
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;

				private bool canToggleActiveState = false;
				private bool pickedUp = false;
				private bool activeStateBeforeHunt;
				private bool activeState;
				private bool? currentState = null;
				private float activeSeconds;
				private float breakCoolDownSeconds;

				private Transform Transform { get; set; }
				private Rigidbody RigidBody { get; set; }

				private bool IsHeldByPlayer(out PlayerBehaviour p) => this.TryGetComponentAllParent(out p);

				private void Awake()
				{
						Transform = transform;
						RigidBody = GetComponent<Rigidbody>();
				}

				private void Start()
				{
						SeupShopInfo();
				}

				private void SeupShopInfo()
				{
						if (ShopInfo != null)
						{
								shopInfo = ShopInfo;
						}
						base.SetShopInfo(shopInfo, this);
				}

				protected override void Update()
				{
						if (IsHuntingActive || pickedUp == false)
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

												// play switch off animation as usal
												PlayAnimationOnOrOffState();

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

						// toggled or first time:
						if (currentState == null || currentState != activeState)
						{
								PlayAnimationOnOrOffState();

								// start only once! "currentState"
								currentState = activeState;
						}
				}

				private void PlayAnimationOnOrOffState()
				{
						if (activeState)
						{
								if (IsBroken)
								{
										// only toggle, but do no visual!
										ShopInfo.SwitchOnAnimation?.PlayAudio(Transform);
								}
								else
								{
										ShopInfo.SwitchOnAnimation?.Play(Transform, animator);
								}
						}
						else
						{
								if (IsBroken)
								{
										// only toggle, but do no visual!
										ShopInfo.SwitchOffAnimation?.PlayAudio(Transform);
								}
								else
								{
										ShopInfo.SwitchOffAnimation?.Play(Transform, animator);
								}
						}
				}

				public override bool CanInteract(PlayerBehaviour sender) => !base.IsLocked;

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsHeldByPlayer(out _))
						{
								if (canToggleActiveState)
								{
										ToggleOnOff();
								}
						}
						else if (sender.TryGetComponent(out PlayerBehaviour p))
						{
								PickUp(p);
						}
				}

				private void ToggleOnOff()
				{
						activeState = !activeState;
				}

				private void PickUp(PlayerBehaviour senderIgnored)
				{
						if (User == null)
						{

						}

						/// Remarks:
						// Only the player script animates if required, when pickup, because
						// only the player script knows the euipmentholder position and more ...

						pickedUp = true;
						canToggleActiveState = true;
						RigidBody.isKinematic = true;
				}

				protected override void OnEquip()
				{
						// show
						gameObject.SetActive(true);

						// status action and animation is solved by Update-Method!
						activeState = true;
				}

				protected override void OnInvectory()
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
		}
}