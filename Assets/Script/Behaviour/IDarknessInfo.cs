namespace Assets.Script.Behaviour
{
		internal interface IDarknessInfo
		{
				float InDarknessTime { get; }
				float DarknessMultiplier { get; }
				bool IsInDarkness { get; }

				void InLightUpdate(float intensity);
		}
}