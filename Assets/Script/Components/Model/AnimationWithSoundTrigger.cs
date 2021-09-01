using System;

using UnityEngine;

namespace Assets.Script.Components
{
		[CreateAssetMenu(fileName = "Animation Sound Reference", menuName = "Game/Animation und Audio (Trigger)")]
		public class AnimationWithSoundTrigger : ScriptableObject
		{
				public string triggerName;
				public AudioClip audio;
				public float volume;
				private bool playing;

				public void ResetOnce()
				{
						playing = false;
				}
				
				public void PlayAudio(Transform transform, bool playOnce = true)
				{
						if (audio == null)
						{
								Debug.LogError($"Missing audio: {name}: {triggerName}");
								return;
						}

						if(playOnce && playing)
						{
								return;
						}

						AudioSource.PlayClipAtPoint(audio, transform.position, volume);
						playing = true;
				}

				public void StartTriggerAnimation(Animator animator)
				{
						if (animator is { })
						{
								animator.SetTrigger(triggerName);
						}
				}

				public void Play(Transform target, Animator animator)
				{
						PlayAudio(target);
						StartTriggerAnimation(animator);
				}
		}
}
