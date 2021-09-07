using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Assets.Script.GameMenu
{
		public class CleanupTabData
		{
				public CleanupTabData()
				{
						textures = new TabData();
						materials = new TabData();
						shaders = new TabData();
				}

				public class TabData
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
				
				public TabData textures { get; }
				
				public TabData materials { get; }
				
				public TabData shaders { get; }
		}
}