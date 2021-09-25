using Assets.Script.Components;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public interface IPickupItem : IInteractibleBase
		{
				PlayerBehaviour User { get; }

				Vector3 NormalUp { get; }

				bool IsTakenByPlayer { get; }

				bool CheckBelongsTo(PlayerBehaviour player);
				
				[CalledByPlayerBehaviour]
				void DropItemRotated(PlayerBehaviour oldOwner, bool noForce = false, bool dropForPlacing = false);
				
				void TriggerPlayerPickup(PlayerBehaviour newUser);
				
				void SetParent(Transform holderParent);
				
				/// <summary>
				/// Checks if the object is unlocked and not taken (basic logic) + extra logic
				/// </summary>
				bool CheckPlayerCanPickUp(PlayerBehaviour player);
		}
}