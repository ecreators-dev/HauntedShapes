using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		public interface ICrosshairUI
		{
				/// <summary>
				/// Does placement position and rotation upward from surface and hides placement sprite
				/// </summary>
				void PlaceEquipment(Equipment equipment, Vector3 up, float normalOffset);
				
				/// <summary>
				/// Does placement position and rotation upward from surface and hides placement sprite
				/// </summary>
				void PlaceEquipment(Equipment equipment, float normalOffset);

				/// <summary>
				/// Sets an equipment active for beeing placed. Shows a placement sprite at target surface and
				/// how it will be oriented, propably.
				/// </summary>
				void ShowPlacementPointer(Equipment equipmentNotNull);

				(bool actualHit, RaycastHit hit) GetRaycastCollidersOnlyResult();

				(HitInfo clickRange, HitInfo hoverRange) RaycastCollidersOnlyAllLayers(Camera camera, float clickDistance = 6, float hoverDistance = 8);

				(HitInfo clickRange, HitInfo hoverRange) RaycastCollidersOnly(Camera camera, ISet<LayerMask> hitMasks, ISet<LayerMask> avoidMasks, float clickDistance = 6, float hoverDistance = 8);
				
				Transform GetPlacementPosition();
				Vector3 GetPlacementNormal();
				void HidePlacementPointer();

				bool TryGetResult(out bool inRange, out GameObject target, out Vector3 hitPoint, out Vector3 normal);
		}
}