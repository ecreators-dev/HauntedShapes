
using System;

using UnityEngine;

namespace Assets.Script.Model
{
		public interface IAudioFader
		{
				float MaxVolume { get; set; }

				Coroutine FadeIn(AudioSource audio, Action<ProgressValue> updatedValue = null);

				Coroutine FadeOut(AudioSource audio, Action<ProgressValue> updatedValue = null);
		}
}