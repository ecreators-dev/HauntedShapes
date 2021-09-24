namespace Assets.Script.Behaviour
{
		public interface IPlacableEquipment : IEquipment
		{
				IEquipment Self { get; }

				bool IsPlaced { get; }
				bool IsUnusedOnFloor { get; }
		}
}