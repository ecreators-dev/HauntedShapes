using Assets.Script.Behaviour;

namespace Assets.Script.Components
{
		public interface IInteractible
		{
				EObjectType ObjectType { get; }
				bool IsLocked { get; }
				bool IsUnlocked { get; }

				bool CanInteract(PlayerBehaviour sender);
				string GetTargetName();
				void Interact(PlayerBehaviour sender);
		}
}