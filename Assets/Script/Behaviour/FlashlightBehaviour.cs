using Assets.Script.Behaviour.FirstPerson;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		[DisallowMultipleComponent]
		public class FlashlightBehaviour : EquipmentPlacable, ILightSource
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[Min(1)]
				[SerializeField] private float activeMultiplier = 2;
				[SerializeField] private string cooldownText = "Gerät kühlt ab";

				private bool canToggleActiveState = true;
				private float activeSeconds;
				private float breakCoolDownSeconds;

				private Transform Transform { get; set; }

				public bool IsActive => IsPowered;

				public float ActiveMultiplier => activeMultiplier;

				private void Awake()
				{
						Transform = transform;
				}

				protected override void Start()
				{
						base.Start();

						if (ShopInfo != null)
								shopInfo = ShopInfo;

						SetShopInfo(shopInfo, this);
				}

				protected override void Update()
				{
						base.Update();

						animator.SetBool("Hunting", IsHuntingActive);
						animator.SetBool("PowerOn", IsActive);

						if (IsTakenByPlayer is false)
						{
								UpdateNotTaken();
						}
						else if (IsBroken)
						{
								Cooldown();
								SetPowered(false);
						}
						else
						{
								UpdateNotBroken();
						}
				}

				private void UpdateNotTaken()
				{
						// if dropped active: heat up also
						if (IsBroken is false && IsActive)
						{
								Drain();
						}
						if (IsBroken && IsActive is false)
						{
								Cooldown();
						}
				}

				private void UpdateNotBroken()
				{
						// look at hittarget if active or in hunt
						if (IsActive || IsHuntingActive)
						{
								UpdateLookAtCrosshairTarget();
						}

						if (IsActive)
						{
								if (IsHuntingActive is false)
								{
										Drain();
								}
								else
								{
										Cooldown();
								}
						}
						else if (activeSeconds > 0)
						{
								// cool down also, if not active in hand
								activeSeconds -= Time.deltaTime;
						}
				}

				private void Drain()
				{
						// heat up with active time
						activeSeconds += Time.deltaTime;
						if (activeSeconds >= ShopInfo.ActiveSeconds)
						{
								OnBreak();
						}
				}

				private void OnBreak()
				{
						// after usal playback, set broken
						// if setbroken is called before play animation -> no animation would show up, but only sound!
						// the light would stay active
						breakCoolDownSeconds = ShopInfo.CoolDownSeconds;

						SetBroken();

						SetPowered(false);
				}

				private void Cooldown()
				{
						breakCoolDownSeconds -= Time.deltaTime;

						// cool down over:
						if (breakCoolDownSeconds <= 0)
						{
								OnCooldownCompleted();
						}
				}

				private void OnCooldownCompleted()
				{
						// no animation: player must interact by hisself
						SetRepaired();

						// reset
						breakCoolDownSeconds = 0;
						activeSeconds = 0;

						// player must activate manually!
				}

				private void UpdateLookAtCrosshairTarget()
				{
						(bool actualHit, Vector3 point, Vector3 _) = CrosshairHit.RaycastCollidersOnly(CameraMoveType.Instance.GetCamera());
						if (actualHit)
						{
								Vector3 targetDir = point - Transform.position;
								Transform.rotation = Quaternion
										.Slerp(Transform.rotation, Quaternion.LookRotation(targetDir, Transform.up),
										 Time.deltaTime * 10);
						}
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						// on ground or in hand
						return IsLocked is false && (User == null || User == sender);
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsBroken is false && IsHuntingActive is false)
						{
								if (IsTakenByPlayer)
								{
										TogglePowered();
								}
								else
								{
										sender.DropThenEquip(this);
								}
						}
				}

				public override EquipmentInfo GetEquipmentInfo()
				{
						if (IsBroken)
						{
								return new EquipmentInfo
								{
										Text = breakCoolDownSeconds <= 0 ? null : cooldownText,
										TimerText = $"{breakCoolDownSeconds:0.0} s"
								};
						}
						else
						{
								return new EquipmentInfo
								{
										Text = null,
										TimerText = $"{activeSeconds:0} s"
								};
						}
				}
		}
}