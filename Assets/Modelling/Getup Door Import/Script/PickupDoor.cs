using UnityEngine;

namespace HauntedShapes.Doors
{
		[RequireComponent(typeof(Animator))]
		public class PickupDoor : MonoBehaviour
		{
				private Animator animator;


				[Header("Animator Controller - Trigger Names")]
				[SerializeField] private string getupName = "Getup";
				[SerializeField] private string openName = "Open";

				private bool getup;
				private bool open;

				private void Awake()
				{
						animator = GetComponent<Animator>();
				}

				[ContextMenu("Animation/Get up")]
				public void GetUp()
				{
						if (getup)
								return;

						getup = true;
						animator.SetTrigger(getupName);
				}

				[ContextMenu("Animation/Open")]
				public void Open()
				{
						if (open || getup is false)
								return;

						open = true;
						animator.SetTrigger(openName);
				}
		}
}