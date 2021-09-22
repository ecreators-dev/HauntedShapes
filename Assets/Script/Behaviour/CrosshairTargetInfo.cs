﻿using Assets.Script.Behaviour.FirstPerson;
using Assets.Script.Components;

using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// With this component another component can check the target below the Crosshair (using Crosshair Singleton)
		/// </summary>
		[DisallowMultipleComponent]
		public class CrosshairTargetInfo : MonoBehaviour, ICrosshairInfo
		{
				private const int MAX_NUM_LAYERS_UNITY = 32;

				[Min(0)]
				[SerializeField] private float hitDistance = 4;
				[Min(0)]
				[SerializeField] private float hoverDistance = 5;
				[SerializeField] private LayerMask allowedLayers;
				[SerializeField] private LayerMask disallowedLayers;

				public (HitInfo InClickRange, HitInfo InHoverRange) Info { get; private set; }
				public (ObjectHitInfo InClickRange, ObjectHitInfo InHoverRange) ObjectInfo { get; private set; }

				private ISet<LayerMask> allowedLayersSet;
				private ISet<LayerMask> disallowedLayersSet;

				private void Start()
				{
						allowedLayersSet = GetLayerMasks(this.allowedLayers);
						disallowedLayersSet = GetLayerMasks(this.disallowedLayers);
				}

				private void Update()
				{
						CameraMoveType moveType = CameraMoveType.Instance;
						if (moveType == null) return;

						Camera cam = moveType.GetCamera();
						if (cam == null) return;
						ICrosshairUI crosshair = CrosshairHitVisual.Instance;
						if (crosshair == null) return;

						Info = crosshair.RaycastCollidersOnly(cam, allowedLayersSet, disallowedLayersSet, hitDistance, hoverDistance);
						ObjectInfo = (
								new ObjectHitInfo(Info.InClickRange.Target?.GetComponent<Interactible>()),
								new ObjectHitInfo(Info.InHoverRange.Target?.GetComponent<Interactible>())
								);
				}

				private ISet<LayerMask> GetLayerMasks(LayerMask layers)
				{
						ISet<LayerMask> result = new HashSet<LayerMask>();

						LayerMask remaining = layers;
						while (remaining > 0)
						{
								for (int layer = MAX_NUM_LAYERS_UNITY - 1; layer >= 0; layer--)
								{
										if (remaining == (remaining | (1 << layer)))
										{
												// debug:
												string name = LayerMask.LayerToName(layer);

												result.Add(layer);
												remaining.RemoveLayerMask(layer);
										}
								}
						}
						return result;
				}
		}
}