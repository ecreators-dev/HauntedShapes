using Assets.Script.Behaviour;

using System;

using UnityEngine;

namespace Assets.Script.Components
{
		public abstract class Interactible : MonoBehaviour
		{
				[SerializeField] protected EObjectType identifier = EObjectType.UNDEFINED;
				[SerializeField] private bool locked;

				[Header("Sound Effects")]
				[SerializeField] private AudioSource soundPlayer3d;
				[SerializeField] private SoundEffect toggleOn = new SoundEffect();
				[SerializeField] private SoundEffect toggleOff = new SoundEffect();

				[Serializable]
				public class SoundEffect
				{
						public AudioClip[] randomSounds;
						[Range(0, 1)]
						public float volume = 1;

						public void PlayRandomOnce(string ownerName, string effectName, AudioSource soundPlayer3d)
						{
								if (randomSounds is { } && randomSounds.Length > 0)
								{
										AudioClip clip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)];
										soundPlayer3d.PlayOneShot(clip, volume);
								}
								else
								{
										Debug.LogWarning($"{ownerName}: no {effectName}-sounds!");
								}
						}
				}

				private bool oldHuntingStatus;

				public EObjectType ObjectType => identifier;

				protected bool IsHuntingActive { get; private set; }

				protected bool IsHuntingActiveChanged { get; private set; }

				public bool IsLocked => locked;

				public bool IsUnlocked => !locked;

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

				public abstract void Interact(PlayerBehaviour sender);

				/// <summary>
				/// Not empty Method! Calls <see cref="PerformHuntUpdate"/>
				/// </summary>
				protected virtual void Update()
				{
						PerformHuntUpdate();
				}

				/// <summary>
				/// Is called INSIDE Update.<br/>
				/// Updates <see cref="IsHuntingActive"/> and <see cref="IsHuntingActiveChanged"/><br/>
				/// to call <see cref="OnHuntStart"/> or <see cref="OnHuntStop"/>
				/// </summary>
				protected void PerformHuntUpdate()
				{
						IsHuntingActive = Beans.InHunt;
						IsHuntingActiveChanged = IsHuntingActive != oldHuntingStatus;

						if (IsHuntingActiveChanged)
						{
								if (oldHuntingStatus)
								{
										OnHuntStop();
								}
								else
								{
										OnHuntStart();
								}
						}

						oldHuntingStatus = IsHuntingActive;
				}

				protected virtual void OnHuntStart() { }

				protected void UnlockForTesting()
				{
						Unlock();
						Debug.Log("Testing Settings: always unlocked!");
				}

				protected virtual void OnHuntStop() { }

				public abstract string GetTargetName();

				protected void PlayToggleOnSound()
				{
						if (soundPlayer3d is { })
						{
								toggleOn.PlayRandomOnce(GetTargetName(), "toggle on", soundPlayer3d);
						}
						else
						{
								Debug.LogWarning($"{GetTargetName()}: missing Audio Source (Component)!");
						}
				}

				protected void PlayToggleOffSound()
				{
						if (soundPlayer3d is { })
						{
								toggleOff.PlayRandomOnce(GetTargetName(), "toggle off", soundPlayer3d);
						}
						else
						{
								Debug.LogWarning($"{GetTargetName()}: missing Audio Source (Component)!");
						}
				}
		}
}
