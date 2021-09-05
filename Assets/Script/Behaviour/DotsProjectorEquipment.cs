using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class DotsProjectorEquipment : EquipmentPlacable
		{
				[SerializeField] private Rigidbody RigidBody;
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[SerializeField] private string brokenText = "Gerät ist kaputt";

				private bool CrosshairHovered { get; set; }

				private void Start()
				{
						SetShopInfo(shopInfo, this);
				}

				protected override void Update()
				{
						base.Update();
						animator.SetBool("Hunting", IsHuntingActive);
						animator.SetBool("PowerOn", IsPowered);
						
						CrosshairHovered = IsCrosshairHovered;

						// cannot interact, when locked
						if (IsLocked)
								return;
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return IsLocked is false;
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsTakenByPlayer)
						{
								if (sender == User)
										TogglePowered();
						}
						else
						{
								if (IsPlaced && CrosshairHovered)
										TogglePowered();
						}
				}

				protected override void PlaceItem()
				{
						base.PlaceItem();
						DisableFalling();
				}

				private void DisableFalling() => RigidBody.isKinematic = true;

				private void EnableFalling() => RigidBody.isKinematic = false;

				protected override void OnEditMode_ToggleOn() => SetPowered(true);

				protected override void OnEditMode_ToggleOff() => SetPowered(false);

				public override EquipmentInfo GetEquipmentInfo()
				{
						string text = null;
						if (IsBroken)
						{
								text = brokenText;
						}
						return text != null ? new EquipmentInfo { Text = text, TimerText = null } : null;
				}
		}
}