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
				/// <param name="ignored"></param>
				public override void Interact(PlayerBehaviour ignored)
				{
						// called from "protected"
						if (ignored is { })
						{
								// do not allow any interaction, if it is not the player
								if (CanInteract(ignored) is false)
								{
										return;
								}
						}

						IsOpened = !IsOpened;
						if (IsOpened)
						{
								OpenIntern();
						}
						else
						{
								CloseIntern();
						}
				}

				/// <summary>
				/// Sets <see cref="IsOpened"/> to false, no animation nor sound
				/// </summary>
				protected void Close() => IsOpened = false;

				[ContextMenu("Interaction/Open")]
				private void OpenIntern()
				{
						PlayAnimation(openAnimation);
				}

				[ContextMenu("Interaction/Close")]
				private void CloseIntern()
				{
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
						// player only!
						return sender is { };
				}

				protected override void OnHuntStart()
				{
						// nothing - but exits (another script)
				}

				protected override void OnHuntStop()
				{
						// nothing
				}

				public override string GetTargetName()
				{
						return GetLockCloseStatusName(room?.RoomName ?? "??");
				}

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
