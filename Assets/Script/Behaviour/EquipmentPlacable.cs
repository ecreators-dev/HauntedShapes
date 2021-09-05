using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Makes an equipment placeable using place-button
		/// </summary>
		public abstract class EquipmentPlacable : Equipment, IPlacableEquipment
		{
				public bool IsPlaced { get; private set; }
				protected bool IsPlacing { get; private set; }
				protected bool ButtonPlacingPressed { get; private set; }
				protected IInputControls InputControls { get; private set; }
				public Equipment Self => this;

				protected override void Update()
				{
						InputControls = this.InputControls();
						ButtonPlacingPressed = InputControls.PlaceEquipmentButtonPressed;

						base.Update();

						if (IsLocked)
								return;

						if (IsTakenByPlayer && IsPlaced is false)
						{
								// once:
								if (IsPlacing is false)
								{
										IsPlacing = true;
										CrosshairHit.ShowPlacementPointer(this);
								}

								// found place:
								if (ButtonPlacingPressed)
								{
										PlaceItem();
								}
						}
				}

				protected override void OnEquip()
				{
						base.OnEquip();

						IsPlaced = false;
				}

				protected virtual void CompletePlacing()
				{
						IsPlacing = false;
						IsPlaced = true;
				}

				protected virtual void PlaceItem()
				{
						if (IsPlacing)
						{
								// do placing
								DropItemRotated(User, noForce: true);
								CrosshairHit.PlaceEquipment(this, UpNormal, true, this.GetGameController().Crosshair.PlacementOffsetNormal);
								CompletePlacing();
						}
				}
		}
}