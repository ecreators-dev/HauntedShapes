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
						(bool match, IEquipment item, Component cmp) equipment = default;
						(bool match, IInteractible item, Component cmp) item = default;
						if (!Mouse.current.leftButton.isPressed)
						{
								HandleHover(camera, out equipment, out item);
						}

						// CLICK:
						if (Mouse.current.leftButton.isPressed)
						{
								// a) clicked near (match)
								// b) clicked far (hover) - hand with arraow up (shows one step further to pickup)
								HandleClicked(camera, equipment, item);
						}

						image.enabled = hoveredHit;
				}

				private void HandleHover(Transform camera, out (bool match, IEquipment item, Component cmp) equipment,
						out (bool match, IInteractible item, Component cmp) item)
				{
						equipment = default;
						item = default;

						// condition! 1st: match any, 2nd: match only types
						hoveredHit = Physics.Raycast(camera.position, camera.forward, out RaycastHit clickInRange, hitDistance)
								&& IsHit(out equipment, out item, clickInRange);

						static bool IsHit(out (bool isEquipment, IEquipment item, Component cmp) equipment,
								out (bool isAny, IInteractible item, Component cmp) item,
								RaycastHit clickInRange)
						{
								bool match = false;
								equipment = default;
								item = default;

								// hover only if not in hand!
								if (IsEquimentHit(clickInRange, out IEquipment tool, out Component euipmentComponent) && tool.IsEquipped is false)
								{
										equipment = (true, tool, euipmentComponent);
										match = true;
								}
								if (IsInteractibleHit(clickInRange, out IInteractible interactible, out Component anyInteractible) && interactible.IsPickable)
								{
										item = (true, interactible, anyInteractible);
										match = true;
								}
								return match;
						}
				}

				/// <summary>
				/// Called when MouseLeft down = pressed!
				///
				private void HandleClicked(Transform camera, (bool match, IEquipment item, Component cmp) equipment, (bool match, IInteractible item, Component cmp) item)
				{
						// out of range (hitDistanceFar), but in near reange AND clicked
						// hovered and clicked in range:
						if (hoveredHit)
						{
								var player = Camera.current.transform.GetComponentInParent<PlayerBehaviour>();
								if (player != null)
								{
										if (equipment.match)
										{
												player.Equip(equipment.item);
										}
										else if (item.match)
										{
												player.PickUp(item.item);
										}
								}
								else
								{
										Debug.LogError($"No {nameof(PlayerBehaviour)} found in parent of camera.current");
								}
						}
						// clicked and not hovered: check if in near range!
						else if (Physics.Raycast(camera.position, camera.forward, out RaycastHit clickOutOfRange, hitDistanceFar))
						{
								Component cmp;
								if (IsEquimentHit(clickOutOfRange, out IEquipment tool, out cmp) && tool.IsEquipped is false)
								{
										// TODO condition!
										Debug.Log($"{cmp.gameObject.name} clicked OUT OF RANGE, is Euipment");
								}
								else if (IsInteractibleHit(clickOutOfRange, out IInteractible pickup, out cmp) && pickup.IsPickable)
								{
										Debug.Log($"{cmp.gameObject.name} clicked  OUT OF RANGE, is Interactible");
								}
						}
				}

				private static bool IsInteractibleHit(RaycastHit clickOutOfRange, out IInteractible interactible, out Component cmp)
				{
						return IsTargetType(clickOutOfRange, out interactible, out cmp);
				}

				private static bool IsEquimentHit(RaycastHit clickOutOfRange, out IEquipment tool, out Component cmp)
				{
						return IsTargetType(clickOutOfRange, out tool, out cmp);
				}

				private static bool IsTargetType<T>(RaycastHit clickInRange, out T target, out Component component)
				{
						if (clickInRange.collider.TryGetComponent(out target) && target is Component cmp)
						{
								component = cmp;
								return true;
						}
						component = null;
						return false;
				}
		}
}