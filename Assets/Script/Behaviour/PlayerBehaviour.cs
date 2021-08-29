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

				private float money = 0;
				private Equipment activeEquipment;
				private float toggleTimeout;
				private bool toggle;
				private AudioClip stepSoundClip;
				private bool mouseDown;

				public Camera Cam => playerCam;

				private Transform Transform { get; set; }

				public Equipment ActiveEquipment => activeEquipment;

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
						throw new NotImplementedException();
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
						activeEquipment = equipmentHolder.GetComponentInChildren<Equipment>();
						activeEquipment?.OnPlayer_EquippedToHand(this);
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
						HandleClickCrosshair();
						HandleEquipmentToggleButton();
						HandleEquipmentDropButton();
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
						IInputControls keyboardKeys = this.InputControls();
						Equipment equipment = activeEquipment;
						if (equipment is null)
						{
								// no equipment - everything is fine
								return;
						}

						// try to toggle on/off held equipment
						if (keyboardKeys.PlayerToggleEquipmentOnOff)
						{
								if (toggleTimeout <= 0 && toggle is false)
								{
										// 30 ms wait
										toggleTimeout = 30 / 1000f;

										if (equipment is { })
										{
												Debug.Log($"Triggered by Keyboard Hotkey: {nameof(keyboardKeys.PlayerToggleEquipmentOnOff)}");
												DoInteractWithEquipment();
										}
										else
										{
												Debug.LogWarning("No item in hand!");
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

				public void InteractWith(Interactible any)
				{
						if (any.CanInteract(this))
						{
								any.Interact(this);
								Debug.Log($"Item interaction: {any.gameObject.name}");
						}
						else
						{
								Debug.LogWarning($"Cannot interact with item: {any.gameObject.name}. Already Used!");
						}
				}

				public void PickUp(PickupItem item)
				{
						// Handle: is already in hand of any player?
						if (item.CanInteract(this))
						{
								OnPickUp_HandleActivePickupItem(item);
								Debug.Log($"Item pickup: {item.gameObject.name}");
						}
						else
						{
								Debug.LogWarning($"Cannot pickup item: {item.gameObject.name}. Already pickup!");
						}
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

				private void HandleClickCrosshair()
				{
						const float maxDistance = 2.25f; // war 2.25

						InteractionEnum equipmentAction = ClickInteractible(maxDistance, out Equipment equipment);
						HandleEquipmentInteraction(equipmentAction, equipment);

						// prio for equipment picking
						if (equipmentAction != InteractionEnum.CLICKED_ACTIVE)
						{
								InteractionEnum interactibleAction = ClickInteractible(maxDistance, out Interactible worldInteractible);
								HandleInteractibleInteraction(interactibleAction, worldInteractible);
						}

						return;
				}

				private void HandleEquipmentInteraction(InteractionEnum result, Equipment equipment)
				{
						switch (result)
						{
								case InteractionEnum.NONE:
										break;
								case InteractionEnum.HOVER_ACTIVE:
										break;
								case InteractionEnum.CLICKED_ACTIVE:
										Equip(equipment);
										break;
								case InteractionEnum.CLICKED_TOO_FAR:
										break;
								case InteractionEnum.HOVER_TOO_FAR:
										break;
								default:
										break;
						}
				}

				private void HandleInteractibleInteraction(InteractionEnum result, Interactible worldInteractible)
				{
						switch (result)
						{
								case InteractionEnum.NONE:
										break;
								case InteractionEnum.HOVER_ACTIVE:
										break;
								case InteractionEnum.CLICKED_ACTIVE:
										break;
								case InteractionEnum.CLICKED_TOO_FAR:
										break;
								case InteractionEnum.HOVER_TOO_FAR:
										break;
								default:
										throw new MissingSwitchCaseException(result);
						}
				}

				private InteractionEnum ClickInteractible<T>(float maxDistance, out T instance)
						where T : Interactible
				{
						mouseDown = Mouse.current.leftButton.isPressed;
						if (mouseDown)
						{
								float nearByDistance = maxDistance + maxDistance * 0.5f;
								if (CrosshairShoot(transform.position, maxDistance, out var inRangeHit, out Vector3 target))
								{
										if (inRangeHit.collider is { } && inRangeHit.collider.GetComponent<T>() is Component you)
										{
												Debug.Log($"Clicked Object: {you.gameObject.name} - IN RANGE: {inRangeHit.distance} m");
												instance = you is T t ? t : default;
												return InteractionEnum.CLICKED_ACTIVE;
										}
								}
								else if (CrosshairShoot(transform.position, nearByDistance, out inRangeHit, out target))
								{
										if (inRangeHit.collider is { } && inRangeHit.collider.GetComponent<T>() is Component you)
										{
												Debug.Log($"Clicked Object: {you.gameObject.name} - TOO FAR: {inRangeHit.distance} m");
												instance = you is T t ? t : default;
												return InteractionEnum.CLICKED_ACTIVE;
										}
								}
						}
						else if (CrosshairShoot(transform.position, maxDistance, out var inRangeHit, out var target))
						{
								if (inRangeHit.collider is { } && inRangeHit.collider.GetComponent<T>() is Component you)
								{
										Debug.Log("Not clickable Object: " + you.gameObject.name);
										instance = you is T t ? t : default;
										return InteractionEnum.CLICKED_TOO_FAR;
								}
						}

						instance = default;
						return InteractionEnum.NONE;

						bool CrosshairShoot(Vector3 toolPosition, float dist, out RaycastHit hit, out Vector3 aimPoint)
						{
								// Unless you've mucked with the projection matrix, firing through
								// the center of the screen is the same as firing from the camera itself.
								var camera = Camera.main.transform;
								Ray crosshair = new Ray(camera.position, camera.forward);

								// Cast a ray forward from the camera to see what's 
								// under the crosshair from the player's point of view.
								if (Physics.Raycast(crosshair, out hit, dist))
								{
										aimPoint = hit.point;
								}
								else
								{
										aimPoint = crosshair.origin + crosshair.direction * dist;
								}

								// Now we know what to aim at, do a second ray cast from the tool.
								Ray beam = new Ray(toolPosition, aimPoint - toolPosition);

								// If we don't hit anything, just go straight to the aim point.
								if (!Physics.Raycast(beam, out hit, dist))
								{
										return false;
								}

								// Otherwise, stop at whatever we hit on the way.
								return false;
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
						item.transform.SetParent(Transform, false);
				}

				[ContextMenu("Drop equipped item")]
				public void DropEquipment()
				{
						if (activeEquipment is { })
						{
								// important! let fall
								activeEquipment.DropItem(this);
								
								// important! unset reference
								activeEquipment = null;
						}
				}

				private void OnUselessEquipment(Equipment obj)
				{
						if (obj == activeEquipment)
						{
								if (silentHurtClipsRandom.Length > 0)
								{
										AudioClip clip = silentHurtClipsRandom[UnityEngine.Random.Range(0, silentHurtClipsRandom.Length)];
										if (clip is { } && clip.length > 0)
										{
												PlayOnceFromPosition(clip);
										}
								}
						}
				}

				private void PlayOnceFromPosition(AudioClip clip)
				{
						if (playerAudioSource3d is { })
						{
								playerAudioSource3d.spatialBlend = 1;
								playerAudioSource3d.PlayOneShot(clip);
								Debug.LogWarning("Play Audio Clip");
						}
						else
						{
								Debug.LogWarning("Missing AudioSource");
						}
				}

				[ContextMenu("Toggle On | Off")]
				public void DoInteractWithEquipment()
				{
						Interactible equipment = activeEquipment;
						if (equipment is null)
						{
								Debug.Log("There's no equipment recognized. Nothing to interact.");
								return;
						}

						equipment.Interact(this);
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