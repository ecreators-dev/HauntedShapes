using Assets.Script.Behaviour;
using Assets.Script.Model;

using UnityEngine;

namespace HauntedShapes.Doors
{
		[RequireComponent(typeof(Animator))]
		public class PickupDoor : MonoBehaviour, IInteractible
		{
				private Animator animator;

				[Header("Animator Controller - Trigger Names")]
				[SerializeField] private string getupName = "Getup";
				[SerializeField] private string openName = "Open";
				[SerializeField] private Collider passCollider;
				[SerializeField] private Collider doorCollider;
				
				private bool getup;
				private bool open;

				public bool IsPickable { get; }
				public string GameObjectName => this.GetGameObjectName();
				public string ImplementationTypeName => this.GetImplementationTypeName();

				private void Awake()
				{
						animator = GetComponent<Animator>();
				}

				private void Start()
				{
						SetDoorCooliderEnabled(true);
				}

				[ContextMenu("Animation/Reset")]
				internal void ResetDoor()
				{
						getup = false;
						open = false;
						animator.ResetTrigger(getupName);
						animator.ResetTrigger(openName);
						animator.Play("idle", 0, 0f);
				}

				private void SetDoorCooliderEnabled(bool enabled)
				{
						// at start:
						// enabled = true, if door is laying on the floor
						
						doorCollider.enabled = enabled;
						passCollider.enabled = !enabled;
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

						SetDoorCooliderEnabled(false);
				}

				public void TouchClickUpdate()
				{
						
				}

				public void TouchOverUpdate()
				{
						
				}

				public void Drop()
				{
						
				}

				public void OnPickup(PlayerBehaviour player)
				{
						
				}
		}
}