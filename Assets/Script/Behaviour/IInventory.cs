using System.Collections.Generic;

namespace Assets.Script.Behaviour
{
		public interface IInventory
		{
				bool IsFull { get; }

				bool CheckCanPutAll(ShopParameters shopInfo, uint quantity);
				bool CheckCanPutAny(ShopParameters shopInfo, uint quantity);
				int Count(ShopParameters item);
				RemainingCount PutAllEquipment(ShopParameters equipment, uint quantity);
				RemainingCount Take(ShopParameters item, int count, out List<(Equipment item, int amount)> taken);
				Equipment TakeRandomItem();
		}
}