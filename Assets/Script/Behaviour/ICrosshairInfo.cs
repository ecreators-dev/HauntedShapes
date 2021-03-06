using Assets.Script.Components;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public interface ICrosshairInfo
		{
				(HitSurfaceInfo InClickRange, HitSurfaceInfo InHoverRange) RaycastInfo { get; }
				(ObjectHitInfo InClickRange, ObjectHitInfo InHoverRange) RaycastObjectInfo { get; }
		}
}