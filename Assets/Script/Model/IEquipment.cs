using Assets.Script.Components;

namespace Assets.Script.Behaviour
{
		public interface IEquipment : IInteractible, IPickupItem
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