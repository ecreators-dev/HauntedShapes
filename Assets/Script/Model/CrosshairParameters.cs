using System;

using UnityEngine;

namespace Assets.Script.Model
{
		[Serializable]
		public class CrosshairParameters
		{
				[Range(-1, 1)]
				[SerializeField] private float placementOffset = 0.25f;

				public float PlacementOffsetNormal => placementOffset;
		}
}