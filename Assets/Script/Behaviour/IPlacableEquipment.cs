namespace Assets.Script.Behaviour
{
		public interface IPlacableEquipment : IEquipment
		{
				bool IsPlaced { get; }

				bool IsUnusedOnFloor { get; }

				void PlaceAtPositionAndNormal(HitInfo clickRange);
		}
}