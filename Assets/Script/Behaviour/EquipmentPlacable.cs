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

				public override bool CheckPlayerCanPickUp(PlayerBehaviour player)
				{
						// only if unlocked not player and not fixed (or placed).
						// if placed then first grab it with another button (not interaction button)
						return base.CheckPlayerCanPickUp(player) && IsPlaced is false;
				}

				public virtual void PlaceAtPositionAndNormal(HitInfo surfaceClick)
				{
						if (IsPlacing && !IsPlaced)
						{
								// fix: do not put to 0,0,0 when missing target
								if (surfaceClick.IsHit)
								{
										Transform.up = surfaceClick.Normal;
										Transform.rotation = Quaternion.FromToRotation(NormalUp, surfaceClick.Normal);
										Transform.position = surfaceClick.HitPoint + Transform.up * this.GetGameController().Crosshair.PlacementOffsetNormal;
										Transform.SetParent(null);

										IsPlacing = false;
										IsPlaced = true;
										Debug.Log($"{base.GetTargetName()}: End placing @ {GetPrintablePosition(Transform.position)}");
								}
								else
								{
										Debug.Log($"{GetTargetName()}: End placing: Cannot put here without target surface!");
										User.AddMessage("Das funktioniert hier nicht");
								}
						}
				}

				protected static string GetPrintablePosition(Vector3 position)
				{
						return $"xyz={position.x:0.0},{position.y:0.0},{position.z:0.0}";
				}
		}
}