using Assets.Script.Components;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[ExecuteAlways]
		[RequireComponent(typeof(Animator))]
		public class Door : Interactible
		{
				[Tooltip("Init open status")]
				[SerializeField] private bool opened = false;
				[SerializeField] private AnimationWithSoundTrigger open;
				[SerializeField] private AnimationWithSoundTrigger close;
				[Min(0.005f)]
				[SerializeField] private float openSpeed = 1;
				[Min(0.005f)]
				[SerializeField] private float closeSpeed = 1.1f;
				[SerializeField] private Room room;

				private Animator animator;

				public bool IsOpened { get => opened; }

				protected Transform Transform { get; private set; }

				private void Awake()
				{
						Transform = transform;
						animator= GetComponent<Animator>();
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

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return IsLocked is false;
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsOpened)
						{
								Close();
						}
						else
						{
								Open();
						}
				}

				private void Open()
				{
						opened = true;
						open?.Play(Transform, animator);
						animator.SetFloat("Speed", openSpeed);
				}

				private void Close()
				{
						opened = false;
						close?.Play(Transform, animator);
						animator.SetFloat("Speed", closeSpeed);
				}
		}
}