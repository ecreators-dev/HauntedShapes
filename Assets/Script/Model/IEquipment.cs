using Assets.Script.Model;

using System;

namespace Assets.Script.Behaviour
{
		public interface IEquipment : IMonoBehaviour
		{
				event Action<IEquipment> LetFallEvent;
				event Action<IEquipment> UselessEvent;

				float Price { get; }
				float SellPrice { get; }
				PlayerBehaviour OwnedByPlayer { get; }
				PlayerBehaviour InUseOfPlayer { get; }
				bool IsUseless { get; }
				bool IsPowered { get; }
				bool CanToggleOnOff { get; }
				string Name { get; }
				bool IsEquipped { get; }

				void OnEquip(PlayerBehaviour player);

				/// <summary>
				/// After an equipment got useless. You can call this to re-power it up, if it is possible.
				/// </summary>
				void PowerUp(PlayerBehaviour player);

				/// <summary>
				/// Toggles power to status "ON" (activate)
				/// </summary>
				void ToggleOn(PlayerBehaviour player);

				/// <summary>
				/// Toggles power to status "OFF" (deactivate)
				/// </summary>
				void ToggleOff(PlayerBehaviour player);

				void Sell(PlayerBehaviour player);

				void Buy(PlayerBehaviour player);
				void Drop();
				void NoFall();
				void ShowTextInWorld(PlayerBehaviour sender, string text);
		}
}