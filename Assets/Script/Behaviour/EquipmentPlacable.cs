using Assets.Script.Behaviour.FirstPerson;

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

				public ICrosshairUI CrosshairUI { get; private set; }

				protected override void Update()
				{
						base.Update();

						CrosshairUI ??= CrosshairHitVisual.Instance;

						// while pressing hold
						if (CrosshairUI != null)
						{
								// while holding:
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

						//CrosshairHit.ShowPlacementPointer(this);
						Debug.Log($"{GetTargetName()}: Start placing");

						CrosshairHit.ShowTargetPosition(CrosshairUI.RaycastInfo.clickRange, this);
				}

				protected virtual void EndPlacing()
				{
						// fix: do not put to 0,0,0 when missing target
						var info = CrosshairHitVisual.Instance.RaycastInfo;
						if (IsPlacing && !IsPlaced)
						{
								if (info.clickRange.IsHit)
								{
										// do placing
										DropItemRotated(User, noForce: true);

										CrosshairHit.HideTarget();

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
						// only if unlocked not player and not fixed (or placed).
						// if placed then first grab it with another button (not interaction button)
						return base.CheckPlayerCanPickUp(player) && IsPlaced is false;
				}
		}
}