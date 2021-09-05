using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class PforteCondition
		{
				public TributeTypeEnum type = TributeTypeEnum.ENCHANTMENT_BOOK;
				[Min(1)]
				public int amount = 1;

				public TributeTypeEnum TriggerType => type;

				public int RequiredCount => this.amount;

				public bool IsFullfilled(IRitualTribute trigger, int available, out bool matchType)
				{
						// count up available before calling this!
						bool granted = trigger.TriggerType == type && available == amount;
						matchType = trigger.TriggerType == type;
						return granted;
				}
		}
}