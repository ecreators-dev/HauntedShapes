using System;

namespace Assets.Script.Model
{
		[Serializable]
		public struct Fade
		{
				public Fade(float secondsToFade, float fadeTo) : this()
				{
						SecondsToFade = secondsToFade;
						FadeToValue = fadeTo;
				}

				public float SecondsToFade { get; private set; }

				public float FadeToValue { get; private set; }
		}
}
