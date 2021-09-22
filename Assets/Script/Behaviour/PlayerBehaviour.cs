using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Behaviour.GhostTypes;
using Assets.Script.Components;

using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEditor;

using UnityEngine;

using static Assets.Script.Components.Interactible;

using Debug = UnityEngine.Debug;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(CrosshairTargetInfo))]
		[DisallowMultipleComponent]
		public class PlayerBehaviour : MonoBehaviour, IPlayerCam, IStepSoundProvider
		{
				[SerializeField] private PlayerInfo playerData = new PlayerInfo();
				[SerializeField] private Transform equipmentHolder;
				[SerializeField] private Transform pickupHolder;
				[SerializeField] private Camera playerCam;
				[SerializeField] private AudioSource playerAudioSource3d;
				[SerializeField] private InventoryBehaviour inventory;
				[SerializeField] private TMP_Text infoText;
				[SerializeField] private TMP_Text timerText;

				[Header("Steps Fallback Audio")]
				[SerializeField] private SoundEffect stepSoundsRandom;

				public PlayerInfo PlayerData => playerData;

				private float money = 0;
				private Equipment activeEquipment;
				private AudioClip stepSoundClip;
				private bool mouseDown;
				private float litIntensity;
				private IInputControls inputControls;

				/// <summary>
				/// TODO - Handle show messages for a certain amount of time in a coroutine
				/// </summary>
				private readonly Stack<string> messagesToShow = new Stack<string>();

				private readonly List<LightInteractor> litLights = new List<LightInteractor>();

				public Camera Cam => playerCam;

				private Transform Transform { get; set; }
				private ICrosshairInfo PlacingInfo { get; set; }

				public Equipment ActiveEquipment => activeEquipment;

				public bool IsPlayerInDark { get; private set; }

				// for statistics
				public float PlayerDarknessTime { get; private set; }

				// for scoring
				public float PlayerDarknessTimeFactorized { get; private set; }

				public void AddMessage(string message) => messagesToShow.Push(message);

				private bool ButtonEquipmentTogglePressed { get; set; }
				private bool ButtonPlacePressed { get; set; }
				private bool ButtonDropPressed { get; set; }
				private bool ButtonCrosshairTargetInteractionPressed { get; set; }
				public bool InteractedInFrame { get; private set; }
				public bool IsTeleported { get; private set; }

				private void Awake()
				{
						Transform = transform;
						PlacingInfo = GetComponent<CrosshairTargetInfo>();
				}

				private void Start()
				{
						FindEquipment();

						var stepsComponent = GetComponentInChildren<StepAnimationEventReference>();
						stepsComponent.StepAnimationEvent -= OnWalkingAnimation_OnStep;
						stepsComponent.StepAnimationEvent += OnWalkingAnimation_OnStep;
				}

				public void SetTeleported() => IsTeleported = true;

				public void SetReturnedFromTeleport() => IsTeleported = false;

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

				public void InLightUpdate(LightInteractor lightInteractor, float intensity)
				{
						this.litLights.Add(lightInteractor);
						this.litIntensity += intensity;
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

				private void FindEquipment()
				{
						// avoid camera as equipment here!

						var foundEquipment = equipmentHolder.GetComponentInChildren<Equipment>(true);
						if (foundEquipment is { })
						{
								DropThenEquip(foundEquipment);
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
						InteractedInFrame = false;
						inputControls ??= this.InputControls();
						ButtonEquipmentTogglePressed = inputControls.InteractButtonPressed;
						ButtonPlacePressed = inputControls.PlaceEquipmentButtonPressed;
						ButtonDropPressed = inputControls.DropItemButtonPressed;
						ButtonCrosshairTargetInteractionPressed = inputControls.CrosshairTargetInteractionButtonPressed;

						HandleHuntToggleDebug();

						HandleCrosshairTarget();
						HandleToggleOnEquippedItem();
						HandleDropButton();

						HandleEquipmentInfoGui();
						UpdatePlayerInDark();
						HandlePlayerInDarkness();
				}

				private void HandleEquipmentInfoGui()
				{
						if (ActiveEquipment is { })
						{
								EquipmentInfo info = ActiveEquipment.GetEquipmentInfo();
								if (info is { })
								{
										ShowEquipmentInfo(info);
								}
								else
								{
										HideEquipmentInfo();
								}
						}
						else
						{
								HideEquipmentInfo();
						}
				}

				private void ShowEquipmentInfo(EquipmentInfo info)
				{
						if (info.Text != null)
						{
								infoText.gameObject.SetActive(true);
								infoText.text = info.Text;
						}
						else
						{
								infoText.gameObject.SetActive(false);
						}

						if (info.TimerText != null)
						{
								timerText.gameObject.SetActive(true);
								timerText.text = info.TimerText;
						}
						else
						{
								timerText.gameObject.SetActive(false);
						}
				}

				private void HideEquipmentInfo()
				{
						infoText.gameObject.SetActive(false);
						timerText.gameObject.SetActive(false);
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
								&& source.IsPowered is false)
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
				private void HandleDropButton()
				{
						if (activeEquipment is { } && ButtonDropPressed)
						{
								DropEquipment();
						}
				}

				private void HandleHuntToggleDebug()
				{
						if (inputControls.DebugHuntToggleOnOff)
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

				private void HandleToggleOnEquippedItem()
				{
						if (activeEquipment == null)
						{
								// no equipment - everything is fine
								FindEquipment();
								return;
						}

						// fixes double toggled by input via crosshair target clicked
						// when hovered = true -> end
						// hovered interaction is priorized

						// try to toggle on/off held equipment
						if (ButtonEquipmentTogglePressed)
						{
								Debug.Log("interaction-button pressed for equipment");
								OnActiveEquipment_ToggleInteractibleButtonPressed();
						}
				}

				private void OnActiveEquipment_ToggleInteractibleButtonPressed()
				{
						HandleInteractible(ActiveEquipment);
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
								if (active != null && item != active && item is PickupItem)
								{
										// player can only carry one item at the same time!
										active.DropItemRotated(this);
								}
						}
				}

				private void HandleCrosshairTarget()
				{
						// an (any) interactible must be in interactible layer mask
						// to be clickable with name
						if (PlacingInfo.Info.InClickRange.IsHit)
						{
								ObjectHitInfo objectInClickRange = PlacingInfo.ObjectInfo.InClickRange;
								var match = (
										objectInClickRange.GetEquipment(),
										objectInClickRange.GetPickupItem(),
										objectInClickRange.GetInteractible());

								if (objectInClickRange.HasTargetItem && objectInClickRange.TargetItem.IsUnlocked)
								{
										// an (any) interactible must be in interactible layer mask
										// to be clickable with name
										if (ButtonCrosshairTargetInteractionPressed)
										{
												Debug.Log("interaction-button pressed for hovered target");
												OnCrosshairHoverInteractible_InteractionButtonPressed(match);
										}
										else if (ButtonPlacePressed)
										{
												Debug.Log("place-button pressed for hovered target");
												OnCrosshairHoverInteractible_PlaceButtonPressed(match);
										}
								}
						}
						else if (PlacingInfo.Info.InHoverRange.IsHit)
						{

						}
						else
						{

						}

						return;
#if false
						if (((CrosshairHitVisual)CrosshairHitVisual.Instance).TryGetItem(out bool inClickRange, out (Equipment equipment, PickupItem item, Interactible any) match)
																				&& inClickRange
																				&& match.any.IsUnlocked)
						{
								// an (any) interactible must be in interactible layer mask
								// to be clickable with name
								if (ButtonCrosshairTargetInteractionPressed)
								{
										Debug.Log("interaction-button pressed for hovered target");
										OnCrosshairHoverInteractible_InteractionButtonPressed(match);
								}
								else if (ButtonPlacePressed)
								{
										Debug.Log("place-button pressed for hovered target");
										OnCrosshairHoverInteractible_PlaceButtonPressed(match);
								}
						}
#endif
				}

				private void OnCrosshairHoverInteractible_PlaceButtonPressed((Equipment equipment, PickupItem item, Interactible any) match)
				{
						// grab from anywhere, but not from other player
						if (match.equipment != null && match.equipment.IsTakenByPlayer is false && match.any.IsLocked is false)
						{
								DropThenEquip(match.equipment);
								Debug.Log($"'{gameObject.name}' equipped a placed '{match.equipment.GetTargetName()}'");
						}
				}

				private void OnCrosshairHoverInteractible_InteractionButtonPressed((Equipment equipment, PickupItem item, Interactible any) match)
				{
						// equipment is an pickupitem
						// pickupitem is an interactible

						// interactible can be Interact
						// pickupitem can be Interact and PickUp, but not set to inventory
						// equipment can be Interact and PickUp and set to inventory

						if (match.equipment is { })
						{
								HandleEquipment(match.equipment);
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
								if (equipment.IsTakenByPlayer is false
										&& (!(equipment is IPlacableEquipment p) || p.IsPlaced is false))
								{
										//drop and equip:
										DropThenEquip(equipment);
								}
								else
								{
										HandleInteractible(equipment);
										Debug.Log($"'{gameObject.name}' toggled a placed '{equipment.GetTargetName()}'");
								}
						}
				}

				private void HandleInteractible(Interactible any)
				{
						// inside only Interact! No check from outside behaviour!
						if (any.CanInteract(this) && !InteractedInFrame)
						{
								// for example: a door is opened or closed
								// a flashlight is toggled on or off
								// an item is switched on or off or nothing
								any.Interact(this);
								InteractedInFrame = true;
						}
						else if (!InteractedInFrame)
						{
								Debug.Log("Unable to interact: not allowed");
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
				public void DropThenEquip(Equipment item)
				{
						if (activeEquipment is { })
						{
								DropEquipment();
						}

						activeEquipment = item;
						Transform equipment = item.transform;
						equipment.SetParent(GetEquipmentHolder(item));

						item.OnPlayer_NotifyItemTaken(this);
				}

				private Transform GetEquipmentHolder(Equipment item)
				{
						if (item.ObjectType == EObjectType.CAMERA)
						{
								// insert equipment as child!
								return CameraMoveType.Instance.GetCamera().transform.GetChild(0);
						}

						// default holder
						return equipmentHolder;
				}

				[ContextMenu("Drop equipped item")]
				public void DropEquipment()
				{
						if (activeEquipment is { })
						{
								Debug.Log($"Drop: {activeEquipment.GetTargetName()}");

								// important! let fall
								activeEquipment.DropItemRotated(this);

								// important! unset reference
								activeEquipment = null;
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