using Assets.Script.Components;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public interface ICrosshairInfo
		{
				(HitInfo InClickRange, HitInfo InHoverRange) Info { get; }
				(ObjectHitInfo InClickRange, ObjectHitInfo InHoverRange) ObjectInfo { get; }
		}
}