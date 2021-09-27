﻿using Assets.Script.Behaviour;

using System;

using UnityEngine;

namespace Assets.Script.Components
{
		public abstract class Interactible : HuntEventObject, IInteractible
		{
				[SerializeField] protected EObjectType identifier = EObjectType.UNDEFINED;
				[SerializeField] private bool locked;

				[Header("Sound Effects")]
				[SerializeField] private AudioSource soundPlayer3d;
				[SerializeField] protected AudioSource soundPlayer3dAmbient;
				[SerializeField] private SoundEffect toggleOn = new SoundEffect();
				[SerializeField] private SoundEffect toggleOff = new SoundEffect();
				[SerializeField] private SoundEffect ambientLoopOn = new SoundEffect();
				[SerializeField] private bool ambientLoopWithToggleOn = false;
				[Range(0, 1)]
				[Tooltip("Timeout between interactions in seconds")]
				[SerializeField] private float interactionLockTimeout = 0.3f;

				private float interactionTime;

				public EObjectType ObjectType => identifier;

				public bool IsLocked => locked;

				public bool IsUnlocked => !locked;

				protected SoundEffect GetToggleOn() => toggleOn;
				protected SoundEffect GetToggleOff() => toggleOff;

				public virtual void OnCollisionEnterChild(Collision collision)
				{ }

				public virtual void OnCollisionExitChild(Collision collision)
				{ }

				protected SoundEffect GetAmbientOn() => ambientLoopOn;

				protected void Lock()
				{
						bool old = locked;
						locked = true;
						if (old != locked)
						{
								OnLockedStateChanged(old, locked);
						}
				}

				protected void Unlock()
				{
						bool old = locked;
						locked = false;
						if (old != locked)
						{
								OnLockedStateChanged(old, locked);
						}
				}

				protected virtual void OnLockedStateChanged(bool old, bool locked)
				{
				}

				public virtual bool CanInteract(PlayerBehaviour sender)
				{
						return IsLocked is false;
				}

				public bool IsInteractionTimeout => Time.time - interactionTime > interactionLockTimeout;

				public bool RunInteraction(PlayerBehaviour sender)
				{
						if (CanRunInteraction(sender))
						{
								interactionTime = Time.time;
								Interact(sender);
								return true;
						}
						return false;
				}

				protected bool CanRunInteraction(PlayerBehaviour sender) => CanInteract(sender) && IsInteractionTimeout;

				protected abstract void Interact(PlayerBehaviour sender);

				/// <summary>
				/// Not empty Method! Calls <see cref="PerformHuntUpdate"/>
				/// </summary>
				protected override void Update()
				{
						base.Update();
				}

				protected void UnlockForTesting()
				{
						Unlock();
						Debug.Log("Testing Settings: always unlocked!");
				}

				public abstract string GetTargetName();

				protected void PlayToggleOnSoundInternal(bool fireAndForget = false)
				{
						if (toggleOn != null && toggleOn.playedFromScript is false)
						{
								PlayToggleOnSoundExplicitFromScript(fireAndForget);
						}
				}

				protected void PlayAmbientInternal()
				{
						if (toggleOn != null && ambientLoopOn.playedFromScript is false)
						{
								PlayAmbientExplicitFromScript();
						}
				}

				protected void PlayToggleOnSoundExplicitFromScript(bool fireAndForget = false)
				{
						if (soundPlayer3d is { })
						{
								Vector3? fireAndForgetPosition = fireAndForget ? transform.position : default;
								toggleOn.PlayRandomOnce(GetTargetName(), "toggle on", soundPlayer3d, fireAndForgetPosition);

								if (ambientLoopWithToggleOn)
								{
										PlayAmbientExplicitFromScript();
								}
						}
						else
						{
								Debug.LogWarning($"{GetTargetName()}: missing Audio Source (Component)!");
						}
				}

				protected void PlayAmbientExplicitFromScript()
				{
						if (soundPlayer3d is { })
						{
								ambientLoopOn.PlayRandomLoop(GetTargetName(), "toggle on ambient", soundPlayer3dAmbient);
						}
						else
						{
								Debug.LogWarning($"{GetTargetName()}: missing Audio Source (Component)!");
						}
				}

				protected void PlayToggleOffSoundInternal(bool fireAndForget = false)
				{
						if (toggleOff != null && toggleOff.playedFromScript is false)
						{
								PlayToggleOffSoundExplicitFromScript(fireAndForget);
						}
				}

				protected void PlayToggleOffSoundExplicitFromScript(bool fireAndForget = false)
				{
						if (soundPlayer3d is { })
						{
								Vector3? fireAndForgetPosition = fireAndForget ? transform.position : default;
								toggleOff.PlayRandomOnce(GetTargetName(), "toggle off", soundPlayer3d, fireAndForgetPosition);
								ambientLoopOn.StopLoop();
						}
						else
						{
								Debug.LogWarning($"{GetTargetName()}: missing Audio Source (Component)!");
						}
				}

				/// <summary>
				/// Play Audio Toggle On
				/// </summary>
				public virtual void OnAnimation_ToggleOn_Start()
				{
						PlayToggleOnSoundExplicitFromScript();
				}

				protected void StopPlaybackToggleOnOrOff()
				{
						if (soundPlayer3d != null)
						{
								soundPlayer3d.Stop();
						}
				}

				/// <summary>
				/// Plays no audio in default
				/// </summary>
				public virtual void OnAnimation_ToggleOn_End()
				{
						// nothing to play here yet
				}

				/// <summary>
				/// Play Audio Toggle Off
				/// </summary>
				public virtual void OnAnimation_ToggleOff_Start()
				{
						PlayToggleOffSoundExplicitFromScript();
				}

				/// <summary>
				/// Plays no audio in default
				/// </summary>
				public virtual void OnAnimation_ToggleOff_End()
				{
						// nothing to play here yet
				}
		}
}
