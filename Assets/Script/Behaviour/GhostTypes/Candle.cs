using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		[RequireComponent(typeof(Rigidbody))]
		public class Candle : Equipment, IRitualTribute, ILightSource
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private bool startEnlit;
				[SerializeField] private Animator animator;
				[Min(1)]
				[SerializeField] private float lightSourceMultiplier = 1.4f;
				[SerializeField] private string brokenText = "abgebrannt";

				public TributeTypeEnum TriggerType => TributeTypeEnum.CANDLE;

				public float ActiveMultiplier => lightSourceMultiplier;

				protected override void Start()
				{
						base.Start();

						SetShopInfo(shopInfo, this);

						RandomAnimation();

						if (startEnlit)
						{
								ToggleOn();
						}
						else
						{
								ToggleOff();
						}
				}

				protected override void Update()
				{
						base.Update();

						animator.SetBool("Hunting", IsHuntingActive);

						// light on/off
						animator.SetBool("PowerOn", IsPowered);
				}

				private void RandomAnimation()
				{
						var reversedAnimation = Random.Range(0, 100) % 2 == 0;
						var animationSpeedEffector = Random.Range(0.9f, 2.5f);
						animator.SetFloat("Effector", animationSpeedEffector);
						animator.SetBool("Reverse", reversedAnimation);
				}

				private void ToggleOff() => SetPowered(false);

				private void ToggleOn() => SetPowered(true);

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender) && CheckBelongsTo(sender);
				}

				protected override void Interact(PlayerBehaviour senderIgnored)
				{
						TogglePowered();

						// light changed in Update() sby animation
				}

				public override EquipmentInfo GetEquipmentInfo()
				{
						if (IsBroken)
						{
								return new EquipmentInfo
								{
										Text = brokenText
								};
						}
						else
						{
								return null;
						}
				}
		}
}