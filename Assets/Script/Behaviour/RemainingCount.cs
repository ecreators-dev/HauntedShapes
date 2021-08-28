public struct RemainingCount
{
		public RemainingCount(uint val)
		{
				RemainingValue = (int)val;
		}

		public int RemainingValue { get; set; }

		public static implicit operator int(RemainingCount val) => val.RemainingValue;
		public static implicit operator RemainingCount(int val) => new RemainingCount((uint)val);
}