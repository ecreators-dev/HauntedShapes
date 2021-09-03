using UnityEngine;

namespace HauntedShapes.Doors
{
		[ExecuteInEditMode]
		[DisallowMultipleComponent]
		public class GetUpDoorBehaviour : MonoBehaviour
		{
				[SerializeField] private bool onGround = true;
				[SerializeField] private GetUpDoor door;

				[ContextMenu("Animation/Reset Door")]
				public void ResetDoor()
				{
						door.ResetDoor();
				}

				private void Update()
				{
						if (door == null)
								return;

						if (onGround)
						{
								door.transform.localRotation = Quaternion.Euler(-90, 0, 0);
						}
						else
						{
								door.transform.localRotation = Quaternion.Euler(0, 0, 0);
						}
				}
		}
}