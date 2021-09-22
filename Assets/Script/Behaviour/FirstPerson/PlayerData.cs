using System;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public struct PlayerData
		{
				public bool IsActive { get; set; }
				public TransformData Transform { get; set; }
				public CameraData Camera { get; set; }

				public void Update(FirstPersonView player)
				{
						IsActive = player.gameObject.activeSelf;
						Transform.SetTransform(player.transform, false);
				}

				public void Load(FirstPersonView player)
				{
						player.gameObject.SetActive(IsActive);
						Transform.Load(player.transform);
				}
		}
}
