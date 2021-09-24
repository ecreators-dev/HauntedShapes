using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Makes an equipment placeable using place-button
		/// </summary>
		public abstract class EquipmentPlacable : Equipment, IPlacableEquipment
		{
				// if it is only laying on floor, it is not placed!
				public bool IsPlaced { get; private set; }

				// if it is taken by player and holding placing button, then it is placing and not placed yet
				protected bool IsPlacing { get; private set; }

				public bool IsUnusedOnFloor => IsTakenByPlayer is false && IsPlacing is false && IsPlaced is false;

				public IEquipment Self => this;

				protected override void Update()
				{
						base.Update();

						// while pressing hold
						if (IsUnlocked && IsTakenByPlayer)
						{
								if (this.InputControls().PlaceEquipmentButtonPressed)
								{
										UpdatePlacingOrStart();
								}
								else
								{
										EndPlacing();
								}
						}
				}

				protected virtual void UpdatePlacingOrStart()
				{
						// laying only on floor is:
						// not placing and not placed

						// once!
						if (IsPlacing is false && IsTakenByPlayer)
						{
								IsPlacing = true;
								IsPlaced = false;
								CrosshairHit.ShowPlacementPointer(this);
								Debug.Log($"{GetTargetName()}: Start placing");
						}
				}

				protected virtual void EndPlacing()
				{
						// fix: do not put to 0,0,0 when missing target
						ICrosshairInfo info = User.CrosshairTargetInfo;
						if (IsPlacing && !IsPlaced)
						{
								if (info.Info.InClickRange.IsHit)
								{
										// do placing
										DropItemRotated(User, noForce: true);

										float placementOffsetAtNormal = this.GetGameController().Crosshair.PlacementOffsetNormal;
										CrosshairHit.PlaceEquipment(this, UpNormal, placementOffsetAtNormal);

										IsPlacing = false;
										IsPlaced = true;
										Debug.Log($"{GetTargetName()}: End placing");
								}
								else
								{
										Debug.Log($"{GetTargetName()}: End placing: Cannot put here without target surface!");
										User.AddMessage("Das funktioniert hier nicht");
								}
						}
				}

				public override bool CheckPlayerCanPickUp(PlayerBehaviour player)
				{
						return base.CheckPlayerCanPickUp(player);
				}
		}
}