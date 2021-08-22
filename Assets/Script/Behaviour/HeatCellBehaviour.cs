using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class HeatCellBehaviour : MonoBehaviour, IHeatUp
		{
				public event Action<IHeatUp> HeatedUpEvent;
				public event Action<IHeatUp> CooledDownEvent;

				[SerializeField] private Material materialRef;
				[SerializeField]
				[Min(0)]
				private Vector2 randomHeatTimeSeconds = Vector2.one * (float)TimeSpan.FromMinutes(5).TotalSeconds;
				[SerializeField] [Min(0)] private Vector2 randomCoolDownSeconds = Vector2.one * (float)TimeSpan.FromMinutes(1).TotalSeconds;

				private float lifetime;
				private HeatMode mode = HeatMode.HEAT;
				private float heatTimeSeconds;
				private float coolDownSeconds;

				public bool FullyHeated => lifetime >= heatTimeSeconds;
				public bool FullyCooled => lifetime <= 0;

				private enum HeatMode
				{
						HEAT,
						COOL,
						HEATED,
						COOLED,
				}

				private void Start()
				{
						heatTimeSeconds = UnityEngine.Random.Range(randomHeatTimeSeconds.x, randomHeatTimeSeconds.y);
						coolDownSeconds = UnityEngine.Random.Range(randomCoolDownSeconds.x, randomCoolDownSeconds.y);
						materialRef = new Material(materialRef);
						SetHeatOnShader(0);
						HeatUp();
				}

				private void Update()
				{
						if (mode is HeatMode.COOL)
						{
								if (lifetime > 0)
								{
										lifetime -= Time.deltaTime;
										lifetime = Mathf.Max(0, lifetime);
								}
								else
								{
										mode = HeatMode.COOLED;
										OnFullCooled();
								}
						}
						else if (mode is HeatMode.HEAT)
						{
								if (lifetime < heatTimeSeconds)
								{
										lifetime += Time.deltaTime;
										lifetime = Mathf.Min(heatTimeSeconds, lifetime);
								}
								else
								{
										mode = HeatMode.HEATED;
										OnFullHeat();
								}
						}

						float heatProgress = (lifetime / heatTimeSeconds);
						SetHeatOnShader(heatProgress);
				}

				private void OnFullCooled()
				{
						CooledDownEvent?.Invoke(this);
				}

				public void HeatUp()
				{
						if (mode is HeatMode.HEAT || mode is HeatMode.HEATED)
						{
								return;
						}

						mode = HeatMode.HEAT;
						lifetime = 0;
				}

				public void CoolDown()
				{
						if (mode != HeatMode.HEAT || mode != HeatMode.HEATED)
						{
								return;
						}

						mode = HeatMode.COOL;
						lifetime = coolDownSeconds;
				}

				private void OnFullHeat()
				{
						HeatedUpEvent?.Invoke(this);
				}

				private void SetHeatOnShader(float heatValue)
				{
						const string nameId = "Heat_Current";
						materialRef.SetFloat(nameId, heatValue);
				}
		}
}