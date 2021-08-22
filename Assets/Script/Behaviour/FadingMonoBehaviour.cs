using Assets.Script.Model;

using System;
using System.Collections;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public abstract class FadingMonoBehaviour : MonoBehaviour
		{
				protected Coroutine Fade(Func<float> get, Action<float> set, Fade config, Action<ProgressValue> updatedValue = null, Action complete = null)
				{
						return StartCoroutine(Fading());

						IEnumerator Fading()
						{
								float timeout = config.SecondsToFade;
								WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
								yield return waitForEndOfFrame;
								float value = get();
								while (timeout > 0)
								{
										value = Mathf.Lerp(value, config.FadeToValue, Time.deltaTime / config.SecondsToFade);
										set(value);
										updatedValue?.Invoke(new ProgressValue((config.SecondsToFade - timeout) / config.SecondsToFade));
										timeout -= Time.deltaTime;
										yield return waitForEndOfFrame;
								}

								value = config.FadeToValue;
								set(value);
								updatedValue?.Invoke(new ProgressValue(1));
								complete?.Invoke();
								yield break;
						}
				}
		}
}
