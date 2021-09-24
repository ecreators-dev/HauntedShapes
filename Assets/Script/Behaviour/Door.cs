using Assets.Script.Components;

using System.Linq;

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
				[SerializeField] private bool canBeOpenedDuringHunt;

				private Animator animator;

				public bool IsOpened { get => opened; }

				protected Transform Transform { get; private set; }

				private void Awake()
				{
						Transform = transform;
						animator = GetComponent<Animator>();
						GetToggleOff().playedFromScript = true;
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

				protected override void Interact(PlayerBehaviour sender)
				{
						if (IsHuntingActive)
						{
								// player can hold close with placing button
								if (this.InputControls().PlaceEquipmentButtonPressed)
								{
										Close();
								}
								else if (canBeOpenedDuringHunt)
								{
										// player can open/close with interaction button
										ToggleOpenClose();
								}
						}
						else
						{
								// player can open/close with interaction button
								ToggleOpenClose();
						}
				}

				private void ToggleOpenClose()
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

				public override void OnAnimation_ToggleOn_Start()
				{
						base.OnAnimation_ToggleOn_Start();
				}

				public override void OnAnimation_ToggleOn_End()
				{
						// automatically - see base implementation
				}

				public override void OnAnimation_ToggleOff_Start()
				{
						// automatically - see base implementation
				}

				public override void OnAnimation_ToggleOff_End()
				{
						// manually OK
						PlayToggleOffSoundExplicitFromScript();
				}
		}
}