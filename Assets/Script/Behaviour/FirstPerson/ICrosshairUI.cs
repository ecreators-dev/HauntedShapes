using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		public interface ICrosshairUI
		{
				void SetPlaced(Equipment equipment);

				void SetPlacementEquipment(Equipment equipmentNotNull);

				(bool actualHit, RaycastHit hit) GetRaycastCollidersOnlyResult();

				(bool hit, Vector3 point, Vector3 normal) RaycastCollidersOnly(Camera camera);
				Transform GetPlacementPosition();
				PlacementEnum GetPlacementInfo(out PlacementCheck.HitCheck? info);
		}
}