
using System;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Script.Components
{
		[Serializable]
		public class SoundEffect
		{
				public AudioClip[] randomSounds;
				[Range(0, 1)]
				public float volume = 1;
				public bool playedFromScript = false;
				public bool loop = false;
				[Min(0)]
				public float delaySeconds = 0;

				public bool IsLooping { get; private set; }

				private AudioSource LoopingAudio { get; set; }

				public float length => randomSounds.Max(clip => clip.length);

				public void StopLoop()
				{
						if (IsLooping)
						{
								LoopingAudio.Stop();
								LoopingAudio = null;
						}
				}


				public void PlayRandomLoop(string ownerName, string effectName, AudioSource soundPlayer3d)
				{
						if (soundPlayer3d == null)
								return;

						if (randomSounds is { } && randomSounds.Length > 0)
						{
								if (IsLooping) return;

								AudioClip clip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)];
								PlayClipLoop(soundPlayer3d, clip);
						}
						else if (soundPlayer3d.clip is { })
						{
								if (IsLooping) return;

								PlayClipLoop(soundPlayer3d, soundPlayer3d.clip);
						}
				}

				private void PlayClipLoop(AudioSource soundPlayer3d, AudioClip clip)
				{
						IsLooping = true;
						LoopingAudio = soundPlayer3d;
						soundPlayer3d.clip = clip;
						soundPlayer3d.volume = volume;
						soundPlayer3d.loop = true;
						PlayDelayedAsync(soundPlayer3d);
				}

				private async void PlayDelayedAsync(AudioSource soundPlayer3d)
				{
						Debug.Log($"{nameof(SoundEffect)}: plays loop sound in delay of {delaySeconds} s");
						await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
						soundPlayer3d.Play();
						Debug.Log($"{nameof(SoundEffect)}: played loop sound after delay of {delaySeconds} s");
				}

				private async void PlayOneShotDelayedAsync(AudioSource soundPlayer3d, AudioClip clip, Vector3? fireAndForget)
				{
						Debug.Log($"{nameof(SoundEffect)}: plays {clip.name} in delay of {delaySeconds} s");
						await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
						if (fireAndForget.HasValue)
						{
								GameObject temp = new GameObject("fire and forget");

								CopyPositionAndRotation(soundPlayer3d, temp);

								AudioSource audioSource = CopyAudioSource3D(soundPlayer3d, temp);
								audioSource.PlayOneShot(clip, volume);

								await Task.Delay(TimeSpan.FromSeconds(clip.length));
								UnityEngine.Object.Destroy(temp);
						}
						else
						{
								soundPlayer3d.PlayOneShot(clip, volume);
						}

						Debug.Log($"{nameof(SoundEffect)}: played {clip.name} after delay of {delaySeconds} s");
				}

				private static void CopyPositionAndRotation(AudioSource soundPlayer3d, GameObject temp)
				{
						temp.transform.position = soundPlayer3d.transform.position;
						temp.transform.rotation = soundPlayer3d.transform.rotation;
				}

				private static AudioSource CopyAudioSource3D(AudioSource soundPlayer3d, GameObject temp)
				{
						AudioSource audioSource = temp.AddComponent<AudioSource>();
						audioSource.spatialBlend = soundPlayer3d.spatialBlend;
						audioSource.rolloffMode = soundPlayer3d.rolloffMode;
						audioSource.minDistance = soundPlayer3d.minDistance;
						audioSource.maxDistance = soundPlayer3d.maxDistance;
						return audioSource;
				}

				public void PlayRandomOnce(string ownerName, string effectName, AudioSource soundPlayer3d, Vector3? fireAndForget = null)
				{
						if (randomSounds is { } && randomSounds.Length > 0)
						{
								AudioClip clip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)];
								PlayOneShotDelayedAsync(soundPlayer3d, clip, fireAndForget);
								Debug.Log($"{ownerName}: plays {effectName}-sounds!");
						}
						else
						{
								Debug.LogWarning($"{ownerName}: no {effectName}-sounds!");
						}
				}
		}
}
