namespace Assets.Script.Behaviour
{
		public struct Filling
		{
				private Filling(float value)
				{
						Value = value;
				}

				private float Value { get; }

				public static implicit operator float(Filling fill) => fill.Value;
				public static implicit operator Filling(float fill) => new Filling(fill);
		}
}