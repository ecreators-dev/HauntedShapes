using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Makes an equipment placeable using place-button
		/// </summary>
		public abstract class EquipmentPlacable : Equipment, IPlacableEquipment
		{
				private bool oldIsTaken;

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
								OnPlacing();
						}

						Update_HandleToResetPlacing();
						
						oldIsTaken = IsTakenByPlayer;
				}

				private void Update_HandleToResetPlacing()
				{
						bool changedTaken = oldIsTaken != IsTakenByPlayer && IsTakenByPlayer is true;
						if (changedTaken)
						{
								// now can place again: reset!
								if (IsPlaced)
								{
										IsPlaced = false;
								}
						}
				}

				protected virtual void OnPlacing()
				{
						StartPlacing();

						// found place:
						if (ButtonPlacingPressed)
						{
								PlaceItem();
						}
				}

				private void StartPlacing()
				{
						// once!
						if (IsPlacing is false)
						{
								IsPlacing = true;
								CrosshairHit.ShowPlacementPointer(this);
								Debug.Log($"{GetTargetName()}: Start placing");
						}
				}

				protected override void OnEquip()
				{
						base.OnEquip();

						IsPlaced = false;
				}

				protected virtual void PlaceItem()
				{
						if (IsPlacing)
						{
								OnPlaced();
						}
				}

				protected virtual void OnPlaced()
				{
						// do placing
						DropItemRotated(User, noForce: true);
						CrosshairHit.PlaceEquipment(this, UpNormal, true, this.GetGameController().Crosshair.PlacementOffsetNormal);

						IsPlacing = false;
						IsPlaced = true;
						Debug.Log($"{GetTargetName()}: End placing");
				}
		}
}