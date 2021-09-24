using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class DotsProjectorEquipment : EquipmentPlacable
		{
				[SerializeField] private Rigidbody rigidBody;
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[SerializeField] private string brokenText = "Gerät ist kaputt";

				private bool CrosshairHovered { get; set; }

				protected override void Start()
				{
						base.Start();

						RigidBody = rigidBody;
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
						return base.CanInteract(sender) && (User == null || sender == User);
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsTakenByPlayer)
						{
								TogglePowered();
						}
						else
						{
								if (IsPlaced is false)
								{
										CrosshairHit.ShowPlacementPointer(this);
								}
								else if (CrosshairHovered)
								{
										TogglePowered();
								}
						}
				}

				protected override void EndPlacing()
				{
						base.EndPlacing();
						DisableGravity();
				}


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