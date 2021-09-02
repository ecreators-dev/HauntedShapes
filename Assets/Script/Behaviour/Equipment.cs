﻿using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Extends Items: you can put it into your inventory and take it off from it
		/// </summary>
		public abstract class Equipment : PickupItem
		{
				public bool IsBroken { get; private set; }

				public bool IsPowered { get; private set; }

				public ShopParameters ShopInfo { get; private set; }

				public PlayerBehaviour Owner { get; private set; }

				public bool TryGetCrosshairHitInfo(out Vector3 position, out Vector3 normal, out PlacementEnum type)
				{
						bool result = CrosshairHit.GetPlacementInfo(out var info) != PlacementEnum.NONE;
						position = result ? info.Value.Raycast.point : default;
						normal = result ? info.Value.Raycast.normal : default;
						type = result ? info.Value.PlacementType : PlacementEnum.NONE;
						return result;
				}

				protected void SetPowered(bool active)
				{
						this.IsPowered = active;
				}

				protected void TogglePowered()
				{
						this.IsPowered = !this.IsPowered;
				}

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
								OnInventory();
						}
				}

				/// <summary>
				/// AFTER the player equipped this item
				/// </summary>
				protected virtual void OnEquip() { }

				/// <summary>
				/// AFTER the player put this item into his owner inventory
				/// </summary>
				protected virtual void OnInventory() { }

				protected virtual void SetBroken()
				{
						IsBroken = true;
						if (User is { })
						{
								User.OnEquipment_Broken(this);
						}
				}

				protected void SetRepaired()
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
				protected virtual void OnOwnerOwnedEquipment() { }

				public void SetShopInfo(ShopParameters shopInfo, Equipment prefab)
				{
						this.ShopInfo = shopInfo;
						shopInfo.SetPrefab(prefab);
				}

#if UNITY_EDITOR
				[ContextMenu("Toggle ON (EDIT-MODE only )")]
				public void EditorToggleOn()
				{
						OnEditMode_ToggleOn();
				}


				[ContextMenu("Toggle OFF (EDIT-MODE only )")]
				public void EditorToggleOff()
				{
						OnEditMode_ToggleOff();
				}
#endif

				protected virtual void OnEditMode_ToggleOn() { }

				protected virtual void OnEditMode_ToggleOff() { }

				public override string GetTargetName()
				{
						return ShopInfo.DisplayName;
				}

				public abstract EquipmentInfo GetEquipmentInfo();
		}
}