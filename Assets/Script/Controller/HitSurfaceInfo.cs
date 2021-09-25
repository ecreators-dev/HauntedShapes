using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public struct HitSurfaceInfo
		{
				public HitSurfaceInfo(bool isHit, Vector3 hitPoint, Vector3 normal, GameObject target)
				{
						IsHit = isHit;
						HitPoint = hitPoint;
						Normal = normal;
						Target = target;
				}

				public bool IsHit { get; }
				public Vector3 HitPoint { get; }
				public Vector3 Normal { get; }
				public GameObject Target { get; }
		}
}