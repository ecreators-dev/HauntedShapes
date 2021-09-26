using Assets.Door;
using Assets.Script.Behaviour;
using Assets.Script.Components;

using System.Collections.Generic;

using UnityEngine;

namespace HauntedShapes.Doors
{
		[RequireComponent(typeof(Animator))]
		public class GetUpDoor : Interactible
		{
				[Header("Animator Controller - Trigger Names")]
				[SerializeField] private string getupName = "Getup";
				[SerializeField] private string openName = "Open";
				[SerializeField] private Collider passCollider;
				[SerializeField] private Collider doorCollider;
				[SerializeField] private Light doorLight;
				[SerializeField] private TriggerTeleporter doorTeleporter;
				[SerializeField] private SoundEffect pullChainsAudio;
				[SerializeField] private AudioSource pullChainsAudioSource;
				[SerializeField] private SoundEffect ambienceHellAudio;
				[SerializeField] private AudioSource ambienceHellAudioSource;

				private Animator animator;
				private bool getup;
				private bool open;
				private List<PlayerBehaviour> lastTeleported;

				public List<PlayerBehaviour> TeleportedPlayers { get; private set; }
				public List<PlayerBehaviour> ReturnedPlayers { get; private set; }

				private void Awake()
				{
						animator = GetComponent<Animator>();
				}

				protected override void Update()
				{
						base.Update();

						List<PlayerBehaviour> teleported = new List<PlayerBehaviour>();
						List<PlayerBehaviour> returned = new List<PlayerBehaviour>();

						foreach (PlayerBehaviour current in doorTeleporter.TeleportedPlayers)
						{
								if (lastTeleported == null || lastTeleported.Contains(current) is false)
								{
										teleported.Add(current);
								}
						}
						if (lastTeleported != null)
						{
								foreach (PlayerBehaviour current in lastTeleported)
								{
										if (doorTeleporter.TeleportedPlayers == null || doorTeleporter.TeleportedPlayers.Contains(current) is false)
										{
												returned.Add(current);
										}
								}
						}

						lastTeleported = new List<PlayerBehaviour>(doorTeleporter.TeleportedPlayers);
						TeleportedPlayers = teleported;
						ReturnedPlayers = returned;
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

				public override bool CanInteract(PlayerBehaviour sender)
				{
						// for testing: in game, the player will need to have at least level 10
						// the player need to unlock first a ritual area to see the door (only the player with lavel >= 10 can see)
						// the player can then interact to pick up or to open
						UnlockForTesting();

						return base.CanInteract(sender) && (getup is false || open is false);
				}

				protected override void Interact(PlayerBehaviour sender)
				{
						if (getup is false)
						{
								GetUp();
						}
						else if (open is false)
						{
								Open();
						}
				}

				public override string GetTargetName()
				{
						if (IsLocked)
						{
								return $"{gameObject.name} (Lv {10})";
						}
						return $"{gameObject.name}";
				}

				/// <summary>
				/// Get Up Start
				/// </summary>
				public override void OnAnimation_ToggleOn_Start()
				{
						base.OnAnimation_ToggleOn_Start();
						pullChainsAudio.PlayRandomLoop(GetTargetName(), "pull chains", pullChainsAudioSource);
				}

				/// <summary>
				/// Get Up End
				/// </summary>
				public override void OnAnimation_ToggleOn_End()
				{
						//base.OnAnimation_ToggleOn_End();
						StopPlaybackToggleOnOrOff();
						pullChainsAudio.StopLoop();
				}

				/// <summary>
				/// Open Start
				/// </summary>
				public override void OnAnimation_ToggleOff_Start()
				{
						base.OnAnimation_ToggleOff_Start();
						ambienceHellAudio.PlayRandomLoop(GetTargetName(), "ambience hell open", ambienceHellAudioSource);
						// keep running
				}

				/// <summary>
				/// Open End
				/// </summary>
				public override void OnAnimation_ToggleOff_End()
				{
						PlayAmbientExplicitFromScript();
				}
		}
}