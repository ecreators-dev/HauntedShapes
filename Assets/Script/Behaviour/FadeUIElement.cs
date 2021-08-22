using Assets.Script.Model;

using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(CanvasGroup))]
		public class FadeUIElement : FadingMonoBehaviour, IAlphaFader
		{
				private CanvasGroup canvasGroup;
				[SerializeField]
				[Range(0, 5)]
				private float fadeInSeconds = 1;

				[SerializeField]
				[Range(0, 5)]
				private float fadeOutSeconds = 1;

				private void Awake()
				{
						canvasGroup = GetComponent<CanvasGroup>();
				}

				public Coroutine RunFade(CanvasGroup canvasFade, Fade config, Action<ProgressValue> updatedValue = null, Action complete = null)
				{
						return Fade(() => canvasFade.alpha, alpha => canvasFade.alpha = alpha, config, updatedValue, complete);
				}

				public void FadeIn(Action complete = null)
				{
						RunFade(canvasGroup, new Fade(fadeInSeconds, 1), OnFadeUpdate, complete);
				}

				public void FadeOut(Action complete = null)
				{
						RunFade(canvasGroup, new Fade(fadeOutSeconds, 0), OnFadeUpdate, complete);
				}

				public void SetAlpha(ProgressValue progress)
				{
						canvasGroup.alpha = progress.ValueBetweenZeroAndOne;
						Debug.Log($"\"{gameObject.name}\" (name) canvasgroup alpha updated to {canvasGroup.alpha}");
				}

				private void OnFadeUpdate(ProgressValue progress)
				{

				}
		}
}
