using System;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct CameraData
		{
				public bool IsActive { get; set; }
				public TransformData Transform { get; set; }

				public void Update(Camera cam)
				{
						IsActive = cam.gameObject.activeSelf;
						Transform.SetTransform(cam.transform, true);
				}

				public void Load(Camera cam)
				{
						cam.gameObject.SetActive(IsActive);
						Transform.Load(cam.transform);
				}
		}
}