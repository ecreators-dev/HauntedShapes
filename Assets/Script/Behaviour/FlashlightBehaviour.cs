using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(Rigidbody))]
		[DisallowMultipleComponent]
		public class FlashlightBehaviour : EquipmentPlacable, ILightSource, IPowerbankSupport
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[Min(1)]
				[SerializeField] private float activeMultiplier = 2;
				[SerializeField] private string cooldownText = "Gerät kühlt ab";
				[Header("Powerbank")]
				[Min(0)]
				[SerializeField] private float power = 100;
				[Min(0)]
				[SerializeField] private int maxPower = 100;

				private float activeSeconds;
				private float breakCoolDownSeconds;
				// TODO not yet shown!
				private float powerVisual;

				public float ActiveMultiplier => activeMultiplier;

				public int MaxPower => maxPower;

				public int Power => Mathf.RoundToInt(power);

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
						animator.SetBool("PowerOn", IsPowered);

						if (IsTakenByPlayer)
						{
								// look at hittarget if active or in hunt
								UpdateLookAtCrosshairTarget();
						}

						if (IsTakenByPlayer is false)
						{
								UpdateNotTaken();
						}
						else if (IsBroken)
						{
								Cooldown();

								if (IsPowered)
								{
										SetPowered(false);
								}
						}
						else
						{
								UpdateNotBroken();
						}
				}

				private void UpdateNotTaken()
				{
						// if dropped active: heat up also
						if (IsBroken is false && IsPowered)
						{
								Drain();
						}
						if (IsBroken && IsPowered is false)
						{
								Cooldown();
						}
				}

				private void UpdateNotBroken()
				{
						if (IsPowered)
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
						if (IsTakenByPlayer is false) return;

						// any mask
						(HitInfo _, HitInfo hoverRange) = CrosshairHit.RaycastInfo;

						if (hoverRange.IsHit)
						{
								Vector3 targetDir = hoverRange.HitPoint - Transform.position;
								// it takes up to 5 seconds
								Transform.rotation = Quaternion
										.Slerp(Transform.rotation, Quaternion.LookRotation(targetDir, Transform.up),
										 Time.deltaTime * 5);
						}
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						// on ground or in hand
						return base.CanInteract(sender) && IsUserOrNotTaken(sender);
				}

				protected override void Interact(PlayerBehaviour sender)
				{
						if (IsHuntingActive is false && IsTakenByPlayer)
						{
								TogglePowered();
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

				public bool LoadPower(int power)
				{
						bool canLoad = this.power < this.maxPower;
						if (canLoad)
						{
								this.power += Mathf.Min(power, this.maxPower - this.power);
								this.power = Mathf.Min(this.power, this.maxPower);
						}
						return canLoad;
				}

				public void VisualizePowerLoadUpdate(float actualPowerDelta, int maxPowerDelta)
				{
						this.powerVisual = (this.Power + actualPowerDelta) / this.maxPower;
				}
		}
}