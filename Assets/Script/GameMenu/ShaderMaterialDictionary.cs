using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Script.GameMenu
{
		public sealed class ShaderMaterialDictionary
		{
				private readonly List<string> materialNames;
				private readonly List<string> shaderNames;
				private readonly List<Material> uniqueMaterials;

				public ShaderMaterialDictionary(IEnumerable<Material> materials)
				{
						materialNames = materials.Select(material => material.name).Distinct().ToList();
						shaderNames = materials.Select(material => material.shader.name).Distinct().ToList();
						uniqueMaterials = materialNames.Select(name => materials.First(m => m.name.Equals(name))).ToList();
				}

				public IReadOnlyList<string> MaterialNames => materialNames.AsReadOnly();
				public IReadOnlyList<string> ShaderNames => shaderNames.AsReadOnly();
				public IReadOnlyList<Material> GetMaterialsDistinct() => uniqueMaterials.AsReadOnly();
		}
}