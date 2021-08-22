using System;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public class MissingSwitchCaseException : Exception
		{
				public MissingSwitchCaseException(Enum result) : base($"'{result}' was not covered by switch condition")
				{ }
		}
}