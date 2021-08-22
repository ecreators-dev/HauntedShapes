namespace Assets.Script.Model
{
		public struct ProgressValue
		{
				public ProgressValue(float progressValue)
				{
						ValueBetweenZeroAndOne = progressValue;
				}

				public float ValueBetweenZeroAndOne { get; }

				public ProgressValue Invert()
				{
						return new ProgressValue(1f - ValueBetweenZeroAndOne);
				}
		}
}