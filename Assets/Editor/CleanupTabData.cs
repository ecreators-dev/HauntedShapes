using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Assets.Script.GameMenu
{
		public class CleanupTabData
		{
				public CleanupTabData()
				{
						textures = new ProjectCleanUpTabData();
						materials = new ProjectCleanUpTabData();
						shaders = new ProjectCleanUpTabData();
						scriptObjects = new SceneObjectTabData();
				}

				public class SceneObjectTabData
				{
						private Vector2 scrollPos;
						public ref Vector2 ScrollPosition => ref scrollPos;

						public List<Object> Objects { get; set; }
				}

				public class ProjectCleanUpTabData
				{
						private Vector2 scrollPos;

						public DateTime SearchUpdate { get; set; }
						public ISet<AssetPath> Unused { get; set; }
						public List<AssetPath> UnusedSorted { get; set; }
						public List<AssetPath> ForDelete { get; } = new List<AssetPath>();
						public EntriesPerPageEnum PageMode { get; set; }
						public int PageIndex { get; set; }
						public ref Vector2 ScrollPosition => ref scrollPos;
						public string FilterText { get; set; }
						public bool FilterActive { get; set; }
						public Regex FilterPattern { get; set; }
						public bool FilterEnabled { get; set; }
				}
				
				public ProjectCleanUpTabData textures { get; }
				
				public ProjectCleanUpTabData materials { get; }
				
				public ProjectCleanUpTabData shaders { get; }

				public SceneObjectTabData scriptObjects { get; }
		}
}