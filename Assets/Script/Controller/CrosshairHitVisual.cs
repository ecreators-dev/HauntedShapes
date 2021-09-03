using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Components;
using Assets.Script.Controller;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using static Assets.Script.Behaviour.SurfacePlacingFactory;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Attached to an Canvas/RawImage (Hand)
		/// </summary>
		[RequireComponent(typeof(RawImage))]
		[DisallowMultipleComponent]
		public class CrosshairHitVisual : MonoBehaviour, ICrosshairUI
		{
				private static readonly LayerMask LAYER_MASK_EVERYTHING = ~0;

				[SerializeField] private CrosshairRoot root;
				[SerializeField] private bool hovered;
				private bool placementHitFound;
				[SerializeField] private float hitDistance = 4;
				[SerializeField] private float hitDistanceFar = 5;

				[SerializeField] private LayerMask interactibleLayer;
				[Range(0.0001f, 0.2f)]
				[SerializeField] private float size = 0.02f;
				[SerializeField] private TMP_Text targetTextUI;
				[SerializeField] private TMP_Text tooFarTextUI;
				[SerializeField] private LayerMask hitLayers;
				[SerializeField] private GameObject placementSprite;
				[SerializeField] private PlacementCheck placementCheck;

				private RawImage image;
				private Color matchColor;
				private bool clickableRange;
				private Equipment hitEquipment;
				private PickupItem hitItem;
				private Interactible hitAny;
				private RaycastHit anyTarget;
				private bool anyTargetHit;
				private Vector3 hitBefore;
				private Equipment placementRequestor;
				private RaycastHit placementHit;

				public static ICrosshairUI Instance { get; private set; }

				public void SetHitActive() => hovered = true;

				public void SetHitInactive() => hovered = false;

				private void Awake()
				{
						image = GetComponent<RawImage>();
				}

				private void Start()
				{
						placementSprite.SetActive(false);
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

				/// <summary>
				/// See also <seealso cref="PlaceEquipment"/>
				/// </summary>
				public void ShowPlacementPointer(Equipment equipmentNotNull)
				{
						if (placementRequestor != null && placementRequestor != equipmentNotNull)
						{
								Debug.LogError($"You must call {nameof(PlaceEquipment)} from the original quipment first");
								return;
						}
						if (equipmentNotNull == null)
						{
								Debug.LogError($"Only allowed for equipments not null!");
								return;
						}
						placementRequestor = equipmentNotNull;
						//placing = SurfacePlacingFactory.FindSurface(showPlacementForEquipment.transform, Vector3.up);
				}

				/// <summary>
				/// See also <seealso cref="ShowPlacementPointer"/>
				/// </summary>
				public void PlaceEquipment(Equipment equipment, Vector3 up, bool useHitNormal)
				{
						if (placementRequestor == null || placementRequestor != equipment)
						{
								Debug.LogError("You must call this from the quipment, when placed or aborted");
								return;
						}

						equipment.transform.SetParent(null);
						equipment.transform.position = placementHit.point;
						if (useHitNormal)
						{
								equipment.transform.rotation = Quaternion.FromToRotation(up, placementHit.normal);
						}
						placementRequestor = null;
				}

				public PlacementEnum GetPlacementInfo(out PlacementCheck.HitCheck? info)
				{
						return placementCheck.GetPlacementType(out info);
				}

				public Transform GetPlacementPosition()
				{
						return placementSprite.transform;
				}

				public Vector3 GetPlacementNormal()
				{
						return placementHitFound ? placementHit.normal : Vector3.up;
				}

				private void Update()
				{
						Camera cam = CameraMoveType.Instance.GetCamera();
						Transform camera = cam.transform;

						// HOVER: show near items with hand visible! (c)
						hovered = false;
						UpdateHoveredTarget(camera);
						UpdatePlacementTarget();

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

				private void UpdatePlacementTarget()
				{
						// now placed: reset (only placeable equipments can be placed)
						if (placementRequestor != null && placementRequestor is IPlacableEquipment p
								&& p.IsPlaced)
						{
								placementRequestor = null;
						}

						// show or hide
						bool hoveredOnEnvironment = placementRequestor != null && placementHitFound;
						if (hoveredOnEnvironment)
						{
								// put sprite to wall:
								PlacementVisualUpdatePosition();
						}
						placementSprite.SetActive(hoveredOnEnvironment);
				}

				private void PlacementVisualUpdatePosition()
				{
						if (placementHitFound is false)
								return;

						Vector3 normal = placementHit.normal;
						Vector3 position = placementHit.point;
						placementSprite.transform.forward = normal;
						placementSprite.transform.position = position + placementSprite.transform.forward * 0.01f;
				}

				public (bool actualHit, RaycastHit hit) GetRaycastCollidersOnlyResult()
				{
						return (anyTargetHit, anyTarget);
				}

				public (bool hit, Vector3 point, Vector3 normal) RaycastCollidersOnly(Camera sourceCamera)
				{
						// layerMask = cam.cullingMask means:
						// takes only visible targets in view, not player (for example)
						Transform camera = sourceCamera.transform;
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
						return (anyTargetHit, point, anyTarget.normal);
				}

				private bool IsInteractionPressed()
				{
						return Mouse.current.leftButton.isPressed
								|| this.InputControls().CrosshairTargetInteractionButtonPressed;
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

						// place anywhere:
						placementHitFound =
								Physics.Raycast(new Ray(camera.position, camera.forward),
								out placementHit, hitDistance, LAYER_MASK_EVERYTHING, QueryTriggerInteraction.Ignore);

						// place not on other interactibles!
						if (placementHit.collider != null &&
								placementHit.collider.GetComponent<Interactible>() != null)
						{
								placementHitFound = false;
								placementHit = default;
						}

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

				public bool TryGetItem(out bool inRange, out (Equipment equipment, PickupItem item, Interactible any) result)
				{
						result = (hitEquipment, hitItem, hitAny);
						inRange = clickableRange;
						return hovered;
				}
		}
}