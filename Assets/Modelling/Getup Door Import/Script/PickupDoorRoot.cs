using UnityEngine;

namespace HauntedShapes.Doors
{
		[ExecuteInEditMode]
		[DisallowMultipleComponent]
		public class PickupDoorRoot : MonoBehaviour
		{
				[SerializeField] private bool onGround = true;
				[SerializeField] private PickupDoor door;

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