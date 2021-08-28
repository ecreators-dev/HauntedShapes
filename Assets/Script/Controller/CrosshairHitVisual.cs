using Assets.Script.Components;
using Assets.Script.Model;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Attached to an Canvas/RawImage (Hand)
		/// </summary>
		[RequireComponent(typeof(RawImage))]
		public class CrosshairHitVisual : MonoBehaviour
		{
				[SerializeField] private bool hoveredHit;
				[SerializeField] private float hitDistance = 4;
				[SerializeField] private float hitDistanceFar = 5;
				[SerializeField] private LayerMask interactibleLayer;
				[Range(0.0001f, 0.2f)]
				[SerializeField] private float size = 0.02f;

				private RawImage image;

				public void SetHitActive() => hoveredHit = true;
				public void SetHitInactive() => hoveredHit = false;

				private void Awake()
				{
						image = GetComponent<RawImage>();
				}

				private void Update()
				{
						Transform camera = Camera.main.transform;

						// HOVER: show near items with hand visible! (c)
						hoveredHit = false;
						Equipment equipment = default;
						PickupItem item = default;
						Interactible any = default;
						if (!Mouse.current.leftButton.isPressed)
						{
								HandleHover(camera, out equipment, out item, out any);
						}

						// CLICK:
						if (Mouse.current.leftButton.isPressed)
						{
								// a) clicked near (match)
								// b) clicked far (hover) - hand with arraow up (shows one step further to pickup)
								HandleClickedComponentInWorld(camera, equipment, item, any);
						}

						image.enabled = hoveredHit;
				}

				private void HandleHover(Transform camera, out Equipment equipment,
						out PickupItem item, out Interactible any)
				{
						equipment = default;
						item = default;
						any = default;

						// condition! 1st: match any, 2nd: match only types
						hoveredHit = Physics.SphereCast(camera.position, size, camera.forward, out RaycastHit clickInRange, hitDistance, interactibleLayer)
								&& IsHit(out equipment, out item, out any, clickInRange);

						static bool IsHit(out Equipment equipment,
								out PickupItem item,
								out Interactible any,
								RaycastHit clickInRange)
						{
								item = default;
								any = default;
								bool match = false;

								// hover only if not in hand!
								if (IsEquimentHit(clickInRange, out equipment)
										|| IsPickupItemHit(clickInRange, out item)
										|| IsInteractibleHit(clickInRange, out any))
								{
										match = true;
								}
								return match;
						}
				}

				/// <summary>
				/// Called when MouseLeft down = pressed!
				///
				private void HandleClickedComponentInWorld(Transform camera,
						Equipment equipment,
						PickupItem item,
						Interactible any)
				{
						// out of range (hitDistanceFar), but in near reange AND clicked
						// hovered and clicked in range:
						if (hoveredHit)
						{
								// current player who is playing the game active
								var player = Camera.main.transform.GetComponentInParent<PlayerBehaviour>();
								if (player != null)
								{
										if (equipment is { })
										{
												// for example: take a flashlight
												player.Equip(equipment);
										}
										else if (item is { })
										{
												// for example: pick up a photo
												player.PickUp(item);
										}
										else if (any is { })
										{
												// for example: open a door
												player.InteractWith(any);
										}
								}
								else
								{
										Debug.LogError($"No {nameof(PlayerBehaviour)} found in parent of camera.current");
								}
						}
						// clicked and not hovered: check if in near range!
						else if (Physics.SphereCast(camera.position, size, camera.forward, out RaycastHit clickOutOfRange, hitDistanceFar, interactibleLayer))
						{
								if (IsEquimentHit(clickOutOfRange, out Equipment tool))
								{
										Debug.Log($"{tool.gameObject.name} clicked OUT OF RANGE, is Euipment");
								}
								else if (IsPickupItemHit(clickOutOfRange, out PickupItem pickup))
								{
										Debug.Log($"{pickup.gameObject.name} clicked  OUT OF RANGE, is PickupItem");
								}
								else if (IsInteractibleHit(clickOutOfRange, out Interactible interact))
								{
										Debug.Log($"{interact.gameObject.name} clicked  OUT OF RANGE, is Interactible");
								}
						}
				}

				private static bool IsInteractibleHit(RaycastHit clickOutOfRange, out Interactible interactible)
				{
						return IsTargetType(clickOutOfRange, out interactible);
				}

				private static bool IsPickupItemHit(RaycastHit clickOutOfRange, out PickupItem item)
				{
						return IsTargetType(clickOutOfRange, out item);
				}

				private static bool IsEquimentHit(RaycastHit clickOutOfRange, out Equipment tool)
				{
						return IsTargetType(clickOutOfRange, out tool);
				}

				private static bool IsTargetType<T>(RaycastHit clickInRange, out T target)
						where T : Component
				{
						return clickInRange.collider.TryGetComponent(out target);
				}
		}
}