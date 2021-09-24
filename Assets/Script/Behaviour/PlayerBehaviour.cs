using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Behaviour.GhostTypes;
using Assets.Script.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEditor;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(CrosshairTargetInfo))]
		[DisallowMultipleComponent]
		public class PlayerBehaviour : MonoBehaviour, IStepSoundProvider
		{
				[SerializeField] private PlayerInfo playerData = new PlayerInfo();
				[Space]
				[SerializeField] private ItemHolder equipmentHolder;
				[SerializeField] private ItemHolder pickupHolder;
				[SerializeField] private ItemHolder cameraHolder;
				[Space]
				[SerializeField] private AudioSource playerAudioSource3d;
				[SerializeField] private InventoryBehaviour inventory;

				[Header("Steps Fallback Audio")]
				[SerializeField] private SoundEffect stepSoundsRandom;

				private float money = 0;
				private AudioClip stepSoundClip;
				private bool mouseDown;
				/// <summary>
				/// TODO - Handle show messages for a certain amount of time in a coroutine
				/// </summary>
				private readonly Stack<string> messagesToShow = new Stack<string>();

				public PlayerInfo PlayerData => playerData;

				private Transform Transform { get; set; }

				public ICrosshairInfo CrosshairTargetInfo { get; set; }

				public IEquipment ActiveEquipment => equipmentHolder is IItemHolder holder ? holder.CurrentItem as Equipment : null;

				public bool IsTeleported { get; private set; }

				private bool IsButtonPlacingPressed => this.InputControls().PlaceEquipmentButtonPressed;

				private bool IsButtonInteractionPressed => this.InputControls().CrosshairTargetInteractionButtonPressed;

				private void Awake()
				{
						Transform = transform;
						CrosshairTargetInfo = GetComponent<CrosshairTargetInfo>();
				}

				private void Start()
				{
						var stepsComponent = GetComponentInChildren<StepAnimationEventReference>();
						stepsComponent.StepAnimationEvent -= OnWalkingAnimation_OnStep;
						stepsComponent.StepAnimationEvent += OnWalkingAnimation_OnStep;
				}

				private void Update()
				{
						(HitInfo ClickRange, HitInfo HoverRange) = CrosshairTargetInfo.Info;
						if (ClickRange.IsHit)
						{
								ObjectHitInfo clickableTarget = CrosshairTargetInfo.ObjectInfo.InClickRange;
								if (clickableTarget.HasTargetItem)
								{
										HandleCrosshairClickEventWithTarget(clickableTarget);
								}
						}
						else if (HoverRange.IsHit)
						{
								// too far
						}
						else
						{
								// nothing
						}
				}

				public void AddMessage(string message) => messagesToShow.Push(message);

				public void SetTeleported() => IsTeleported = true;

				public void SetReturnedFromTeleport() => IsTeleported = false;

				public bool CheckCanBuy(ShopParameters equipmentInfo, uint quantity = 1) => money >= equipmentInfo.Cost * quantity;

				/// <summary>
				/// Counts the quantity for this item to buy, depends to the <see cref="money"/> 
				/// and <see cref="ShopParameters.cost"/>
				/// </summary>
				public int CountMaximumForMoney(ShopParameters item) => Mathf.FloorToInt(money / item.Cost);

				public int CountOwnedEquipment(ShopParameters item) => inventory.Count(item);

				public void SellOne(Equipment ownedEquipment)
				{
						// item must be owned by this player
						// inventory contains this equipment
						if (ownedEquipment.Owner == this)
						{
								if (inventory.Count(ownedEquipment.ShopInfo) > 0)
								{
										inventory.Take(ownedEquipment.ShopInfo, 1, out List<(Equipment item, int amount)> taken);
										float sellPrice = ownedEquipment.ShopInfo.SellPrice * 1;
										money += sellPrice;
										Debug.Log($"Sold {1} x {ownedEquipment.ShopInfo.DisplayName} for {sellPrice}");
										OnEquipmentSold(ownedEquipment.ShopInfo);
								}
						}
				}

				private void OnEquipmentSold(ShopParameters shopInfo)
				{
				}

				public void TakeEquipment(PlayerBehaviour diedPlayer)
				{
						if (diedPlayer.playerData.isDead)
						{
								var oneItem = inventory.TakeRandomItem();
								if (oneItem is { } && inventory.CheckCanPutAll(oneItem.ShopInfo, 1))
								{
										PutEquipmentIntoInventory(oneItem);
								}
						}
				}

				/// <summary>
				/// You can put one item into your inventory
				/// </summary>
				public bool PutEquipmentIntoInventory(Equipment equipmentWithoutOwner)
				{
						// an equipment within the world and without owned by someone
						// this may happen, if a player dies. Other players can pickup
						// his equipments to sell it in the shop
						// therefore the living player must pickup the died player
						if (equipmentWithoutOwner.Owner == null)
						{
								int remaining = inventory.PutAllEquipment(equipmentWithoutOwner.ShopInfo, 1);
								if (remaining > 0)
								{
										Debug.Log($"Not enought space in inventory: {equipmentWithoutOwner.ShopInfo.DisplayName}");
								}
								else
								{
										// valid!
										return true;
								}
						}
						return false;
				}

				public void Buy(ShopParameters shopItem, uint quantity = 1)
				{
						if (CheckCanBuy(shopItem, quantity))
						{
								float oldMoney = money;
								money -= shopItem.Cost * quantity;
								int remaining = inventory.PutAllEquipment(shopItem, quantity);
								if (remaining > 0)
								{
										money += remaining * shopItem.Cost;
								}
								Debug.Log($"Bought {quantity - remaining} items for {oldMoney - money}: {shopItem.DisplayName}");
						}
						else
						{
								Debug.Log($"Not enough money!");
						}
				}

				[CalledByEquipmentBehaviour]
				public void OnEquipment_Broken(Equipment equipment)
				{
						// broken can only be called (logically), if it was active
						// what will the player do on this event case?
						// - starting a cool down?
						// - show a text?
						// - mark as broken?
				}

				/// <summary>
				/// Call this from an equipment AFTER it was fixed from being broken
				/// </summary>
				[CalledByEquipmentBehaviour]
				public void OnEquipment_Fixed(Equipment equipment)
				{
						// wanna play a sound?s
				}

				private void PickUp(IPickupItem item)
				{
						// Handle: is already in hand of any player?
						pickupHolder.DropThenPut(this, item, false);
						Debug.Log($"Item pickup: {item.GetTargetName()}");
				}

				private void OnCrosshairClick_HandleInteractible(IInteractible target)
				{
						if (IsButtonInteractionPressed)
						{
								if (target.IsLocked)
								{
										Debug.Log($"Interaction-button @ on LOCKED: {target.GetTargetName()}");
										return;
								}

								// 2nd toggle: right hand
								Debug.Log($"Interact @ target: {target.GetTargetName()}");
								target.RunInteraction(this);
						}
				}

				private void OnCrosshairClick_HandlePickup(IPickupItem target)
				{
						if (IsButtonInteractionPressed)
						{
								if (target.IsLocked)
								{
										Debug.Log($"Interaction-button @ on LOCKED: {target.GetTargetName()}");
										return;
								}

								if (target.IsTakenByPlayer is false && target.CheckPlayerCanPickUp(this))
								{
										// 1st pickup: left hand
										Debug.Log($"Pickup @ target: {target.GetTargetName()}");
										PickUp(target);
								}
						}
				}

				private void OnCrosshairClick_HandleEquipment(IEquipment target)
				{
						OnCrosshairClick_HandleInteractible(target);
				}

				private void OnCrosshairClick_HandlePlaceable(IPlacableEquipment target)
				{
						if (IsButtonPlacingPressed)
						{
								if (target.IsLocked)
								{
										Debug.Log($"placing-button @ on LOCKED: {target.GetTargetName()}");
										return;
								}

								Debug.Log($"placing-button @ placeable: {target.GetTargetName()}");

								if (target.IsTakenByPlayer is false)
								{
										Debug.Log($"Equip @ placeable: '{target.GetTargetName()}'");
										// right hand: equip
										DropThenEquip(target);
								}
								else
								{
										Debug.Log($"Equip Placable @ already taken by player: '{target.GetTargetName()}'");
								}
						}
				}

				private void HandleCrosshairClickEventWithTarget(ObjectHitInfo clickableTarget)
				{
						IPlacableEquipment placeable = clickableTarget.GetPlaceableItem();
						if (placeable != null)
						{
								OnCrosshairClick_HandlePlaceable(placeable);
								return;
						}

						IEquipment equipment = clickableTarget.GetEquipment();
						if (equipment != null)
						{
								OnCrosshairClick_HandleEquipment(equipment);
								return;
						}

						IPickupItem pickup = clickableTarget.GetPickupItem();
						if (pickup != null)
						{
								OnCrosshairClick_HandlePickup(pickup);
								return;
						}

						IInteractible interactible = clickableTarget.GetInteractible();
						if (interactible != null)
						{
								OnCrosshairClick_HandleInteractible(interactible);
								return;
						}
				}

				private static IPickupItem GetPreferredPickable(IEquipment equipment, IPickupItem pickup)
				{
						return equipment ?? pickup;
				}

				private static IInteractible GetPreferredInteractible(IEquipment equipment, IInteractible any)
				{
						return equipment ?? any;
				}

				private void HandlePickupItem(IPickupItem item)
				{
						if (item.IsTakenByPlayer is false)
						{
								PickUp(item);
						}
				}

				private void HandleEquipment(IEquipment equipment)
				{
						if (equipment.CanInteract(this))
						{
								if (equipment.IsTakenByPlayer is false
										&& (!(equipment is IPlacableEquipment p) || p.IsPlaced is false))
								{
										//drop and equip:
										DropThenEquip(equipment);
								}
								else
								{
										equipment.RunInteraction(this);
										Debug.Log($"'{gameObject.name}' toggled a placed '{equipment.GetTargetName()}'");
								}
						}
				}

#if UNITY_EDITOR
				private void OnGUI()
				{
						EditorGUILayout.HelpBox("T = Interagieren", MessageType.Info);
						EditorGUILayout.HelpBox("H = Hunt", MessageType.Info);
						EditorGUILayout.HelpBox("Backspace = Drehen Aus", MessageType.Info);
						EditorGUILayout.HelpBox("W,A,S,D = Laufen", MessageType.Info);
						EditorGUILayout.HelpBox("Maus = Sehen", MessageType.Info);
						EditorGUILayout.HelpBox("C = Ducken", MessageType.Info);
						EditorGUILayout.HelpBox("G = Platzieren", MessageType.Info);
				}

				private void OnDrawGizmos()
				{
						if (Application.isPlaying)
						{
								if (mouseDown)
								{
										Ray forward = new Ray(Camera.current.transform.position, Camera.current.transform.forward);
										if (Physics.Raycast(forward, out var hit, 20))
										{
												Vector3 hitPos = hit.point;
												Handles.color = Color.yellow;
												Handles.DrawLine(forward.origin, hitPos);

												Handles.color = Color.white;
												Handles.Label(hitPos, $"{Vector3.Distance(forward.origin, hitPos)} m");
										}
								}
						}
				}
#endif

				private IItemHolder EquipmentHolder => equipmentHolder;

				private IItemHolder CameraHolder => cameraHolder;

				public void DropThenEquip(IEquipment item)
				{
						if (item.ObjectType == EObjectType.CAMERA)
						{
								// insert equipment as child!
								CameraHolder.DropThenPut(this, item, false);
						}
						else
						{
								EquipmentHolder.DropThenPut(this, item, false);
						}
				}

				public void Die()
				{
						if (playerData.isDead is false && playerData.ghostType is null)
						{
								playerData.isDead = true;
								playerData.ghostType = gameObject.AddComponent<GhostEntity>();
								playerData.ghostType.CopyPersonality(this);
						}
				}

				public void Reserect()
				{
						if (playerData.isDead && playerData.reserectable)
						{
								playerData.isDead = false;
								Destroy(playerData.ghostType);

								// das geht nur einmal bei einem Ritual
								playerData.reserectable = false;
						}
				}

				public void SetStepSound(AudioClip clip)
				{
						var oldName = GetSoundName(stepSoundClip);
						this.stepSoundClip = clip;
						Debug.Log($"Changed step sound for {gameObject.name} from {oldName} to {GetSoundName(stepSoundClip)}");
				}

				private string GetSoundName(AudioClip clip)
				{
						if (clip == null || clip.name is null)
						{
								return "<null>";
						}
						if (string.IsNullOrWhiteSpace(clip.name))
						{
								return "<no name>";
						}
						return $"'{clip.name}'";
				}

				public void OnWalkingAnimation_OnStep()
				{
						if (stepSoundClip == null)
						{
								stepSoundsRandom.PlayRandomOnce(gameObject.name, "Step Fallback", playerAudioSource3d);
						}
						else
						{
								//playerAudioSource3d.PlayOneShot(stepSoundClip);
						}
				}
		}
}