namespace Assets.Script.Behaviour
{
		public interface ILightSource
		{
				bool IsPowered { get; }

				float ActiveMultiplier { get; }
		}
}