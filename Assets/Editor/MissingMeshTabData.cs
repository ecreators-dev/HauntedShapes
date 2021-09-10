using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.GameMenu
{
		public class MissingMeshTabData
		{
				private Vector2 scrollPos;

				public MissingMeshTabData()
				{
				}

				public List<(MeshCollider repair, Mesh mesh)> repairables { get; internal set; }
				public List<MeshCollider> nonRepairables { get; internal set; }

				public ref Vector2 ScrollPosition => ref scrollPos;
		}
}