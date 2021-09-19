using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		public interface ICrosshairUI
		{
				/// <summary>
				/// Does placement position and rotation upward from surface and hides placement sprite
				/// </summary>
				void PlaceEquipment(Equipment equipment, Vector3 up, bool useHitNormal, float normalOffset);

				/// <summary>
				/// Sets an equipment active for beeing placed. Shows a placement sprite at target surface and
				/// how it will be oriented, propably.
				/// </summary>
				void ShowPlacementPointer(Equipment equipmentNotNull);

				(bool actualHit, RaycastHit hit) GetRaycastCollidersOnlyResult();

				(bool hit, Vector3 point, Vector3 normal) RaycastCollidersOnly(Camera camera);
				Transform GetPlacementPosition();
				Vector3 GetPlacementNormal();
		}
}