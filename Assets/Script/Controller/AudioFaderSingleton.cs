using Assets.Script.Behaviour;
using Assets.Script.Model;

using System;

using UnityEngine;

namespace Assets.Script.Controller
{
		public class AudioFaderSingleton : FadingMonoBehaviour, IAudioFader
		{
				[SerializeField]
				[Range(0, 5)]
				private float fadeInSeconds = 0.3f;

				[SerializeField]
				[Range(0, 5)]
				private float fadeOutSeconds = 0.4f;

				[SerializeField]
				[Range(0, 1)]
				private float maxVolume = 1;

				public static IAudioFader Instance { get; private set; }

				public float MaxVolume { get => maxVolume; set => maxVolume = Mathf.Max(0, Mathf.Min(1, value)); }

				public Coroutine FadeIn(AudioSource audio, Action<ProgressValue> updatedValue = null)
				{
						return Fade(() => audio.volume, volume => audio.volume = volume, new Fade(fadeInSeconds, maxVolume), updatedValue);
				}

				public Coroutine FadeOut(AudioSource audio, Action<ProgressValue> updatedValue = null)
				{
						return Fade(() => audio.volume, volume => audio.volume = volume, new Fade(fadeOutSeconds, 0), updatedValue);
				}

				private void Awake()
				{
						if (Instance is null)
						{
								Instance = this;
						}
						else
						{
								Destroy(this);
								Debug.LogError($"Doppelter {nameof(AudioFaderSingleton)}. Wird gelöscht.");
						}
				}
		}
}