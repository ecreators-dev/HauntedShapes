using Assets.Script.Behaviour.FirstPerson;
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
		public class CrosshairHitVisual : MonoBehaviour, ICrosshairUI
		{
				[SerializeField] private CrosshairRoot root;
				[SerializeField] private bool hovered;
				[SerializeField] private float hitDistance = 4;
				[SerializeField] private float hitDistanceFar = 5;

				[SerializeField] private LayerMask interactibleLayer;
				[Range(0.0001f, 0.2f)]
				[SerializeField] private float size = 0.02f;
				[SerializeField] private TMP_Text targetTextUI;
				[SerializeField] private TMP_Text tooFarTextUI;
				[SerializeField] private LayerMask hitLayers;

				private RawImage image;
				private Color matchColor;
				private bool clickableRange;
				private Equipment hitEquipment;
				private PickupItem hitItem;
				private Interactible hitAny;
				private RaycastHit anyTarget;
				private bool anyTargetHit;
				private Vector3 hitBefore;

				public static ICrosshairUI Instance { get; private set; }

				public void SetHitActive() => hovered = true;

				public void SetHitInactive() => hovered = false;

				private void Awake()
				{
						image = GetComponent<RawImage>();
				}

				private void Start()
				{
						if (Instance == null)
						{
								Instance = this;
						}
						else
						{
								DestroyImmediate(this);
								Debug.LogError($"Duplicated Instance of {nameof(CrosshairHitVisual)}");
						}
				}

				private void Update()
				{
						Transform camera = CameraMoveType.Instance.GetCamera().transform;

						// HOVER: show near items with hand visible! (c)
						hovered = false;
						UpdateHoveredTarget(camera);

						// CLICK:
						tooFarTextUI.enabled = false;
						if (IsInteractionPressed())
						{
								// show only when want to interact
								tooFarTextUI.enabled = hovered && !clickableRange;
						}

						image.enabled = hovered;

						//! Click Action is handled in Player Behaviour
				}

				public (bool actualHit, RaycastHit hit) GetHitPointLastUpdate()
				{
						return (anyTargetHit, anyTarget);
				}

				public (bool hit, Vector3 point) UpdateHitPointFarAway(Camera cam)
				{
						// layerMask = cam.cullingMask means:
						// takes only visible targets in view, not player (for example)
						Transform camera = cam.transform;
						Ray ray = new Ray(camera.position, camera.forward);
						anyTargetHit = Physics.SphereCast(ray, size,
								out anyTarget, 10000, hitLayers, 
								// fixes hit no trigger!
								QueryTriggerInteraction.Ignore);
						var point = anyTarget.point;
						if (anyTargetHit is false)
						{
								point = hitBefore;
						}
						else
						{
								hitBefore = point;
						}
						return (anyTargetHit, point);
				}

				private bool IsInteractionPressed()
				{
						return Mouse.current.leftButton.isPressed
								|| this.InputControls().InteractionCrosshairPressed;
				}

				private void UpdateHoveredTarget(Transform camera)
				{
						hitEquipment = default;
						hitItem = default;
						hitAny = default;

						// condition! 1st: match any, 2nd: match only types
						hovered =
								Physics.SphereCast(camera.position, size, camera.forward, out RaycastHit clickInRange, hitDistance, interactibleLayer)
								&& (IsEquimentHit(clickInRange, out hitEquipment)
										| IsPickupItemHit(clickInRange, out hitItem)
										| IsInteractibleHit(clickInRange, out hitAny));

						clickableRange = hovered && clickInRange.distance <= hitDistance;
						matchColor = root.GetColor(GetActionType(hitEquipment, hitAny));
						targetTextUI.enabled = hovered;
						tooFarTextUI.enabled = false;

						if (hovered)
						{
								targetTextUI.text = $"{hitAny.GetTargetName()} in {clickInRange.distance:0}m";
								targetTextUI.color = matchColor;
								tooFarTextUI.enabled = clickInRange.distance <= hitDistance;
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
						return IsTargetType(clickOutOfRange, out interactible);
				}

				private bool IsPickupItemHit(RaycastHit clickOutOfRange, out PickupItem item)
				{
						return IsTargetType(clickOutOfRange, out item);
				}

				private bool IsEquimentHit(RaycastHit clickOutOfRange, out Equipment tool)
				{
						return IsTargetType(clickOutOfRange, out tool);
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
						return hovered;
				}
		}
}