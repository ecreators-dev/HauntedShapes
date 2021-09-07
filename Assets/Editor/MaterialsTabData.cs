using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.GameMenu
{
		public class MaterialsTabData
		{
				private Vector2 materialsScrollPos;

				public MaterialsTabData()
				{
				}

				public ref Vector2 materialsScrollPosition => ref materialsScrollPos;

				public Dictionary<AssetPath, Shader> UsedShaders { get; internal set; }
				public Dictionary<AssetPath, Material> UsedMaterials { get; internal set; }
				public Dictionary<AssetPath, Texture> UsedTextures { get; internal set; }
		}
}