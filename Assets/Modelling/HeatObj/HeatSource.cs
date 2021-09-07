using System.Collections.Generic;

using UnityEngine;

namespace Assets.Modelling.HeatObj
{
		[RequireComponent(typeof(Light))]
		public class HeatSource : MonoBehaviour
		{
				public static bool INFRARED_VIEW_ACTIVE = false;
				public static readonly List<(Light light, int mask)> LIGHTS = new List<(Light light, int mask)>();
						
				[Tooltip("Value in °C")]
				[SerializeField] private float heatMaximum;
				
				private float intensityMaximum;
				private bool powered;
				private float progress;

				public Light Light { get; private set; }

				public float PoweredProgress => progress;

				/// <summary>
				/// You need to add environment temperature
				/// </summary>
				public float NormalizedTemperature => PoweredProgress * heatMaximum;

				private void Awake()
				{
						Light = GetComponent<Light>();
						LIGHTS.Add((Light, Light.renderingLayerMask));
						Debug.Log("Added HeatSource Light to list");
						Init();
				}

				private void Update()
				{
						powered = INFRARED_VIEW_ACTIVE;

						if (powered)
						{
								// 1s = Time.deltaTime
								Light.intensity = Mathf.Lerp(Light.intensity, intensityMaximum, Time.deltaTime);
						}
						else
						{
								// 5s
								float durationSeconds = progress * 5f;
								Light.intensity = Mathf.Lerp(Light.intensity, intensityMaximum, Time.deltaTime / durationSeconds);
						}
						progress = Light.intensity / intensityMaximum;
				}

				public void PowerUp()
				{
						powered = true;
				}

				public void PowerDown()
				{
						powered = false;
				}

				internal void Init()
				{
						this.intensityMaximum = Light.intensity;
				}
		}
}