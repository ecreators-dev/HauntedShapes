
using System;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct RotationData
		{
				public float X { get; set; }
				public float Y { get; set; }
				public float Z { get; set; }
				public float W { get; set; }
				public bool IsLocal { get; set; }

				public Quaternion GetQuaternion() => new Quaternion(X, Y, Z, W);
				public void Load(Transform transform)
				{
						if (IsLocal)
						{
								transform.localRotation = GetQuaternion();
						}
						else
						{
								transform.rotation = GetQuaternion();
						}
				}

				public void SetQuaterionWorld(Transform transform) => SetQuaterion(transform.rotation, false);
				public void SetQuaterionLocal(Transform transform) => SetQuaterion(transform.localRotation, true);
				public void SetQuaterion(Quaternion rotation, bool local)
				{
						X = rotation.x;
						Y = rotation.y;
						Z = rotation.z;
						W = rotation.w;
						IsLocal = local;
				}
		}
}
