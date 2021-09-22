using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class DarknessInfo : MonoBehaviour, IDarknessInfo
		{
				private float litIntensity;
				private ItemHolder[] itemHolders;
				
				public float InDarknessTime { get; private set; }
				public float DarknessMultiplier { get; private set; }
				public bool IsInDarkness { get; private set; }

				private void Awake()
				{
						itemHolders = GetComponents<ItemHolder>();
				}

				private void LateUpdate()
				{
						IsInDarkness = true;
						if (LightInteractor.Lights.Count > 0)
						{
								float averageIntensity = litIntensity / LightInteractor.Lights.Count;
								if (averageIntensity >= 0.1f)
								{
										IsInDarkness = false;
								}

								// reset at end of frame!
								LightInteractor.ClearLights();
								litIntensity = 0;
						}

						if (IsInDarkness)
						{
								InDarknessTime += Time.deltaTime;
								float multiplier = GetLightSourceEquipmentMultiplier();
								DarknessMultiplier = Time.deltaTime * multiplier;
						}
				}

				private float GetLightSourceEquipmentMultiplier()
				{
						float? multiplier = null;
						foreach (ILightSource source in GetLightSourceEquipment())
						{
								if (IsActive() && IsBestMultiplier(multiplier, source))
								{
										multiplier = source.ActiveMultiplier;
								}

								bool IsActive() => (source?.IsPowered ?? false) is false;
						}
						return multiplier.GetValueOrDefault(1);

						static bool IsBestMultiplier(float? multiplier, ILightSource source)
						{
								return multiplier.HasValue is false || multiplier.Value < source.ActiveMultiplier;
						}
				}

				private IEnumerable<ILightSource> GetLightSourceEquipment()
				{
						return itemHolders.Where(holder => holder != null).Select(item => item.CurrentItem).OfType<ILightSource>();
				}

				public void InLightUpdate(float intensity)
				{
						this.litIntensity += intensity;
				}
		}
}