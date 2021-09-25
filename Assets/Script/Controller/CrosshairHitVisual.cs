using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Components;
using Assets.Script.Controller;

using System;
using System.Collections.Generic;

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
				[Range(0.0001f, 0.2f)]
				[SerializeField] private float size = 0.02f;
				[Range(0.0001f, 0.2f)]
				[SerializeField] private float sizeGamepad = 0.04f;
				[SerializeField] private TMP_Text targetTextUI;
				[SerializeField] private TMP_Text tooFarTextUI;
				[SerializeField] private GameObject placementSprite;
				[SerializeField] private Transform crosshairDot;
				[SerializeField] private LayerMask layersHit;
				[Min(0)]
				[SerializeField] private float hitDistance = 6;
				[Min(0)]
				[SerializeField] private float hoverDistance = 400;

				private RawImage image;

				private RectTransform Transform { get; set; }

				private float initSize;
				private float initScaleW;
				private float initScaleH;
				private Vector3 hitPointBefore;
				public static ICrosshairUI Instance { get; private set; }

				public (HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) RaycastInfo { get; private set; }
				public (ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) RaycastObjectInfo { get; private set; }

				public bool IsHovered { get; private set; }

				public bool IsGamepadConnected { get; private set; }

				private void Awake()
				{
						Instance = this;
						image = GetComponent<RawImage>();
						Transform = transform as RectTransform;
				}

				private void Start()
				{
						initSize = size;
						initScaleW = Transform.sizeDelta.x;
						initScaleH = Transform.sizeDelta.y;
						HideTargetOnce();
				}

				private void FixedUpdate()
				{
						IsGamepadConnected = !this.GetGameController().IsGamepadDisconnected;
						size = IsGamepadConnected ? sizeGamepad : initSize;
						float scale = (size / 0.02f);
						crosshairDot.localScale = Vector3.one * scale;
						//Transform.sizeDelta = new Vector2(initScaleW, initScaleH) * scale;

						(ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) objectInfo;

						LayerMaskFilter layers = new LayerMaskFilter();
						layers.Includers.Clear();
						layers.Includers.Add(layersHit);

						RangeFilter range = new RangeFilter
						{
								ClickRange = hitDistance,
								HoverRange = hoverDistance
						};

						RaycastInfo = CustomRaycastIgnoreTriggers(CameraMoveType.Instance.GetCamera(),
								layers, range,
								out objectInfo);

						RaycastObjectInfo = objectInfo;

						IsHovered = RaycastInfo.HoverRange.IsHit;

						UpdateGui();
				}

				public (HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) CustomRaycastIgnoreTriggers(Camera sourceCamera,
						LayerMaskFilter layers,
						RangeFilter range,
						out (ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) objectInfo)
				{
						// layerMask = cam.cullingMask means:
						// takes only visible targets in view, not player (for example)
						Transform camera = sourceCamera.transform;
						Ray ray = new Ray(camera.position, camera.forward);

						int masks = LayerMaskUtils.CombineLayerMasks(layers.Includers, layers.Excluders);

						bool anyTargetHit = Physics.SphereCast(ray, size,
								out RaycastHit anyTarget, range.ClickRange, masks,
								// fixes hit no trigger!
								QueryTriggerInteraction.Ignore);

						bool anyTargetHover = Physics.SphereCast(ray, size,
								out RaycastHit anyHover, range.HoverRange, masks,
								// fixes hit no trigger!
								QueryTriggerInteraction.Ignore);

						if (anyTargetHit)
						{
								hitPointBefore = anyTarget.point;
						}

						(HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) info = GetRaycastInfo(anyTargetHit, anyTarget, anyTargetHover, anyHover);
						objectInfo = GetRaycastObjectInfo(info);

						return info;
				}

				private static (ObjectHitInfo ClickRange, ObjectHitInfo HoverRange) GetRaycastObjectInfo((HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) info)
				{
						return (
														ClickRange: new ObjectHitInfo(info.ClickRange.Target?.GetComponent<Interactible>(), info.ClickRange.Target),
														HoverRange: new ObjectHitInfo(info.HoverRange.Target?.GetComponent<Interactible>(), info.HoverRange.Target)
														);
				}

				private (HitSurfaceInfo ClickRange, HitSurfaceInfo HoverRange, bool IsHitAny) GetRaycastInfo(bool anyTargetHit, RaycastHit anyTarget, bool anyTargetHover, RaycastHit anyHover)
				{
						return (ClickRange:
										new HitSurfaceInfo(anyTargetHit, anyTargetHit ? anyTarget.point : hitPointBefore, anyTarget.normal,
										anyTargetHit ? anyTarget.collider.gameObject : null),
										HoverRange:
										new HitSurfaceInfo(anyTargetHover, anyTargetHover ? anyHover.point : hitPointBefore,
										anyHover.normal,
										anyTargetHover ? anyHover.collider.gameObject : null),
										IsHitAny: anyTargetHit || anyTargetHover
										);
				}

				private void UpdateGui()
				{
						// only visible in click range!
						image.enabled = !tooFarTextUI.enabled && IsHovered;
						// visible in hover range
						targetTextUI.enabled = IsHovered;
						// visible in hover range when targeting an object
						tooFarTextUI.enabled = CanSeeTextTooFar();

						if (IsHovered)
						{
								targetTextUI.text = GetTargetName();
								targetTextUI.color = root.GetColor(GetTargetType());
						}

						string GetTargetName()
						{
								return RaycastObjectInfo.HoverRange.GetInteractible()?.GetTargetName();
						}

						CrosshairRoot.ActionEnum GetTargetType()
						{
								if (RaycastInfo.ClickRange.IsHit)
								{
										ObjectHitInfo clickable = RaycastObjectInfo.ClickRange;
										if (clickable.GetInteractible()?.IsLocked ?? false)
										{
												return CrosshairRoot.ActionEnum.LOCKED;
										}
										if (clickable.GetEquipment()?.IsBroken ?? false)
										{
												return CrosshairRoot.ActionEnum.BROKEN;
										}
								}

								return CrosshairRoot.ActionEnum.INTERACTIBLE;
						}
				}

				private bool CanSeeTextTooFar()
				{
						return IsHovered && RaycastInfo.ClickRange.IsHit is false && RaycastObjectInfo.HoverRange.HasTargetItem;
				}

				// from update!
				public void ShowTargetPosition(IPlacableEquipment placable)
				{
						if (RaycastInfo.IsHitAny)
						{
								Transform visual = placementSprite.transform;
								Vector3 position = RaycastInfo.ClickRange.HitPoint;
								Vector3 spriteNormal = visual.forward;
								visual.position = position + spriteNormal * this.GetGameController().Crosshair.PlacementOffsetNormal;
								visual.rotation = Quaternion.FromToRotation(spriteNormal, RaycastInfo.ClickRange.Normal);
								placementSprite.gameObject.SetActive(true);

								// if not null: make a copy of it an show it at target position
								if (placable != null)
								{
										var copy = ShowAtTarget(placable);
										copy.position = RaycastInfo.ClickRange.HitPoint;
										copy.rotation = Quaternion.FromToRotation(placable.NormalUp, RaycastInfo.ClickRange.Normal);
								}
						}
						else
						{
								HideTargetOnce();
						}
				}

				private int copyInstanceFrom;
				private Transform copy;

				private Transform ShowAtTarget(IPlacableEquipment placable)
				{
						MonoBehaviour behaviour = placable as MonoBehaviour;
						if (behaviour == null)
								return null;

						if (copyInstanceFrom == behaviour.GetInstanceID())
						{
								return copy;
						}

						if (copy != null)
						{
								DestroyCopy();
						}

						copyInstanceFrom = behaviour.GetInstanceID();
						var clone = Instantiate(GetBlueprint(placable, behaviour), null);
						copy = clone.transform;
						var copyCmp = copy.GetComponent<IPlacableEquipment>();
						copyCmp.StartPreviewPlacement(placable);

						/* only with parent != null!
						// show in size of original
						float scaleReverse = 1 / placementSprite.transform.lossyScale.x;
						copy.localScale = behaviour.transform.lossyScale * scaleReverse;
						*/

						return copy;

						static GameObject GetBlueprint(IPlacableEquipment placable, MonoBehaviour behaviour)
						{
								GameObject source = placable.GetPlacingPrefab();
								return source == null ? behaviour.gameObject : source;
						}
				}

				public void HideTargetOnce()
				{
						placementSprite.gameObject.SetActive(false);
						if (copy != null)
						{
								DestroyCopy();
						}
				}

				private void DestroyCopy()
				{
						copyInstanceFrom = 0;
						Destroy(copy.gameObject);
				}
		}
}