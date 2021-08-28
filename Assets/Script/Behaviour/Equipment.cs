﻿using UnityEditor;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Extends Items: you can put it into your inventory and take it off from it
		/// </summary>
		public abstract class Equipment : PickupItem
		{
				public bool IsBroken { get; private set; }

				public ShopParameters ShopInfo { get; private set; }

				public PlayerBehaviour Owner { get; private set; }

				[CalledByPlayerBehaviour]
				public virtual void OnPlayer_EquippedToHand(PlayerBehaviour owner)
				{
						if (User is { } && User == owner)
						{
								OnEquip();
						}
				}

				[CalledByPlayerBehaviour]
				public virtual void OnPlayer_PutIntoInventory(PlayerBehaviour owner)
				{
						if (User is { } && User == owner)
						{
								OnInvectory();
						}
				}

				protected abstract void OnEquip();

				protected abstract void OnInvectory();

				protected virtual void SetBroken()
				{
						IsBroken = true;
						if (User is { })
						{
								User.OnEquipment_Broken(this);
						}
				}

				protected void SetFixed()
				{
						IsBroken = false;
						if (User is { })
						{
								User.OnEquipment_Fixed(this);
						}
				}

				/// <summary>
				/// Call this after the player sold for this equipment
				/// </summary>
				[CalledByPlayerBehaviour]
				public void OnPlayer_EquipmentBought(PlayerBehaviour owner)
				{
						if (Owner == null)
						{
								Owner = owner;
								OnOwnerOwnedEquipment();
						}
				}

				/// <summary>
				/// Call this after the player sell this equipment
				/// </summary>
				[CalledByPlayerBehaviour]
				public void OnPlayer_EquipmentSold(PlayerBehaviour owner)
				{
						if (Owner is { } && owner == Owner)
						{
								Owner = null;
						}
				}

				/// <summary>
				/// AFTER the player paid for this equipment, the owner changed to [not null]
				/// </summary>
				protected abstract void OnOwnerOwnedEquipment();

				public void SetShopInfo(ShopParameters shopInfo, Equipment prefab)
				{
						this.ShopInfo = shopInfo;
						shopInfo.SetPrefab(prefab);

						string assetPath = AssetDatabase.GetAssetPath(shopInfo.GetInstanceID());
						string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
				}
		}
}