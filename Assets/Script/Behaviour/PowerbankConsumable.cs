using Assets.Modelling.HeatObj;
using Assets.Script.Behaviour.FirstPerson;

using System.Collections;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class PowerbankConsumable : Consumable
		{
				[SerializeField] private string title = "Head Cell";
				[Min(1f)]
				[SerializeField] private int powerCapacity = 5 * 100;
				[Header("Visibility Fade")]
				[Min(0)]
				[SerializeField] private float visibleAtDistance = 8;
				[Min(0)]
				[SerializeField] private float hideAtDistance = 15;

				private HeatSource[] sources;
				private string messageNoValidActiveEquipment;
				private Coroutine draining_co;

				private Transform Transform { get; set; }
				private Material Material { get; set; }

				public float PowerProgress => sources.Average(source => source.PoweredProgress);

				public float NormalizedTemperature => sources.Average(sources => sources.NormalizedTemperature);

				protected virtual void Awake()
				{
						Transform = transform;
						Material = GetComponent<MeshRenderer>().material;
				}

				private void Start()
				{
						sources = GetComponentsInChildren<HeatSource>();
				}

				protected override void Update()
				{
						base.Update();

						FadeMaterialByDistance();
				}

				private void FadeMaterialByDistance()
				{
						var camera = CameraMoveType.Instance.GetCamera();
						if (camera is null)
								return;

						float maxDistanceOfView = camera.farClipPlane;
						float actualDistance = Vector3.Distance(camera.transform.position, Transform.position);
						float visibility;
						if (actualDistance <= visibleAtDistance)
						{
								visibility = 1;
						}
						else if (actualDistance >= hideAtDistance)
						{
								visibility = 0;
						}
						else if (actualDistance <= maxDistanceOfView)
						{
								visibility = 1 - (actualDistance / maxDistanceOfView);
						}
						else
						{
								visibility = 0;
						}
						Material.color = new Color(1, 1, 1, visibility);
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

				protected override void Interact(PlayerBehaviour sender)
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