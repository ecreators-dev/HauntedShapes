using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Components;

using System.Collections;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public partial class DotsProjectorEquipment : Equipment, IPlacableEquipment
		{
				[SerializeField] private Rigidbody RigidBody;
				[SerializeField] private ShopParameters shopInfo;
				[SerializeField] private Animator animator;
				[SerializeField] private string brokenText = "Gerät ist kaputt";
				[SerializeField] private PlacementCheck placementCheck;
				private bool placing;
				private IInputControls inputControls;

				private PlacementEnum PlacementStatus { get; set; } = PlacementEnum.NONE;

				public bool IsPlaced => PlacementStatus != PlacementEnum.NONE;

				private Transform Transform { get; set; }

				public Equipment Self => this;

				private bool ButtonCrosshairTargetPressed { get; set; }
				private bool ButtonPlacingPressed { get; set; }
				private bool CrosshairHovered { get; set; }

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

						// cannot interact, when locked
						if (IsLocked)
								return;

						inputControls ??= this.InputControls();
						ButtonPlacingPressed = inputControls.PlaceEquipmentButtonPressed;
						ButtonCrosshairTargetPressed = inputControls.CrosshairTargetInteractionButtonPressed;
						CrosshairHovered = base.IsCrosshairHovered;

						Update_HandleNotPlaced_FindPlace();
						Update_HandlePlaced_FindPlacementType();
				}

				private void Update_HandleNotPlaced_FindPlace()
				{
						// in hand finding place:
						if (IsTakenByPlayer && IsPlaced is false)
						{
								// once:
								EnableFindPlacementOnce();

								// found place:
								if (ButtonPlacingPressed)
								{
										Debug.Log("Equipment placed button pressed");
										InHand_PlaceButtonPressed();
								}
						}
				}

				private void Update_HandlePlaced_FindPlacementType()
				{
						// placed, but without placement target type:
						// find placement type
						if (IsTakenByPlayer is false
								&& IsPlaced is false
								// was placed!
								&& placing)
						{
								const bool QuickFix = true;
								if (QuickFix)
								{
										placing = false;
										PlacementStatus = PlacementEnum.WALL; // any NONE value possible
								}
								else // BUGFIX TODO: placementcheck not working correct
								{
										// check until matched:
										PlacementStatus = placementCheck.GetPlacementType(out _);
										// now found:
										if (IsPlaced)
										{
												// reset: can find place again in future
												placing = false;
										}
								}
						}
				}

				private void EnableFindPlacementOnce()
				{
						if (placing is false)
						{
								placing = true;
								CrosshairHit.ShowPlacementPointer(this);
						}
				}

				private void InHand_PlaceButtonPressed()
				{
						PlaceItem();
				}

				protected override void OnEquip()
				{
						base.OnEquip();

						PlacementStatus = PlacementEnum.NONE;
						Debug.Log($"'{GetTargetName()}' equipped: reset on placing-status to NONE");
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return IsLocked is false;
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (sender == User || IsPlaced)
						{
								TogglePowered();
						}
				}

				private void PlaceItem()
				{
						// do placing
						DropItem(User, noForce: true);
						CrosshairHit.PlaceEquipment(this, Vector3.up, true);
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