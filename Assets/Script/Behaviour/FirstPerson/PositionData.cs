
using System;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct PositionData
		{
				public PositionData(Vector3 position, bool local) : this()
				{
						X = position.x;
						Y = position.y;
						Z = position.z;
						IsLocal = local;
				}

				public float X { get; set; }
				public float Y { get; set; }
				public float Z { get; set; }
				public bool IsLocal { get; set; }

				public Vector3 GetVector3() => new Vector3(X, Y, Z);
				public void Load(Transform transform)
				{
						if (IsLocal)
						{
								transform.localPosition = GetVector3();
						}
						else
						{
								transform.position = GetVector3();
						}
				}
				public void SetVector3World(Transform position) => SetVector3(position.position, false);
				public void SetVector3Local(Transform position) => SetVector3(position.localPosition, true);

				public void SetVector3(Vector3 position, bool local)
				{
						X = position.x;
						Y = position.y;
						Z = position.z;
						IsLocal = local;
				}
		}
}
