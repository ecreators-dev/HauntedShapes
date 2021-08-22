using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class PforteTriggerBehaviour : MonoBehaviour
		{
				private readonly Dictionary<PforteTriggerTypeEnum, int> amountsOfType = new Dictionary<PforteTriggerTypeEnum, int>();

				[SerializeField] private Collider triggerCollider;
				[SerializeField] private PasswayUpDown gate;
				[SerializeField] private PforteCondition[] conditions;
				[Header("Debug")]
				[SerializeField] private bool reactOnClickDebug;
				private Collider Collider { get; set; }

				private IGate Gate => gate;

				private void Awake()
				{
						Collider = triggerCollider;
						Collider.enabled = true;
						Collider.isTrigger = true;
				}

				private void Start()
				{
						if (triggerCollider.TryGetComponent(out MeshRenderer meshRenderer))
						{
								meshRenderer.enabled = false;
								Debug.Log($"{gameObject.name} disabled MeshRenderer for trigger");
						}
				}

				private void OnMouseDown()
				{
						if (reactOnClickDebug)
						{
								Debug.Log("Open Pforte");
								OnReadyToOpen();
						}
						else
						{
								Debug.Log("Pforte.Click = OFF (debug)");
						}
				}

				private void OnTriggerExit(Collider other)
				{
						// a unit has been taken away again
						if (other.TryGetComponent<IPforteTrigger>(out var trigger))
						{
								amountsOfType.Decrement(trigger.TriggerType);

								// no sound play caused by take-away
						}
				}

				private void OnTriggerEnter(Collider other)
				{
						// a unity was placed inside the ranges
						if (other.TryGetComponent<IPforteTrigger>(out var trigger))
						{
								int amountNew = amountsOfType.TryGetValue(trigger.TriggerType, out int amountCurrent) ? amountCurrent + 1 : 0;

								foreach (PforteCondition condition in conditions)
								{
										if (condition.IsFullfilled(trigger, amountNew, out bool matchType))
										{
												amountsOfType.Increment(trigger.TriggerType);
												OnTriggerGranted(trigger.TriggerType);
												break;
										}
										else if (matchType)
										{
												amountsOfType.Increment(trigger.TriggerType);
												OnTriggerValid(trigger.TriggerType);
												break;
										}
								}
						}
				}

				private void OnTriggerValid(PforteTriggerTypeEnum triggerType)
				{
						// play sound?
				}

				private void OnTriggerGranted(PforteTriggerTypeEnum triggerType)
				{
						// es war eine idee, dass die gegenst�nde hingestellt weden m�ssen
						// aber warum nicht erlauben, sie einfach nur zu halten - macht
						// doch keinen Unteschied - ist f�r die gruppe nur schwieriger,
						// da alle innerhalb des kreises bleiben m�ssen und vor allem daher
						// schwieriger, weil es keinen ton gibt, wenn man einen gegenstand
						// aus der reichweite entfernt, sondern nur beim hineinbringen.
						// daher wird es darauf hinauslaufen, dass die gegenst�nde hingestellt
						// werden

						if (CheckReadyToOpen())
						{
								OnReadyToOpen();
						}
						else
						{
								// play ready sound
								PlayGrantedTypeSound(triggerType);
						}
				}

				private bool CheckReadyToOpen()
				{
						bool readyToOpen = true;
						foreach (PforteCondition condition in conditions)
						{
								if (amountsOfType.TryGetValue(condition.TriggerType, out int amountCurrent) is false
										|| amountCurrent < condition.RequiredCount)
								{
										readyToOpen = false;
										break;
								}
						}

						return readyToOpen;
				}

				private void PlayGrantedTypeSound(PforteTriggerTypeEnum triggerType)
				{
						// TODO
				}

				private void OnReadyToOpen()
				{
						// eine idee ist es, dass man, wenn alle gegenst�nde vorhanden sind
						// ein einfaches wort oder ein spruch gesagt werden muss,
						// der dann das tor �ffnet

						// TODO register word or phrase in speech recognition - if said, open

						// version 1.0 automatisch
						Gate.Open();
				}
		}
}