
using UnityEngine;

namespace Assets.Script.Behaviour.GhostTypes
{
		public enum AgeType
		{
				[Range(0, 12)]
				CHILD,
				[Range(13, 17)]
				TEEN,
				[Range(18, 60)]
				ADULT,
				[Range(61, 5000)]
				OLD
		}
}