namespace Assets.Script.Behaviour
{
		public interface ILightSource
		{
				bool IsActive { get; }

				float ActiveMultiplier { get; }
		}
}