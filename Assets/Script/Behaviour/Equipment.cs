using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Extends Items: you can put it into your inventory and take it off from it
		/// </summary>
		public abstract class Equipment : PickupItem
		{
				[Min(0)]
				[SerializeField] private float toggleTimerSeconds = 15 / 1000f;
				[SerializeField] private SoundEffect ambientLoopOff;

				private float toggleTimer;

				protected bool IsPowerToggled { get; private set; }

				public bool IsBroken { get; private set; }

				public bool IsPowered { get; private set; }

				public ShopParameters ShopInfo { get; private set; }

				public PlayerBehaviour Owner { get; private set; }

				protected override void Update()
				{
						base.Update();

						if (toggleTimer > 0)
						{
								toggleTimer -= Time.deltaTime;
						}

						if (CanPlayAmbientOffState())
						{
								// plays once:
								ambientLoopOff.PlayRandomLoop(GetTargetName(), "Ambient off sound loop", soundPlayer3dAmbient);
						}
				}

				private bool CanPlayAmbientOffState()
				{
						return IsPowered is false && ambientLoopOff != null && ambientLoopOff.IsLooping is false;
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender) && !IsBroken;
				}

				protected void SetPowered(bool active, bool force = false)
				{
						if (toggleTimer <= 0 || force)
						{
								toggleTimer = toggleTimerSeconds;
								IsPowerToggled = active != IsPowered;
								IsPowered = active;
								LogPowerCanged();

								if (IsPowerToggled)
								{
										if (active)
										{
												PlayToggleOnSound();
										}
										else
										{
												PlayToggleOffSound();
										}
								}
						}
						else
						{
								Debug.LogWarning($"'{GetTargetName()}': Toggle Power not possible: not ready!");
						}
				}

				private void LogPowerCanged()
				{
						var sb = new StringBuilder();
						const int max = 3;
						for (int i = 1; i < max; i++)
						{
								StackFrame frame = new StackFrame(i);
								MethodBase method = frame.GetMethod();
								string file = method.DeclaringType.Name;
								int position = i - 1;
								int number = max - position;
								string from = $"{number}:{GetTabs(position)}{file}.{method.Name}";
								sb.AppendLine(from);
						}
						Debug.Log($"'{GetTargetName()}': Set Power {!IsPowered} -> {IsPowered}:\ncalled by:\n{sb}");
				}

				private static string GetTabs(int amount) => string.Join("", new string[amount].Select(_ => "\t"));

				protected void TogglePowered(bool ignoreTimer = false)
				{
						if (toggleTimer <= 0 || ignoreTimer)
						{
								SetPowered(!IsPowered, ignoreTimer);

								toggleTimer = toggleTimerSeconds;
								LogPowerCanged();
						}
						else
						{
								Debug.LogWarning($"'{GetTargetName()}': Toggle Power not possible: not ready!");
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
						ShopInfo = shopInfo;
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