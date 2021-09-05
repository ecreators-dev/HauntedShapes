using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		[RequireComponent(typeof(Rigidbody))]
		public class Candle : Equipment, IRitualTribute, ILightSource
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[Min(1)]
				[SerializeField] private float lightSourceMultiplier = 1.4f;
				[SerializeField] private string brokenText = "abgebrannt";

				public TributeTypeEnum TriggerType => TributeTypeEnum.CANDLE;

				public float ActiveMultiplier => lightSourceMultiplier;

				public bool IsActive => IsPowered;

				protected override void Start()
				{
						base.Start();

						SetShopInfo(shopInfo, this);

						RandomAnimation();
				}

				protected override void Update()
				{
						base.Update();

						animator.SetBool("Hunting", IsHuntingActive);
						animator.SetBool("PowerOn", IsActive);
				}

				private void RandomAnimation()
				{
						var reversedAnimation = Random.Range(0, 100) % 2 == 0;
						var animationSpeedEffector = Random.Range(0.9f, 2.5f);
						animator.SetFloat("Effector", animationSpeedEffector);
						animator.SetBool("Reverse", reversedAnimation);
				}

				public void ToggleOff() => SetPowered(false);

				public void ToggleOn() => SetPowered(true);

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender) && (User == null || User == sender);
				}

				public override void Interact(PlayerBehaviour senderIgnored)
				{
						if (IsTakenByPlayer)
						{
								TogglePowered();
						}
				}

				protected override void OnEditMode_ToggleOn() => ToggleOn();

				protected override void OnEditMode_ToggleOff() => ToggleOff();

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