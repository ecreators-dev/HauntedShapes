using Assets.Script.Behaviour.FirstPerson;
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
				[SerializeField] private float hoverDistance = 40;
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

						var info = crosshair.RaycastCollidersOnly(cam, allowedLayersSet, disallowedLayersSet, hitDistance, hoverDistance);
						var objectInfo = (
								ClickRange: new ObjectHitInfo(Info.InClickRange.Target?.GetComponent<Interactible>()),
								HoverRange: new ObjectHitInfo(Info.InHoverRange.Target?.GetComponent<Interactible>())
								);

						if (objectInfo.ClickRange.HasTargetItem != ObjectInfo.InClickRange.HasTargetItem)
						{
								Debug.Log($"Crosshair: changed! has target = {objectInfo.ClickRange.HasTargetItem}");
						}
						if (objectInfo.ClickRange.HasTargetItem && objectInfo.ClickRange.TargetItem != ObjectInfo.InClickRange.TargetItem)
						{
								Debug.Log($"Crosshair: changed! target = {objectInfo.ClickRange.TargetItem.GetTargetName()}");
						}

						Info = info;
						ObjectInfo = objectInfo;
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