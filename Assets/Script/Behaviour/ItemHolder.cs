using Assets.Script.Components;

using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class ItemHolder : MonoBehaviour, IItemHolder
		{
				[SerializeField] private HolderTypeEnum type = HolderTypeEnum.RIGHT_HAND;
				[SerializeField] private InventoryBehaviour inventory;
				[SerializeField] private Transform holderParent;
				[SerializeField] private PlayerBehaviour playerOwner;

				public HolderTypeEnum HolderType => type;

				public IPickupItem CurrentItem { get; private set; }

				private IInventory Inventory { get; set; }
				private bool HasInventory => Inventory != null;
				private bool CanPutIntoInventory => CurrentItem is IEquipment e && Inventory.CheckCanPutAny(e.ShopInfo, 1);

				private void Awake()
				{
						Inventory = inventory;
				}

				private void Update()
				{
						IInputControls inputControls = this.InputControls();
						if (inputControls.DropEquipmentButtonPressed)
						{
								if (CurrentItem is IEquipment)
								{
										Drop();
								}
						}
						else if (inputControls.InteractButtonPressed)
						{
								if (CurrentItem is IInteractible item)
								{
										item.RunInteraction(playerOwner);
								}
						}
				}

				public void DropThenPut(PlayerBehaviour user, IPickupItem item, bool fromInventory)
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
								item.TriggerPlayerPickup(user);
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
						equipment.OnPlayer_PutIntoInventory(equipment.User);
						equipment.Destroy();
						CurrentItem = null;
				}

				public void Drop()
				{
						CurrentItem.DropItemRotated(CurrentItem.User);
						CurrentItem = null;
				}
		}
}