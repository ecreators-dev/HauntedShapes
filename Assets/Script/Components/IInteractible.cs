using Assets.Script.Behaviour;

namespace Assets.Script.Components
{
		public interface IInteractible : IInteractibleBase
		{
				bool CanInteract(PlayerBehaviour sender);
				bool RunInteraction(PlayerBehaviour sender);
		}

		public interface IInteractibleBase
		{
				EObjectType ObjectType { get; }
				bool IsLocked { get; }
				bool IsUnlocked { get; }
				string GetTargetName();

		}
}