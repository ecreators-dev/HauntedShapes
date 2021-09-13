
using System;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct TransformData
		{
				public bool IsLocale { get; private set; }
				public PositionData Position { get; set; }

				public RotationData Rotation { get; set; }

				public ScaleData Scale { get; set; }

				public void Load(Transform transform)
				{
						Position.Load(transform);
						Rotation.Load(transform);
						Scale.Load(transform);
				}

				public void SetTransform(Transform transform, bool local)
				{
						IsLocale = local;
						Position = new PositionData();
						if (local)
						{
								Position.SetVector3Local(transform);
						}
						else
						{
								Position.SetVector3World(transform);
						}
						Rotation = new RotationData();
						if (local)
						{
								Rotation.SetQuaterionLocal(transform);
						}
						else
						{
								Rotation.SetQuaterionWorld(transform);
						}

						Scale = new ScaleData();
						Scale.SetVector3Local(transform);
				}
		}
}
