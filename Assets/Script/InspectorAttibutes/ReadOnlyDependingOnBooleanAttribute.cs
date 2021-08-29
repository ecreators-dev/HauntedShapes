using System;

using UnityEngine;

namespace Assets.Script.Controller
{
		[Serializable]
		public sealed class ReadOnlyDependingOnBooleanAttribute : PropertyAttribute
		{
				public ReadOnlyDependingOnBooleanAttribute(string nameOfBooleanFieldSameOwner, bool trueState)
				{
						TrueIfState = trueState;
						BooleanFieldName = nameOfBooleanFieldSameOwner;
				}

				public string BooleanFieldName { get; }

				public bool TrueIfState { get; }
		}
}