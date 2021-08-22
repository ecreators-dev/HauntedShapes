
using System;

namespace Assets.Script.Model
{
		public interface IAlphaFader
		{
				void FadeIn(Action complete = null);

				void FadeOut(Action complete = null);

				void SetAlpha(ProgressValue progress);
		}
}
