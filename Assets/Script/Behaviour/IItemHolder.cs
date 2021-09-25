﻿using Assets.Script.Components;

namespace Assets.Script.Behaviour
{
		public interface IItemHolder
		{
				IPickupItem CurrentItem { get; }
				
				HolderTypeEnum HolderType { get; }

				void Drop(bool dropForPlacing);
				
				void PutIntoInventory();

				void DropThenPut(PlayerBehaviour user, IPickupItem item, bool fromInventory);				
		}
}