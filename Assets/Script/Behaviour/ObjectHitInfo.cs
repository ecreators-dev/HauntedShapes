using Assets.Script.Components;

using System;

namespace Assets.Script.Behaviour
{
		public struct ObjectHitInfo
		{
				public ObjectHitInfo(Interactible target)
				{
						TargetItem = target;
						var type = InteractionType.NONE;
						if (TargetItem is Interactible)
						{
								type = InteractionType.INTERACTIBLE;
						}
						if (TargetItem is PickupItem)
						{
								type |= InteractionType.PICKUP;
						}
						if (TargetItem is Equipment)
						{
								type |= InteractionType.EQUIPMENT;
						}
						if (TargetItem is EquipmentPlacable)
						{
								type |= InteractionType.PLACEABLE;
						}
						Type = type;
				}

				public InteractionType Type { get; }

				public IInteractibleBase TargetItem { get; }

				public IInteractible GetInteractible() => TargetItem as IInteractible;

				public IPickupItem GetPickupItem() => TargetItem as PickupItem;

				public IEquipment GetEquipment() => TargetItem as Equipment;

				public EquipmentPlacable GetPlaceableItem() => TargetItem as EquipmentPlacable;

				public bool HasTargetItem => Type != InteractionType.NONE;

				public bool IsType(InteractionType type) => Type.HasFlag(type);

				[Flags]
				public enum InteractionType
				{
						NONE = 0,
						INTERACTIBLE = 1,
						PICKUP = 2,
						EQUIPMENT = 4,
						PLACEABLE = 8
				}
		}
}