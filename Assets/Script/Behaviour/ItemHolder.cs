using Assets.Script.Components;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

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
						if (inputControls.DropEquipmentButton)
						{
								if (CurrentItem is IEquipment)
								{
										Drop(false);
								}
						}
						else if (inputControls.InteractWithEquipmentButton)
						{
								if (CurrentItem is IInteractible item)
								{
										// to toggle head equipment you must also press north button
										if (HolderType == HolderTypeEnum.HEAD)
										{
												if (this.InputControls().InteractWithEquipmentUpButton)
												{
														Debug.Log("Toggle Head Equipment");
														item.RunInteraction(playerOwner);
												}
										}
										else if (HolderType == HolderTypeEnum.RIGHT_HAND)
										{
												if (!this.InputControls().InteractWithEquipmentUpButton)
												{
														Debug.Log("Toggle Right Hand Equipment");
														item.RunInteraction(playerOwner);
												}
										}
										else
										{
												// left Hand is not allowed to interact!
										}
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
								Drop(false);
								return;
						}

						if (!CanPutIntoInventory)
						{
								Debug.Log($"Inventory is full! Drop instead: {equipment.GetTargetName()}");
								Drop(false);
								return;
						}

						Inventory.PutAllEquipment(equipment.ShopInfo, 1);
						equipment.OnPlayer_PutIntoInventory(equipment.User);
						equipment.Destroy();
						CurrentItem = null;
				}

				public void Drop(bool dropForPlacing)
				{
						CurrentItem?.DropItemRotated(CurrentItem.User, dropForPlacing, dropForPlacing);
						CurrentItem = null;
				}
		}
}