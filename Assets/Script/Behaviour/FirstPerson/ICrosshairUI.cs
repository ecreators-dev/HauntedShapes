using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		public interface ICrosshairUI
		{
				(HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) RaycastInfo { get; }
				
				(ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) RaycastObjectInfo { get; }
				
				bool IsHovered { get; }
				
				bool IsGamepadConnected { get; }

				void ShowTargetPosition(IPlacableEquipment equipment);
				
				void HideTargetOnce();

				(HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) CustomRaycastIgnoreTriggers(Camera sourceCamera, LayerMaskFilter layers, RangeFilter range, out (ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) objectInfo);
		}
}