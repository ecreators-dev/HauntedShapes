using System;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		public class GhostEntity : MonoBehaviour
		{
				public GhostPropertyAsset properties;

				[Range(0, 10)]
				private int nameTriggerCounter;
				[Min(0)]
				private float nameTriggerTimestamp;
				private int shockEventCounter;
				private float shockEventTimestamp;
				private bool shockEventEnabled;

				private void Start()
				{
						properties = Instantiate(properties);
						properties.Generate();
						properties.CheckScenePhysicalEvidence();
				}

				private void Update()
				{
						HandleNameTriggerCounterDecrease();
						HandleShockingIncrease();
						HandleShockingEventEnable();
				}

				private void HandleShockingEventEnable()
				{
						// wenn die geistige Gesundheit in bestimmtes Maß unterschreitet
						// TODO - finde alle Spieler
				}

				public void CopyPersonality(PlayerBehaviour playerBehaviour)
				{
						this.properties = Instantiate(Resources.FindObjectsOfTypeAll<GhostPropertyAsset>().First());
						this.properties.Generate();
						this.properties.CopyPersonality(playerBehaviour);
				}

				private void HandleShockingIncrease()
				{
						// maximal 3 mal hintereinander darf ein Geist ein Schockerlebnis für die Spieler
						// auslösen. Alls 30 Sekunden bekommt der Geist eine Ladung wieder zurück.
						if (shockEventCounter < 3 && Time.timeSinceLevelLoad - shockEventTimestamp > 30)
						{
								// darf nur geladen werden, wenn der Moment es zu lässt
								if (shockEventEnabled)
								{
										shockEventTimestamp = Time.timeSinceLevelLoad;
										shockEventCounter++;
								}
						}
				}

				private void HandleNameTriggerCounterDecrease()
				{
						// wenn für mehr als 60 Sekunden der Name nicht getriggert wurde,
						// reduziert sich der counter wieder: der geister beruhigt sich langsam
						if (nameTriggerCounter > 0 && Time.timeSinceLevelLoad - nameTriggerTimestamp > 60)
						{
								// in weiteren 60 Sekunden erneut
								nameTriggerTimestamp = Time.timeSinceLevelLoad;
								nameTriggerCounter--;
						}
				}

				public void TriggerName()
				{
						if (nameTriggerCounter < 10)
						{
								nameTriggerCounter++;
								nameTriggerTimestamp = Time.timeSinceLevelLoad;
						}
						else
						{
								nameTriggerTimestamp = Time.timeSinceLevelLoad;
						}
				}
		}
}