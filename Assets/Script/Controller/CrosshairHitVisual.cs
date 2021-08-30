using Assets.Script.Components;
using Assets.Script.Controller;
using Assets.Script.Model;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Attached to an Canvas/RawImage (Hand)
		/// </summary>
		[RequireComponent(typeof(RawImage))]
		[DisallowMultipleComponent]
		public class CrosshairHitVisual : MonoBehaviour
		{
				[SerializeField] private CrosshairRoot root;
				[SerializeField] private bool hoveredHit;
				[SerializeField] private float hitDistance = 4;
				[SerializeField] private float hitDistanceFar = 5;
				[SerializeField] private LayerMask interactibleLayer;
				[Range(0.0001f, 0.2f)]
				[SerializeField] private float size = 0.02f;
				[SerializeField] private TMP_Text targetTextUI;
				[SerializeField] private TMP_Text tooFarTextUI;

				private RawImage image;
				private Color matchColor;
				private bool clickableRange;
				private Equipment hitEquipment;
				private PickupItem hitItem;
				private Interactible hitAny;

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
						HandleHover(camera, out hitEquipment, out hitItem, out hitAny);

						// CLICK:
						if (IsInteractionPressed())
						{
								// show only when want to interact
								tooFarTextUI.enabled = hoveredHit && !clickableRange;
						}

						image.enabled = hoveredHit;

						//! Click Action is handled in Player Behaviour
				}

				private bool IsInteractionPressed()
				{
						return Mouse.current.leftButton.isPressed
														|| this.InputControls().InteractionCrosshairPressed;
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

						targetTextUI.enabled = hoveredHit;
						if (hoveredHit)
						{
								targetTextUI.text = any.GetTargetName();
								targetTextUI.color = matchColor;
						}

						bool IsHit(out Equipment equipment,
								out PickupItem item,
								out Interactible any,
								RaycastHit clickInRange)
						{
								bool match = false;

								// hover only if not in hand!
								if (IsEquimentHit(clickInRange, out equipment)
										| IsPickupItemHit(clickInRange, out item)
										| IsInteractibleHit(clickInRange, out any))
								{
										matchColor = root.GetColor(GetActionType(equipment, any));
										match = true;
								}
								clickableRange = match && clickInRange.distance <= hitDistance;
								return match;
						}
				}

				private CrosshairRoot.ActionEnum GetActionType(Equipment equipment, Interactible any)
				{
						if (any is { })
						{
								if (any.IsLocked)
								{
										return CrosshairRoot.ActionEnum.LOCKED;
								}
								else if (equipment is { })
								{
										if (equipment.IsBroken)
										{
												return CrosshairRoot.ActionEnum.BROKEN;
										}
								}
								return CrosshairRoot.ActionEnum.INTERACTIBLE;
						}
						return default;
				}

				private bool IsInteractibleHit(RaycastHit clickOutOfRange, out Interactible interactible)
				{
						bool result = IsTargetType(clickOutOfRange, out interactible);
						hitAny = result ? interactible : default;
						return result;
				}

				private bool IsPickupItemHit(RaycastHit clickOutOfRange, out PickupItem item)
				{
						bool result = IsTargetType(clickOutOfRange, out item);
						hitItem = result ? item : default;
						return result;
				}

				private bool IsEquimentHit(RaycastHit clickOutOfRange, out Equipment tool)
				{
						bool result = IsTargetType(clickOutOfRange, out tool);
						hitEquipment = result ? tool : default;
						return result;
				}

				private static bool IsTargetType<T>(RaycastHit clickInRange, out T target)
						where T : Component
				{
						return clickInRange.collider.TryGetComponent(out target);
				}

				public bool TryGetObject(out bool inRange, out (Equipment equipment, PickupItem item, Interactible any) result)
				{
						result = (hitEquipment, hitItem, hitAny);
						inRange = clickableRange;
						return hoveredHit;
				}
		}
}