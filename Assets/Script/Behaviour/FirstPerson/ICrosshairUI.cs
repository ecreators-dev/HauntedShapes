using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		public interface ICrosshairUI
		{
				(HitInfo clickRange, HitInfo hoverRange) RaycastInfo { get; }
				(ObjectHitInfo clickRange, ObjectHitInfo hoverRange) RaycastObjectInfo { get; }
				bool IsHovered { get; }
				bool IsGamepadConnected { get; }

				void ShowTargetPosition(IPlacableEquipment equipment);
				
				void HideTarget();

				(HitInfo ClickRange, HitInfo HoverRange) CustomRaycastIgnoreTriggers(Camera sourceCamera, LayerMaskFilter layers, RangeFilter range, out (ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) objectInfo);
		}
}