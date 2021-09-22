
using System;

using UnityEngine;

namespace Assets.Script.Components
{
		[Serializable]
		public class SoundEffect
		{
				public AudioClip[] randomSounds;
				[Range(0, 1)]
				public float volume = 1;
				public bool loop = false;

				public bool IsLooping { get; private set; }
				private AudioSource LoopingAudio { get; set; }

				public void StopLoop()
				{
						if (IsLooping)
						{
								LoopingAudio.Stop();
								LoopingAudio.clip = null;
								LoopingAudio.loop = false;
								LoopingAudio.volume = 1;
								LoopingAudio = null;
						}
				}

				public void PlayRandomLoop(string ownerName, string effectName, AudioSource soundPlayer3d)
				{
						if (randomSounds is { } && randomSounds.Length > 0)
						{
								if (IsLooping) return;

								IsLooping = true;
								LoopingAudio = soundPlayer3d;

								AudioClip clip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)];
								soundPlayer3d.clip = clip;
								soundPlayer3d.volume = volume;
								soundPlayer3d.loop = true;
						}
						else
						{
								Debug.LogWarning($"{ownerName}: no {effectName}-sounds!");
						}
				}

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
}
