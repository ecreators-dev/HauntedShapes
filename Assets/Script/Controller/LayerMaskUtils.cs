using System.Collections.Generic;

using UnityEngine;

using System.Linq;

namespace Assets.Script.Behaviour
{
		public static class LayerMaskUtils
		{
				public const int EVERY_LAYER_MASK = ~0;
				public static readonly LayerMask EVERY_LAYER = ~0;

				public static LayerMask CombineLayerMasks(ISet<LayerMask> match, ISet<LayerMask> avoid)
				{
						List<LayerMask> hitMasks = match?.ToList();
						List<LayerMask> avoidMasks = avoid?.ToList();

						LayerMask result = EVERY_LAYER_MASK;

						if (avoidMasks != null)
						{
								hitMasks?.ForEach(item => avoidMasks.Remove(item));
								result = EVERY_LAYER_MASK;
								foreach (LayerMask item in avoidMasks)
								{
										result.RemoveLayerMask(item);
								}
						}

						if (hitMasks != null)
						{
								avoidMasks?.ForEach(item => hitMasks.Remove(item));
								bool isFirst = true;
								foreach (LayerMask item in hitMasks)
								{
										int value = item.value;
										if (isFirst)
										{
												result = item;
										}
										else
										{
												result.AddLayerMask(item);
										}
										if (isFirst) isFirst = false;
								}
						}

						return result;
				}

				public static void RemoveLayerMask(this ref LayerMask result, LayerMask item)
				{
						// https://forum.unity.com/threads/how-to-remove-a-single-layer-from-cullingmask.410820/
						result &= ~(1 << item);
				}

				public static void AddLayerMask(this ref LayerMask result, LayerMask item)
				{
						// https://forum.unity.com/threads/how-to-remove-a-single-layer-from-cullingmask.410820/
						result |= 1 << item;
				}
		}
}