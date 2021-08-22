namespace Assets.Script.Behaviour
{
		public interface IFlashlight
		{
				bool NextPowerStatus { get; }

				void TryPowerOn();

				void TryPowerOff();
		}
}