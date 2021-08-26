
using UnityEngine;

namespace Assets.Script.Components
{
		/// <summary>
		/// Represents getup-Door or pickup-Door. If or if not a ritual lifts the door to be able to open it.
		/// If a ritual executes the execute-methods the door gets up or closes, if the ritual is ended
		/// </summary>
		public sealed class TeleportationDoor : DoorInteractible
		{
				[SerializeField] private AnimationWithSound getUpAnimation;
				[SerializeField] private Collider doorCollider;
				[SerializeField] private Collider teleportCollider;

				public bool IsUp { get; private set; }

				private void Start()
				{
						// colliders must be set! NPE expected if not!

						doorCollider.isTrigger = false;
						doorCollider.enabled = true; // always!

						teleportCollider.isTrigger = true;
						SetTeleportActive(false);
				}

				private void SetTeleportActive(bool activeStatus)
				{
						teleportCollider.enabled = activeStatus; // changing!
				}

#if UNITY_EDITOR
				[SerializeField] private string idleStateNameInAnimator = "idle";
				[ContextMenu("Animation/Reset")]
				public void ResetAnimation()
				{
						Close();
						IsUp = false;

						if (animator != null)
						{
								// play idle animation:
								animator.Play(idleStateNameInAnimator ??= "idle", -1, 0);
						}
						SetTeleportActive(false);
				}
#endif

				[UnityEventCallBy(typeof(RitualArea))] // for example: reference a unity event inside ritualarea with this method
				public void ExecuteGetUp()
				{
						// if a ritual is active (aligned by unity event with this method)
						// the door gets up with this method:
						IsUp = true;
						PlayAnimation(getUpAnimation);
						SetTeleportActive(false);
				}

				[UnityEventCallBy(typeof(RitualArea))]
				public void ExecuteDisable()
				{
						// after the ritual was successful and interupts
						// the door gets shut and the teleport is offline
						if (IsUp && IsOpened)
						{
								// close intern (includes IsOpened = false, close animation and closing sound)
								base.Interact(null);
								SetTeleportActive(false);
						}
				}

				public override void Interact(GameObject sender)
				{
						// ONE WAY open! Means:
						// can get up, when closed
						// can open, when get up
						// cannot close again
						// cannot get down again

						if (IsUp is false)
						{
								if (IsOpened is false)
								{
										ExecuteGetUp();
								}
						}
						else
						{
								// open and close
								// do only open!
								if (IsOpened is false)
								{
										// always active
										SetTeleportActive(true);
										base.Interact(null);
								}
						}
				}
		}
}
