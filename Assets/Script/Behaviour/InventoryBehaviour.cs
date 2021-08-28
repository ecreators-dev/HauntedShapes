using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class InventoryBehaviour : MonoBehaviour
		{
				[Header("Slots")] 
				
				[Tooltip("Define a size of slots, so the player can put equipments to it.")]
				[SerializeField] private List<InventorySlot> slots;

				public bool IsFull => slots.All(slot => slot.IsFull);

				public RemainingCount PutAllEquipment(ShopParameters equipment, uint quantity)
				{
						(int remaining, List<InventorySlot> slotsTake) slotsResult = FindUsableSlotToAdd(equipment, quantity);
						if (slotsResult.slotsTake.Any() is false)
						{
								Debug.LogWarning("No inventory slot free for adding");
								return (int)quantity;
						}

						if (slotsResult.remaining > 0)
						{
								Debug.LogError("Not every item will fit in this inventory. Remaining: " + slotsResult.remaining);
						}

						int remaining = (int)quantity;
						foreach (var slot in slotsResult.slotsTake)
						{
								uint insert = (uint)(quantity % slot.Maximum);
								slot.Put(equipment, insert);
								remaining -= (int)insert;
								if (remaining <= 0)
										break;
						}

						return slotsResult.remaining;
				}

				private (int remaining, List<InventorySlot> slots) FindUsableSlotToAdd(ShopParameters shopInfo, uint quantity)
				{
						int remaining = (int)quantity;
						List<InventorySlot> result = new List<InventorySlot>();
						foreach (InventorySlot slot in slots)
						{
								if (slot.IsFull) continue;

								if (slot.Contains(shopInfo))
								{

								}

								if (slot.RemainingSpace <= remaining)
								{
										int insert = slot.RemainingSpace;
										remaining -= (int)insert;
										result.Add(slot);
								}

								if (remaining <= 0)
										break;
						}
						return (remaining, result);
				}

				public int Count(ShopParameters item) => slots.Sum(slot => slot.CountItem(item));

				/// <summary>
				/// Takes only 1 equipment from a inventory. Not matter what equipment.
				/// </summary>
				public Equipment GetRandomEquipment()
				{
						List<InventorySlot> randomSlots = slots.Where(slot => slot.IsEmpty is false).ToList();
						if (randomSlots.Any() is false)
								return null;

						InventorySlot slot = randomSlots[UnityEngine.Random.Range(0, randomSlots.Count)];
						return slot.TakePrefab(1);
				}

				public bool CheckCanPutAll(ShopParameters shopInfo, uint quantity)
				{
						return FindUsableSlotToAdd(shopInfo, quantity).remaining == 0;
				}

				public bool CheckCanPutAny(ShopParameters shopInfo, uint quantity)
				{
						return FindUsableSlotToAdd(shopInfo, quantity).slots.Any();
				}

				public void Take(ShopParameters itemInInventory, int count)
				{
						if (count <= 0)
								return;

						foreach (var slot in this.slots.Where(slot => slot.Contains(itemInInventory)))
						{
								if (count <= 0)
										break;

								int countIn = slot.CountItem(itemInInventory);
								if (countIn < count)
								{
										(int amount, Equipment prefab) = slot.TakeAll();
										count -= amount;
								}
								else
								{
										slot.TakePrefab((uint)count);
										count = 0;
								}
						};
				}
		}
}