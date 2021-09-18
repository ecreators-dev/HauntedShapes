namespace Assets.Script.Behaviour
{
		public interface IPowerbankSupport
		{
				bool IsBroken { get; }
				
				int MaxPower { get; }
				
				int Power { get; }

				bool LoadPower(int power);
				
				void VisualizePowerLoadUpdate(float actualPowerDelta, int maxPowerDelta);
		}
}