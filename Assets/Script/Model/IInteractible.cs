using Assets.Script.Behaviour;

namespace Assets.Script.Model
{
		public interface IInteractible : IMonoBehaviour
		{
				/// <summary>
				/// Defines if this item can be pick up by player
				/// </summary>
				bool IsPickable { get; }

				void TouchClickUpdate();

				void TouchOverUpdate();

				void Drop();

				void OnPickup(PlayerBehaviour player);
		}
}