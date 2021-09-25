using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Makes an equipment placeable using place-button
		/// </summary>
		public abstract class EquipmentPlacable : Equipment, IPlacableEquipment
		{
				[Tooltip("If unset (null), this.gameObject will be used")]
				[SerializeField] private GameObject placingPrefab;

				// if it is only laying on floor, it is not placed!
				public bool IsPlaced { get; private set; }

				// if it is taken by player and holding placing button, then it is placing and not placed yet
				protected bool IsPlacing { get; private set; }

				public bool IsUnusedOnFloor => IsTakenByPlayer is false && IsPlacing is false && IsPlaced is false;

				protected override void OnPickedUp()
				{
						base.OnPickedUp();

						IsPlacing = false;
						IsPlaced = false;
				}

				public virtual bool PlaceAtPositionAndNormal(HitSurfaceInfo surfaceInfo)
				{
						if (!IsPlaced)
						{
								// fix: do not put to 0,0,0 when missing target
								if (surfaceInfo.IsHit)
								{
										Transform.position = surfaceInfo.HitPoint + surfaceInfo.Normal * this.GetGameController().Crosshair.PlacementOffsetNormal;
										Transform.rotation = Quaternion.FromToRotation(NormalUp, surfaceInfo.Normal);

										IsPlacing = false;
										IsPlaced = true;
										Debug.Log($"{base.GetTargetName()}: End placing @ {GetPrintablePosition(Transform.position)}");
										return true;
								}
								else
								{
										Debug.Log($"{GetTargetName()}: End placing: Cannot put here without target surface!");
										User.AddMessage("Das funktioniert hier nicht");
								}
						}
						return false;
				}

				protected static string GetPrintablePosition(Vector3 position)
				{
						return $"xyz={position.x:0.0},{position.y:0.0},{position.z:0.0}";
				}

				public GameObject GetPlacingPrefab()
				{
						return this.placingPrefab ?? gameObject;
				}

				public void StartPreviewPlacement(IPlacableEquipment original)
				{
						RigidBody = GetComponent<Rigidbody>();
						DisableGravity();
						RigidBody.detectCollisions = false;
						SetShopInfo(original.ShopInfo, (Equipment)original);
				}
		}
}