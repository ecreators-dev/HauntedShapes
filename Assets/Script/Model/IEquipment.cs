
using UnityEngine;

namespace Assets.Script.Behaviour
{
		public interface IEquipment : IPickupItem
		{
				bool IsBroken { get; }
				
				bool IsPowered { get; }
				
				ShopParameters ShopInfo { get; }
				
				PlayerBehaviour Owner { get; }

				EquipmentInfo GetEquipmentInfo();
				
				[CalledByPlayerBehaviour]
				void OnPlayer_EquipmentBought(PlayerBehaviour owner);
				
				[CalledByPlayerBehaviour]
				void OnPlayer_EquipmentSold(PlayerBehaviour owner);
				
				[CalledByPlayerBehaviour]
				void OnPlayer_PutIntoInventory(PlayerBehaviour owner);
				
				void Destroy();
		}
}