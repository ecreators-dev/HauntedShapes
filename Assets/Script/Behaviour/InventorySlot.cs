using Assets.Script.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public sealed class InventorySlot
		{
				[Tooltip("Summe von Raum, in diesem Slot. Beispiel: 1 x Baseball passt in Größe 1. 2 x Baseball past nicht in Größe von 1 und 1 x Baseballschläger benötigt 3, passt auch nicht.")]
				[SerializeField] private int capacitySize = 1 * 1;

				private readonly Dictionary<EObjectType, (ShopParameters info, int quantity)> items = new Dictionary<EObjectType, (ShopParameters info, int quantity)>();

				public bool IsEmpty => items.Any() is false || items.Values.Sum(val => val.quantity) == 0;

				public bool IsFull => RemainingSpace == 0;

				public int UsedSpace => items.Sum(kvp => kvp.Value.info.SlotSize * kvp.Value.quantity);

				public int RemainingSpace => capacitySize - UsedSpace;

				public int TotalSpace { get => capacitySize; }

				/// <summary>
				/// Removes all Items from this slot and returns them with capacity.
				/// </summary>
				public List<(Equipment item, int capacity)> TakeAll()
				{
						var result = items.Select(kvp => (kvp.Value.info.Prefab, kvp.Value.quantity)).ToList();
						items.Clear();
						return result;
				}

				public bool Contains(ShopParameters other)
				{
						return items.Any(kvp => kvp.Value.info.ObjectType == other.ObjectType);
				}

				public (Equipment prefab, int amount)? TakePrefab(EObjectType type, uint take)
				{
						return TakePrefab(items[type].info, take);
				}

				public (Equipment prefab, int amount)? TakePrefab(ShopParameters type, uint take)
				{
						(ShopParameters info, int quantity) value;
						if (take > 0 && TryGetValue(type, out value))
						{
								if (value.quantity >= take)
								{
										ChangeQuantity(type, -(int)take);
										return (value.info.Prefab, (int)take);
								}
								else
								{
										UpdateOrSet(type, 0);
										return (value.info.Prefab, value.quantity);
								}
						}
						return null;
				}

				private void UpdateOrSet(ShopParameters type, int newQuantity)
				{
						items[type.ObjectType] = (type, newQuantity);
						if (newQuantity == 0)
						{
								items.Remove(type.ObjectType);
						}
				}

				public EObjectType[] GetObjectsTypes()
				{
						return items.Keys.ToArray();
				}

				private void ChangeQuantity(ShopParameters type, int change)
				{
						if (TryGetValue(type, out var value))
						{
								UpdateOrSet(type, value.quantity + change);
						}
				}

				private bool TryGetValue(ShopParameters type, out (ShopParameters info, int quantity) value)
				{
						return items.TryGetValue(type.ObjectType, out value);
				}

				public bool Put(ShopParameters info, uint quantity = 1)
				{
						if (info == null)
						{
								throw new ArgumentNullException(nameof(info));
						}

						if (quantity == 0)
								return false;

						if (CheckQuantityToAdd(quantity) && CheckFitSize((uint)info.SlotSize, quantity))
						{
								ChangeQuantity(info, (int)quantity);
								return true;
						}
						else
						{
								Debug.LogWarning($"Not enough space to put this quantity in this slot! {quantity}");
						}
						return false;
				}

				public List<(Equipment item, int amount)> TakeAmount(uint count)
				{
						var result = new List<(Equipment item, int amount)>();
						while (!IsEmpty && count > 0)
						{
								foreach ((ShopParameters info, int quantity) in items.Values)
								{
										if (quantity > 0 && quantity <= count)
										{
												int amount = Mathf.Min(quantity, (int)count);
												ChangeQuantity(info, -amount);
												count -= (uint)amount;
												result.Add((info.Prefab, amount));
												break;
										}
								}
						}
						return result;
				}

				public bool CheckFitSize(uint itemSize, uint amount)
				{
						return RemainingSpace >= itemSize * amount;
				}

				public bool CheckQuantityToTake(ShopParameters info, uint take)
				{
						return TryGetValue(info, out var value) && value.quantity >= take;
				}

				public bool CheckQuantityToAdd(uint quantity)
				{
						return RemainingSpace <= quantity;
				}

				public int CountItem(EObjectType item)
				{
						return items.TryGetValue(item, out var value) ? value.quantity : 0;
				}
		}
}