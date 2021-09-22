namespace Assets.Script.Behaviour
{
		public interface IItemHolder
		{
				IPickupItem CurrentItem { get; }
				HolderTypeEnum Type { get; }

				void Drop();
				
				void PutIntoInventory();

				void Put(PlayerBehaviour user, IPickupItem item, bool fromInventory);
		}
}