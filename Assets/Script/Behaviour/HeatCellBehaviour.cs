using Assets.Modelling.HeatObj;

using System;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class HeatCellBehaviour : MonoBehaviour
		{
				[Range(0f, 1f)]
				[SerializeField] private float power = 0;

				private HeatSource[] sources;

				private Transform Transform { get; set; }

				public float PowerProgress => sources.Average(source => source.PoweredProgress);

				public float NormalizedTemperature => sources.Average(sources => sources.NormalizedTemperature);

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						sources = GetComponentsInChildren<HeatSource>();
				}

				public void CoolDown()
				{
						foreach (HeatSource source in sources)
						{
								source.PowerDown();
						}
				}

				public void HeatUp()
				{
						foreach (HeatSource source in sources)
						{
								source.PowerUp();
						}
				}
		}
}