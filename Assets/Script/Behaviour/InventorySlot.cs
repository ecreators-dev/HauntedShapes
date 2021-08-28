using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public sealed class InventorySlot
		{
				[SerializeField] ShopParameters shopItemInfo;
				[Min(0)]
				[SerializeField] int quantity;
				[Min(1)]
				[SerializeField] int maximumQuantity;

				public int Quanity => quantity;

				public int Maximum => maximumQuantity;

				public bool IsEmpty => quantity is 0;

				public bool IsFull => quantity == maximumQuantity;

				public int RemainingSpace => maximumQuantity - quantity;

				public (int amount, Equipment prefab) TakeAll()
				{
						(int quantity, Equipment prefab) result = (quantity, shopItemInfo.Prefab);
						shopItemInfo = null;
						quantity = 0;
						return result;
				}

				public bool Contains(ShopParameters other)
				{
						return shopItemInfo is { } && shopItemInfo == other;
				}

				public Equipment TakePrefab(uint take)
				{
						if (shopItemInfo == null)
								return null;

						if (CheckQuantityToTake(take))
						{
								quantity -= (int)take;

								// reset
								if (quantity == 0)
								{
										shopItemInfo = null;
								}

								return shopItemInfo.Prefab;
						}

						return null;
				}

				public void Put(ShopParameters shopItemInfo, uint quantity = 1)
				{
						if (shopItemInfo == null)
						{
								throw new ArgumentNullException(nameof(shopItemInfo));
						}

						if (CheckPrefab(shopItemInfo))
						{
								this.shopItemInfo = shopItemInfo;

								if (CheckQuantityToAdd(quantity))
								{
										this.quantity += (int)quantity;
								}
								else
								{
										Debug.LogWarning($"Not enough space to put this quantity in this slot! {quantity}");
								}
						}
						else
						{
								Debug.LogError("You cannot add a different equipment. You must clear this slot first!");
						}
				}

				public bool CheckQuantityToTake(uint take)
				{
						return this.quantity >= take;
				}

				public bool CheckQuantityToAdd(uint quantity)
				{
						return this.quantity + quantity <= maximumQuantity;
				}

				public int CountItem(ShopParameters item) => item.IsEqualTo(shopItemInfo) ? quantity : 0;

				private bool CheckPrefab(ShopParameters shopItemInfo) => shopItemInfo is { } && shopItemInfo.IsEqualTo(this.shopItemInfo);
		}
}