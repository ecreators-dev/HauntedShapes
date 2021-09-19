using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Script.Behaviour
{
		[ExecuteInEditMode]
		public class CorridorLampSynchronizerOnlyEditor : MonoBehaviour
		{
#if UNITY_EDITOR
				[SerializeField] Light Light;

				private static float? allLumen;
				[Range(0.0001f, 10000)]
				[SerializeField] private float allLightsIntensity;
				private HDAdditionalLightData lightData;
				private float oldIntensity;

				private void Update()
				{
						if (Application.isPlaying is false)
						{
								lightData ??= Light.GetComponent<HDAdditionalLightData>();

								if (allLightsIntensity <= 0)
								{
										allLightsIntensity = lightData.intensity;
										allLumen = lightData.intensity;
								}

								// leader!
								if (allLightsIntensity != oldIntensity)
								{
										// update
										allLumen = allLightsIntensity;

										// show this
										AcceptLumen();

										//end
										oldIntensity = allLightsIntensity;
								}
								// any else
								else if (allLumen.HasValue)
								{
										// show this (next frame) and other same frame, when after updated value
										AcceptLumen();
								}
						}
				}

				private void AcceptLumen()
				{
						lightData.SetIntensity(allLumen.GetValueOrDefault(500), LightUnit.Lumen);
						Light.SetLightDirty();
				}
#endif
		}
}