using Assets.Script.Components;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public interface IPickupItem : IInteractibleBase
		{
				PlayerBehaviour User { get; }
				
				bool IsTakenByPlayer { get; }

				bool CheckBelongsTo(PlayerBehaviour player);
				
				[CalledByPlayerBehaviour]
				void DropItemRotated(PlayerBehaviour oldOwner, bool noForce = false);
				
				void OnPlayer_NotifyItemTaken(PlayerBehaviour newUser);
				
				void SetParent(Transform holderParent);

				bool CanPickUp(PlayerBehaviour player);
		}
}