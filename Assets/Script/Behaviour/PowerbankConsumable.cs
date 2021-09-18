using Assets.Modelling.HeatObj;

using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Script.Behaviour
{
		public class PowerbankConsumable : Consumable
		{
				[SerializeField] private string title = "Head Cell";
				[Min(1f)]
				[SerializeField] private int powerCapacity = 5 * 100;

				private HeatSource[] sources;
				private string messageNoValidActiveEquipment;
				private Coroutine draining_co;

				private Transform Transform { get; set; }

				public float PowerProgress => sources.Average(source => source.PoweredProgress);

				public float NormalizedTemperature => sources.Average(sources => sources.NormalizedTemperature);

				protected override void Awake()
				{
						Transform = transform;
						base.Awake();
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

				public override void Interact(PlayerBehaviour sender)
				{
						if (sender.ActiveEquipment is IPowerbankSupport pb)
						{
								float capacity = this.powerCapacity;
								if (CheckCanLoad(pb))
								{
										int requiredPower = pb.MaxPower - pb.Power;
										float powerToLoad = Mathf.Min(capacity, requiredPower);
										int powerLoad = Mathf.FloorToInt(powerToLoad);
										if (pb.LoadPower(powerLoad))
										{
												draining_co = StartCoroutine(RunLoading());

												IEnumerator RunLoading()
												{
														float load = 0;
														WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
														while (load < powerLoad)
														{
																pb.VisualizePowerLoadUpdate(load, powerLoad);
																load = Mathf.Lerp(load, powerLoad, Time.deltaTime * 2);
																yield return waitForEndOfFrame;
														}

														this.powerCapacity -= powerLoad;
														if (this.powerCapacity <= 0)
														{
																OnLoadEmpty();
																Debug.Log($"{GetTargetName()} destroyed. Capacity empty. (OK)");
														}
														draining_co = null;
														yield break;
												}
										}
								}
						}
						else
						{
								sender.AddMessage(messageNoValidActiveEquipment);
						}
				}

				private void OnLoadEmpty()
				{
						// after drained fully ... animation?

						// destroy parent:
						if (Transform.parent.gameObject == null)
						{
								Debug.LogError("Missing parent object in powerbank");
								Destroy(gameObject);
						}
						else
						{
								Destroy(Transform.parent.gameObject);
						}
				}

				private bool CheckCanLoad(IPowerbankSupport pb) => pb.Power < pb.MaxPower && draining_co is null;

				public override string GetTargetName() => $"{title}: {powerCapacity}";
		}
}