using System;

using UnityEngine;

namespace Assets.Script.Controller
{
		[Serializable]
		public sealed class AudioPlayInfo
		{
				private string name;

				[SerializeField] private AudioSource audioSource;
				[SerializeField] private bool playback = true;
				[ReadOnlyDependingOnBoolean(nameof(playback), true)]
				[Range(0, 1)]
				[SerializeField] private float volume = 1;

				public AudioPlayInfo(string nameOfMusicType)
				{
						this.name = nameOfMusicType;
				}

				public string Name => name;
				public AudioSource AudioSource => audioSource;
				public bool CanPlay => playback;
				public float Volume => volume;

				public void VerifyClipOrThrow()
				{
						if (AudioSource == null || AudioSource.clip == null)
								throw new Exception($"Missing clip in {Name}! (null)");
				}
		}
}