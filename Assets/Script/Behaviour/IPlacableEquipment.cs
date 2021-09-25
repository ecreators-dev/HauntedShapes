using UnityEngine;

namespace Assets.Script.Behaviour
{
		public interface IPlacableEquipment : IEquipment
		{
				bool IsPlaced { get; }

				bool IsUnusedOnFloor { get; }

				bool PlaceAtPositionAndNormal(HitSurfaceInfo clickRange);
				GameObject GetPlacingPrefab();
				void StartPreviewPlacement(IPlacableEquipment placable);
		}
}