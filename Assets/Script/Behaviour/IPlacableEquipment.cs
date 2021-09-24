namespace Assets.Script.Behaviour
{
		public interface IPlacableEquipment : IEquipment
		{
				bool IsPlaced { get; }

				bool IsUnusedOnFloor { get; }

				bool PlaceAtPositionAndNormal(HitInfo clickRange);
		}
}