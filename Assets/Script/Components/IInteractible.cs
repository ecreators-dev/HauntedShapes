using Assets.Script.Behaviour;

namespace Assets.Script.Components
{
		public interface IInteractible : IInteractibleBase
		{
				/// <summary>
				/// Interacts means power or action. It does not mean pickup as action, but can pickup (regarding lock)
				/// </summary>
				bool CanInteract(PlayerBehaviour sender);

				bool RunInteraction(PlayerBehaviour sender);
		}
}