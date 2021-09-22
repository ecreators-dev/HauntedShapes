
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class ItemHolder : MonoBehaviour, IItemHolder
		{
				[SerializeField] private HolderTypeEnum type = HolderTypeEnum.RIGHT_HAND;
				[SerializeField] private InventoryBehaviour inventory;
				[SerializeField] private Transform holderParent;

				public IPickupItem CurrentItem { get; private set; }
				private IInventory Inventory { get; set; }
				private bool HasInventory => Inventory != null;
				private bool CanPutIntoInventory => CurrentItem is IEquipment e && Inventory.CheckCanPutAny(e.ShopInfo, 1);

				public HolderTypeEnum Type => type;

				private void Start()
				{
						Inventory = inventory;
				}

				public void Put(PlayerBehaviour user, IPickupItem item, bool fromInventory)
				{
						if (CurrentItem != null)
						{
								PutIntoInventory();
						}

						if (item != null)
						{
								if (item is IEquipment equipment)
								{
										if (fromInventory && HasInventory && Inventory.Count(equipment.ShopInfo) > 0)
										{
												inventory.Take(equipment.ShopInfo, 1, out List<(Equipment item, int amount)> taken);
										}
								}
								CurrentItem = item;
								item.SetParent(holderParent);
								item.OnPlayer_NotifyItemTaken(user);
						}
				}


				public void PutIntoInventory()
				{
						if (!HasInventory) return;

						if (!(CurrentItem is IEquipment equipment))
						{
								Debug.LogWarning($"{CurrentItem.GetTargetName()}: No equipment type. Cannot put into inventory.");
								Drop();
								return;
						}

						if (!CanPutIntoInventory)
						{
								Debug.Log($"Inventory is full! Drop instead: {equipment.GetTargetName()}");
								Drop();
								return;
						}

						Inventory.PutAllEquipment(equipment.ShopInfo, 1);
						equipment.OnPlayer_PutIntoInventory(CurrentItem.User);
						equipment.Destroy();
						CurrentItem = null;
				}

				public void Drop()
				{
						if (CurrentItem == null) return;

						CurrentItem.DropItemRotated(CurrentItem.User);
						CurrentItem = null;
				}
		}
}