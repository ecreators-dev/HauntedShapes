using Assets.Script.Behaviour;

using UnityEngine;

namespace Assets.Script.Components
{
		public class DoorInteractible : Interactible
		{
				[Header("Animator - Trigger Names")]
				[SerializeField] protected Animator animator;
				[SerializeField] private AnimationWithSoundTrigger openAnimation;
				[SerializeField] private AnimationWithSoundTrigger closeAnimation;
				[SerializeField] private GhostRoom room;

				public bool IsOpened { get; private set; }

				/// <summary>
				/// An interaction call from a player
				/// </summary>
				/// <param name="player"></param>
				protected override void Interact(PlayerBehaviour player)
				{
						ToggleOpenClose();
				}

				private void ToggleOpenClose()
				{
						IsOpened = !IsOpened;
						if (IsOpened)
						{
								Open();
						}
						else
						{
								Close();
						}
				}

				[ContextMenu("Interaction/Open")]
				private void Open()
				{
						IsOpened = true;
						PlayAnimation(openAnimation);
				}

				[ContextMenu("Interaction/Close")]
				protected void Close()
				{
						IsOpened = false;
						PlayAnimation(closeAnimation);
				}

				protected void PlayAnimation(AnimationWithSoundTrigger animation)
				{
						animation?.PlayAudio(transform);
						if (animator != null && animation.triggerName is { })
						{
								animator.SetTrigger(animation.triggerName);
						}
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						// player only, if not locked!
						return base.CanInteract(sender) && sender != null;
				}

				public override string GetTargetName() => GetLockCloseStatusName(room?.RoomName ?? "??");

				private string GetLockCloseStatusName(string name)
				{
						if (IsLocked)
						{
								name = $"{name} - locked";
						}
						else if (!IsOpened)
						{
								name = $"{name} - closed";
						}
						return name;
				}
		}
}
