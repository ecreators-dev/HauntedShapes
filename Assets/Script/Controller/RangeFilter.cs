using System;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public sealed class RangeFilter
		{
				public float ClickRange { get; set; } = 6;
				public float HoverRange { get; set; } = 8;
		}
}