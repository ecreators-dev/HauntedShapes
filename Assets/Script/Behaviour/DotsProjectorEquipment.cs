using UnityEngine;

namespace Assets.Script.Behaviour
{
		public partial class DotsProjectorEquipment : Equipment
		{
				[SerializeField] private Rigidbody RigidBody;
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[SerializeField] private string brokenText = "Gerät ist kaputt";

				private PlacementEnum PlacementStatus { get; set; } = PlacementEnum.NONE;

				private bool IsPlaced => PlacementStatus != PlacementEnum.NONE;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						SetShopInfo(shopInfo, this);
				}

				protected override void Update()
				{
						base.Update();
						animator.SetBool("Hunting", IsHuntingActive);
						animator.SetBool("PowerOn", IsPowered);

						// no matter if in hand of player:
						if (IsPlaced)
						{
								switch (PlacementStatus)
								{
										case PlacementEnum.CEILING:
										case PlacementEnum.WALL:
												DisableFalling();
												break;
										default:
												EnableFalling();
												break;
								}
						}

						// in players hand
						if (IsTakenByPlayer)
						{
								CrosshairHit.SetPlacementEquipment(this);
								// interacting is placing it first
						}
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return IsLocked is false && (User == null || User == sender);
				}

				public override void Interact(PlayerBehaviour sender)
				{
						TogglePowered();
				}

				private void PlaceItem()
				{
						// do placing
						if (TryGetCrosshairHitInfo(out var position, out var normal, out var type))
						{
								Transform.position = position;
								Transform.up = normal;
						}
						PlacementStatus = type;
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