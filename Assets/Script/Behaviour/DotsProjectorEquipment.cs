using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class DotsProjectorEquipment : EquipmentPlacable
		{
				[SerializeField] private Rigidbody rigidBody;
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[SerializeField] private string brokenText = "Gerät ist kaputt";

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
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender) && IsUserOrNotTaken(sender);
				}

				protected override void Interact(PlayerBehaviour sender)
				{
						if (IsHuntingActive is false && (IsTakenByPlayer || IsPlaced))
						{
								TogglePowered();
						}
				}

				public override bool PlaceAtPositionAndNormal(HitSurfaceInfo surfaceClick)
				{
						if (base.PlaceAtPositionAndNormal(surfaceClick))
						{
								DisableGravity();
								return true;
						}
						return false;
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