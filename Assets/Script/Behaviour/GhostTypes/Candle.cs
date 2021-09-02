using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		[RequireComponent(typeof(Rigidbody))]
		public class Candle : Equipment, IPforteTrigger, ILightSource
		{
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[Min(1)]
				[SerializeField] private float lightSourceMultiplier = 1.4f;
				[SerializeField] private string brokenText = "abgebrannt";

				public PforteTriggerTypeEnum TriggerType => PforteTriggerTypeEnum.KERZE;

				public float ActiveMultiplier => lightSourceMultiplier;

				public bool IsActive => IsPowered;

				private void Start()
				{
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

				protected override void OnEquip()
				{
				}

				protected override void OnInventory()
				{
				}

				protected override void OnOwnerOwnedEquipment()
				{
						// when paid and owner set: nothing
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return User == null || User == sender;
				}

				public override void Interact(PlayerBehaviour senderIgnored)
				{
						if (IsActive)
						{
								ToggleOff();
						}
						else
						{
								ToggleOn();
						}
				}

				protected override void OnHuntStart()
				{
						// stays on / off
				}

				protected override void OnHuntStop()
				{
						// stays on / off
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