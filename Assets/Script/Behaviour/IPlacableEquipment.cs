namespace Assets.Script.Behaviour
{
		public interface IPlacableEquipment
		{
				Equipment Self { get; }

				bool IsPlaced { get; }
		}
}