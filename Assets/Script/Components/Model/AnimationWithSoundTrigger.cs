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

				public void PlayAudio(Transform transform)
				{
						if (audio == null)
						{
								Debug.LogWarning($"Missing audio: {name}: {triggerName}");
								return;
						}

						AudioSource.PlayClipAtPoint(audio, transform.position, volume);
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
