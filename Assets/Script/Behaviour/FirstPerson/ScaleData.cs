
using System;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct ScaleData
		{
				public float X { get; set; }
				public float Y { get; set; }
				public float Z { get; set; }

				public Vector3 GetVector3() => new Vector3(X, Y, Z);
				public void Load(Transform transform)
				{
						transform.localScale = GetVector3();
				}
				public void SetVector3Local(Transform scale) => SetVector3(scale.localScale);

				public void SetVector3(Vector3 scale)
				{
						X = scale.x;
						Y = scale.y;
						Z = scale.z;
				}
		}
}
