using Assets.Script.Components;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[DisallowMultipleComponent]
		public class InventoryBehaviour : MonoBehaviour, IInventory
		{
				[Header("Slots")]

				[Tooltip("Define a size of slots, so the player can put equipments to it.")]
				[SerializeField] private List<InventorySlot> slots;

				public bool IsFull => slots.All(slot => slot.IsFull);

				public RemainingCount PutAllEquipment(ShopParameters equipment, uint quantity)
				{
						List<InventorySlot> slotsResult = FindSuitableSlots(equipment, quantity);
						if (slotsResult.Any() is false)
						{
								Debug.LogWarning("No inventory slot free for adding");
								return (int)quantity;
						}

						int remaining = (int)quantity;
						foreach (InventorySlot slot in slotsResult)
						{
								int sizeBefore = slot.UsedSpace;
								if (slot.Put(equipment, (uint)remaining))
								{
										remaining += slot.UsedSpace - sizeBefore;
										if (remaining <= 0)
												break;
								}
						}
						return Mathf.Max(0, remaining);
				}

				private List<InventorySlot> FindSuitableSlots(ShopParameters shopInfo, uint quantity)
				{
						return slots.Where(slot => slot.CheckFitSize((uint)shopInfo.SlotSize, quantity)).ToList();
				}

				public int Count(ShopParameters item) => slots.Sum(slot => slot.CountItem(item.ObjectType));

				/// <summary>
				/// Takes only 1 equipment from a inventory. Not matter what equipment.
				/// </summary>
				public Equipment TakeRandomItem()
				{
						List<InventorySlot> randomSlots = slots.Where(slot => slot.IsEmpty is false).ToList();
						if (randomSlots.Any() is false)
								return null;

						InventorySlot nonEmptySlot = randomSlots[Random.Range(0, randomSlots.Count)];
						EObjectType[] randomTypes = nonEmptySlot.GetObjectsTypes();
						EObjectType randomType = randomTypes[Random.Range(0, randomTypes.Length)];
						var prefab = nonEmptySlot.TakePrefab(randomType, 1);
						return prefab.HasValue ? prefab.Value.prefab : null;
				}

				public bool CheckCanPutAll(ShopParameters shopInfo, uint quantity)
				{
						return FindSuitableSlots(shopInfo, quantity).All(slot => slot.CheckFitSize((uint)shopInfo.SlotSize, quantity));
				}

				public bool CheckCanPutAny(ShopParameters shopInfo, uint quantity)
				{
						return FindSuitableSlots(shopInfo, quantity).Any();
				}

				public RemainingCount Take(ShopParameters item, int count, out List<(Equipment item, int amount)> taken)
				{
						taken = new List<(Equipment item, int amount)>();
						if (count <= 0)
						{
								return 0;
						}

						foreach (var slot in slots.Where(slot => slot.Contains(item)))
						{
								List<(Equipment item, int amount)> prefabs = slot.TakeAmount((uint)count);
								taken.AddRange(prefabs);
								count -= prefabs.Sum(val => val.amount);

								if (count <= 0)
										break;
						}
						return count;
				}
		}
}