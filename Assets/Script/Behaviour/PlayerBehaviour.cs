using Assets.Script.Behaviour.GhostTypes;
using Assets.Script.Components;

using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

using Debug = UnityEngine.Debug;

namespace Assets.Script.Behaviour
{
		[DisallowMultipleComponent]
		public class PlayerBehaviour : MonoBehaviour, IPlayerCam, IStepSoundProvider
		{
				public string playerFirstName;
				public string playerLastName;
				public int playerAge;
				public Gender gender;
				public DateTime birthDay;
				public DateTime deathDay;
				public string murderName;
				public string motherName;
				public string fatherName;
				public List<(string firstName, bool male)> sisters;
				public MoodType mood;
				public AgeType ageType;
				public bool reserectable = true;
				public int level;
				public long levelExpirience;

				private GhostEntity ghostType;
				private bool isDead;

				[SerializeField] private Transform equipmentHolder;
				[SerializeField] private Transform pickupHolder;
				[SerializeField] private Camera playerCam;
				[SerializeField] private AudioSource playerAudioSource3d;
				[SerializeField] private AudioClip[] silentHurtClipsRandom;
				[SerializeField] private LayerMask interactibleDoorMask;
				[SerializeField] private InventoryBehaviour inventory;
				[SerializeField] private CrosshairHitVisual crosshair;

				private float money = 0;
				private Equipment activeEquipment;
				private float toggleTimeout;
				private bool toggle;
				private AudioClip stepSoundClip;
				private bool mouseDown;
				private float litIntensity;
				private readonly List<LightInteractor> litLights = new List<LightInteractor>();

				public Camera Cam => playerCam;

				private Transform Transform { get; set; }

				public Equipment ActiveEquipment => activeEquipment;

				public bool IsPlayerInDark { get; private set; }
				
				// for statistics
				public float PlayerDarknessTime { get; private set; }
				
				// for scoring
				public float PlayerDarknessTimeFactorized { get; private set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						FindEquipmentInEquipmentHolder();
				}

				public bool CheckCanBuy(ShopParameters equipmentInfo, uint quantity = 1)
				{
						return money >= equipmentInfo.Cost * quantity;
				}

				/// <summary>
				/// Counts the quantity for this item to buy, depends to the <see cref="money"/> 
				/// and <see cref="ShopParameters.cost"/>
				/// </summary>
				public int CountMaximumForMoney(ShopParameters item)
				{
						return Mathf.FloorToInt(money / item.Cost);
				}

				public int CountOwnedEquipment(ShopParameters item) => inventory.Count(item);

				public void SellOne(Equipment ownedEquipment)
				{
						// item must be owned by this player
						// inventory contains this equipment
						if (ownedEquipment.Owner == this)
						{
								if (inventory.Count(ownedEquipment.ShopInfo) > 0)
								{
										inventory.Take(ownedEquipment.ShopInfo, 1);
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

				public void InLightUpdate(LightInteractor lightInteractor, float intensity)
				{
						this.litLights.Add(lightInteractor);
						this.litIntensity += intensity;
				}

				public void TakeEquipment(PlayerBehaviour diedPlayer)
				{
						if (diedPlayer.isDead)
						{
								var oneItem = inventory.GetRandomEquipment();
								if (oneItem is { } && inventory.CheckCanPutAll(oneItem.ShopInfo, 1))
								{
										PutEquipment(oneItem);
								}
						}
				}

				/// <summary>
				/// You can put one item into your inventory
				/// </summary>
				public bool PutEquipment(Equipment equipmentWithoutOwner)
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

				private void FindEquipmentInEquipmentHolder()
				{
						activeEquipment = equipmentHolder.GetComponentInChildren<Equipment>(true);
						if (activeEquipment is { })
						{
								Equip(activeEquipment);
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

				private void Update()
				{
						HandleHuntToggleDebug();
						HandleCrosshairClick();
						HandleEquipmentToggleButton();
						HandleEquipmentDropButton();
						UpdatePlayerInDark();
						HandlePlayerInDarkness();
				}

				private void HandlePlayerInDarkness()
				{
						if (IsPlayerInDark)
						{
								PlayerDarknessTime += Time.deltaTime;
								float multiplier = GetLightSourceEquipmentMultiplier();
								PlayerDarknessTimeFactorized = Time.deltaTime * multiplier;
						}
				}

				private float GetLightSourceEquipmentMultiplier()
				{
						float multiplier = 1;
						if (activeEquipment is ILightSource source
								&& source.IsActive is false)
						{
								multiplier = source.ActiveMultiplier;
						}
						return multiplier;
				}

				private void UpdatePlayerInDark()
				{
						IsPlayerInDark = true;
						if (litLights.Count > 0)
						{
								float averageIntensity = litIntensity / litLights.Count;
								Debug.Log($"Player intensity: {averageIntensity}");
								if (averageIntensity >= 0.1f)
								{
										IsPlayerInDark = false;
								}

								// reset at end of frame!
								litLights.Clear();
								litIntensity = 0;
						}
				}

				/// <summary>
				/// Handles only to drop the actual equipment
				/// </summary>
				private void HandleEquipmentDropButton()
				{
						if (activeEquipment is { } && this.InputControls().PlayerDropEquipment)
						{
								DropEquipment();
						}
				}

				private void HandleHuntToggleDebug()
				{
						if (this.InputControls().DebugHuntToggleOnOff)
						{
								HuntingStateBean hunt = HuntingStateBean.Instance;
								if (hunt.InHunt is false)
								{
										hunt.StartHunt();
								}
								else
								{
										hunt.StopHunt();
								}
						}
				}

				private void HandleEquipmentToggleButton()
				{
						if (activeEquipment is null)
						{
								// no equipment - everything is fine
								FindEquipmentInEquipmentHolder();
								return;
						}

						// try to toggle on/off held equipment
						if (this.InputControls().PlayerToggleEquipmentOnOff)
						{
								if (toggleTimeout <= 0 && toggle is false)
								{
										// 30 ms wait
										toggleTimeout = 30 / 1000f;

										if (activeEquipment is { })
										{
												InteractWithEquipment();
										}
										else
										{
												Debug.Log("No item in hand!");
										}
										toggle = true;
								}
						}
						else
						{
								toggle = false;
						}

						if (toggleTimeout > 0)
						{
								toggleTimeout -= Time.deltaTime;
						}
				}

				private void PickUp(PickupItem item)
				{
						// Handle: is already in hand of any player?
						item.transform.SetParent(pickupHolder, true);
						OnPickUp_HandleActivePickupItem(item);
						Debug.Log($"Item pickup: {item.gameObject.name}");
				}

				private void OnPickUp_HandleActivePickupItem(Interactible item)
				{
						Transform parent = pickupHolder;
						PickupItem active = parent.GetComponentInChildren<PickupItem>();
						if (active != null)
						{
								if (active != null && item != active)
								{
										// player can only carry one item at the same time!
										DropItem(active);
								}
						}
				}

				private void DropItem(PickupItem item)
				{
						item.DropItem(this);
				}

				private void HandleCrosshairClick()
				{
						// an (any) interactible must be in interactible layer mask
						// to be clickable with name
						if (crosshair.TryGetObject(out bool inClickRange, out var match)
								&& inClickRange
								&& CheckInteractButtonIsPressed()
								&& match.any.IsUnlocked)
						{
								// equipment is an pickupitem
								// pickupitem is an interactible

								// interactible can be Interact
								// pickupitem can be Interact and PickUp, but not set to inventory
								// equipment can be Interact and PickUp and set to inventory

								if (match.equipment is { })
								{
										if (match.item.IsTakenByPlayer is false)
										{
												HandleEquipment(match.equipment);
										}
								}
								else if (match.item is { })
								{
										if (match.item.IsTakenByPlayer is false)
										{
												HandlePickupItem(match.item);
										}
								}
								else if (match.any is { })
								{
										HandleInteractible(match.any);
								}
						}
				}

				private bool CheckInteractButtonIsPressed()
				{
						// mouse or keyboard
						// (pickup to equip or pickup to take or interact with s.th. you cannot pickup)
						return Mouse.current.leftButton.isPressed ||
								this.InputControls().InteractionCrosshairPressed;
				}

				private void HandlePickupItem(PickupItem item)
				{
						if (item.IsTakenByPlayer is false)
						{
								PickUp(item);
						}
						else
						{
								HandleInteractible(item);
						}
				}

				private void HandleEquipment(Equipment equipment)
				{
						if (equipment.CanInteract(this))
						{
								if (equipment.IsTakenByPlayer is false)
								{
										Equip(equipment);
								}
								else
								{
										HandleInteractible(equipment);
								}
						}
				}

				private void HandleInteractible(Interactible any)
				{
						// TODO inside Interact! No check from outside behaviour!
						if (any.CanInteract(this))
						{
								// for example: a door is opened or closed
								// a flashlight is toggled on or off
								// an item is switched on or off or nothing
								any.Interact(this);
						}
				}

#if UNITY_EDITOR
				private void OnGUI()
				{
						EditorGUILayout.HelpBox("Toggle On/Off = T", MessageType.Info);
						EditorGUILayout.HelpBox("Hunt Fx On/Off = H", MessageType.Info);
						EditorGUILayout.HelpBox("Mouse On/Off = Backspace", MessageType.Info);
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
				public void Equip(Equipment item)
				{
						activeEquipment = item;
						Transform equipment = item.transform;
						equipment.SetParent(equipmentHolder, false);
						equipment.localPosition = Vector3.zero;
						equipment.localEulerAngles = Vector3.zero;
						item.OnPlayer_ItemPickedUp(this);
						item.OnPlayer_EquippedToHand(this);
				}

				[ContextMenu("Drop equipped item")]
				public void DropEquipment()
				{
						if (activeEquipment is { })
						{
								Debug.Log($"Drop: {activeEquipment.GetTargetName()}");

								// important! let fall
								activeEquipment.DropItem(this);

								// important! unset reference
								activeEquipment = null;
						}
				}

				[ContextMenu("Toggle On | Off")]
				public void InteractWithEquipment()
				{
						Interactible equipment = activeEquipment;
						if (equipment is null)
						{
								Debug.Log("There's no equipment recognized. Nothing to interact.");
						}
						else
						{
								equipment.Interact(this);
						}
				}

				public void Die()
				{
						if (isDead is false && ghostType is null)
						{
								isDead = true;
								ghostType = gameObject.AddComponent<GhostEntity>();
								ghostType.CopyPersonality(this);
						}
				}

				public void Reserect()
				{
						if (isDead && reserectable)
						{
								isDead = false;
								Destroy(ghostType);

								// das geht nur einmal bei einem Ritual
								reserectable = false;
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
		}
}