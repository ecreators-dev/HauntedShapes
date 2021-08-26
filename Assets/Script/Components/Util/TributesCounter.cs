using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Script.Components
{
		public sealed class TributesCounter
		{
				private readonly Dictionary<EObjectType, uint> tributesRequired;
				private readonly Dictionary<EObjectType, uint> requiredInit;

				public int CountNoRemainingTypes => tributesRequired.Values.Count(item => item == 0);

				public TributesCounter(IEnumerable<Tribute> tributes)
				{
						tributesRequired = tributes.ToDictionary(t => t.objectType, t => (uint)t.count);
						requiredInit = tributes.ToDictionary(t => t.objectType, t => (uint)t.count);
				}

				public int GetRemainingOfType(EObjectType type) => (int)tributesRequired[type];

				public int GetTributedSizeOfType(EObjectType type) => (int)(requiredInit[type] - GetRemainingOfType(type));

				public void TakeObject(EObjectType type) => tributesRequired[type] = Math.Min(tributesRequired[type] + 1, requiredInit[type]);

				public void PutObject(EObjectType type) => tributesRequired[type] = Math.Max(tributesRequired[type] - 1, 0);

				public bool HasType(EObjectType type) => requiredInit.ContainsKey(type);
		}
}
