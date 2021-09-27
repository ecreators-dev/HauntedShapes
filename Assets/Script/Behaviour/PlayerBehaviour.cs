using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Behaviour.GhostTypes;
using Assets.Script.Components;

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(CharacterController))]
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
				private bool inTeleport;
				private CharacterController controller;

				/// <summary>
				/// TODO - Handle show messages for a certain amount of time in a coroutine
				/// </summary>
				private readonly Stack<string> messagesToShow = new Stack<string>();

				public PlayerInfo PlayerData => playerData;

				private Transform Transform { get; set; }

				public IEquipment ActiveEquipment => equipmentHolder is IItemHolder holder ? holder.CurrentItem as Equipment : null;

				private Transform exitTeleport { get; set; }

				public bool IsTeleported { get; private set; }

				public Rigidbody Rigidbody { get; private set; }

				public MeshCollider Collider { get; private set; }

				private IItemHolder EquipmentHolder => equipmentHolder;

				private IItemHolder CameraHolder => cameraHolder;

				public bool InPlacing { get; private set; }

				private void Awake()
				{
						Transform = transform;
						Rigidbody = GetComponent<Rigidbody>();
						Collider = GetComponent<MeshCollider>();
						controller = GetComponent<CharacterController>();
				}

				private void Start()
				{
						var stepsComponent = GetComponentInChildren<StepAnimationEventReference>();
						stepsComponent.StepAnimationEvent -= OnWalkingAnimation_OnStep;
						stepsComponent.StepAnimationEvent += OnWalkingAnimation_OnStep;
				}

				private void Update()
				{
						(HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, _) = CrosshairHitVisual.Instance.RaycastInfo;
						if (ClickRange.IsHit)
						{
								ObjectHitInfo clickableTarget = CrosshairHitVisual.Instance.RaycastObjectInfo.ClickRange;
								if (clickableTarget.HasOwner)
								{
										HandleCrosshairTarget(clickableTarget);
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

						UpdateForPushObjects();
				}

				private float forceMultiplyer = 1000f;
				private float velocity;
				private Vector3 Velocity;
				private Vector3 lastframepos;

				private void UpdateForPushObjects()
				{
						//calculates velocity
						Velocity.x = transform.position.x - lastframepos.x;
						Velocity.y = transform.position.x - lastframepos.y;
						Velocity.y = transform.position.y - lastframepos.y;
						//calculates velocity "Speed"
						float vx;
						float vy;
						float vz;
						if (Velocity.x < 0) { vx = Velocity.x * -1; } else { vx = Velocity.x; };
						if (Velocity.y < 0) { vy = Velocity.y * -1; } else { vy = Velocity.y; };
						if (Velocity.z < 0) { vz = Velocity.z * -1; } else { vz = Velocity.z; };
						velocity = vx + vy + vz;
						//Sets the lastframe pos
						lastframepos = transform.position;
				}

				//checks character controller collision
				private void OnControllerColliderHit(ControllerColliderHit collision)
				{
						PushObjects(collision);
				}

				private void PushObjects(ControllerColliderHit collision)
				{
						//checks if there is rigidbody
						if (collision.rigidbody == null || collision.rigidbody.mass > Rigidbody.mass)
								return;

						Vector3 pushDir = Velocity;

						//Adds force to the object
						collision.rigidbody.AddForceAtPosition(
								pushDir * velocity * forceMultiplyer * Time.deltaTime,
								collision.point,
								ForceMode.Impulse);
				}

				private void LateUpdate()
				{
						if (inTeleport)
						{
								inTeleport = false;
								Transform target = Transform;
								target.position = exitTeleport.position;
								target.rotation = exitTeleport.rotation;
								Debug.Log($"Teleportation: '{gameObject.name}' to '{exitTeleport.gameObject.name}'");
						}
				}

				public void AddMessage(string message) => messagesToShow.Push(message);

				public void SetTeleported(Transform exit)
				{
						inTeleport = true;
						exitTeleport = exit;
						IsTeleported = true;
				}

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

				private bool OnCrosshairClick_HandleInteractible(IInteractible target)
				{
						if (this.InputControls().InteractWithCrosshairTargetButton)
						{
								// 2nd toggle: right hand
								Debug.Log($"Interact @ target: {target.GetTargetName()}");
								return target.RunInteraction(this);
						}
						return false;
				}

				private void OnCrosshairClick_HandlePickup(IPickupItem target)
				{
						if (this.InputControls().InteractWithCrosshairTargetButton)
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

				private void OnCrosshairClick_HandleEquipment(IEquipment target, bool forceAction = false)
				{
						if (forceAction || this.InputControls().InteractWithCrosshairTargetButton)
						{
								if (target.IsTakenByPlayer is false)
								{
										bool validPlaceable = !(target is IPlacableEquipment tool) || tool.IsPlaced is false;
										if (target.CheckPlayerCanPickUp(this) && validPlaceable)
										{
												DropThenEquip(target);
										}
										else
										{
												OnCrosshairClick_HandleInteractible(target);
										}
								}
								else
								{
										OnCrosshairClick_HandleInteractible(target);
								}
						}
				}

				private void OnCrosshairClick_HandlePlaceable(IPlacableEquipment target)
				{
						if (this.InputControls().PlacingButton)
						{
								// pickup item, that is placed
								if (target.CheckPlayerCanPickUp(this))
								{
										InPlacing = false;
										Debug.Log($"Equip @ placeable: '{target.GetTargetName()}'");

										// right hand: equip, calls equipped notification --- starts placing update
										DropThenEquip(target);
								}
						}
						else if (this.InputControls().InteractWithCrosshairTargetButton)
						{
								OnCrosshairClick_HandleEquipment(target, true);
						}
				}

				private void HandleCrosshairTarget(ObjectHitInfo clickableTarget)
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

						// an placing item in hand
						if (equipmentHolder.CurrentItem is IPlacableEquipment tool)
						{
								// that item is not yet placed
								if (tool.IsPlaced is false)
								{
										if (this.InputControls().PlacingButton)
										{
												// player toggles on: (a)
												// player toggles off: (b)
												InPlacing = !InPlacing;

												// if (a): nothing here - next condition after
												// if (b): player wants to place at that position, only if button was pressed in this frame as well
												if (!InPlacing)
												{
														// place original at target position with normal
														PlaceItem(tool);
												}
										}

										// if (a)
										if (InPlacing)
										{
												// show Target Position, if anything is hit (click or hover)
												if (CrosshairHitVisual.Instance.RaycastInfo.IsHitAny)
												{
														CrosshairHitVisual.Instance.ShowTargetPosition(tool);
												}
												// hide Target Position, if nothing is hit (nor click or hover)
												else
												{
														CrosshairHitVisual.Instance.HideTargetOnce();
												}
										}
										// if (b) - handled in above condition
								}
						}
				}

				private void PlaceItem(IPlacableEquipment placable)
				{
						if (placable == null) return;

						if (EquipmentHolder.CurrentItem == placable)
						{
								// enables gravity!
								EquipmentHolder.Drop(true);

								// disables gravity if it is the dots projector!
								placable.PlaceAtPositionAndNormal(CrosshairHitVisual.Instance.RaycastInfo.ClickRange);

								// important: hide Target Position now (once)
								// delete preview with this:
								CrosshairHitVisual.Instance.HideTargetOnce();
						}
						else
						{
								Debug.LogWarning($"Cannot place item. It is no part in item holder: {placable.GetTargetName()}");
						}
				}

#if UNITY_EDITOR
				private void OnGUI()
				{
						// don't display controls anymore :/
						if (Application.isPlaying) return;

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
						if (!Application.isPlaying) return;

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
#endif

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

				private float lastStepTime;

				[Range(0, 3f)]
				[SerializeField] private float stepDelay = 0.3f;

				public void OnWalkingAnimation_OnStep()
				{
						float stepDelay = this.stepDelay;
						if (GetComponent<MovementForPlayer>().IsRunning)
						{
								stepDelay /= 2;
						}

						if ((Time.time - lastStepTime) >= stepDelay)
						{
								lastStepTime = Time.time;

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
}