using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class PforteCondition
		{
				public PforteTriggerTypeEnum type = PforteTriggerTypeEnum.BUCH;
				[Min(1)]
				public int amount = 1;

				public PforteTriggerTypeEnum TriggerType => type;

				public int RequiredCount => this.amount;

				public bool IsFullfilled(IPforteTrigger trigger, int available, out bool matchType)
				{
						// count up available before calling this!
						bool granted = trigger.TriggerType == type && available == amount;
						matchType = trigger.TriggerType == type;
						return granted;
				}
		}
}