using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[Serializable]
		public sealed class LayerMaskFilter
		{
				public ISet<LayerMask> Includers { get; } = new HashSet<LayerMask> { LayerMaskUtils.EVERY_LAYER };

				public ISet<LayerMask> Excluders { get; } = new HashSet<LayerMask>();
		}
}