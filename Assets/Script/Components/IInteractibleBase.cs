namespace Assets.Script.Components
{
		public interface IInteractibleBase
		{
				EObjectType ObjectType { get; }
				bool IsLocked { get; }
				bool IsUnlocked { get; }
				string GetTargetName();

		}
}