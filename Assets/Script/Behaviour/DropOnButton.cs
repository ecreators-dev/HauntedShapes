using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Put this on every pickable item!
		/// </summary>
		[RequireComponent(typeof(PickupItem))]
		public class DropOnButton : MonoBehaviour
		{
				private PickupItem reference;

				private void Awake()
				{
						this.reference = GetComponent<PickupItem>();
				}


				private void Update()
				{
						if (reference.IsTakenByPlayer)
						{
								if (this.InputControls().DropItemButtonPressed)
								{
										reference.DropItemRotated(reference.User);
								}
						}
				}
		}
}