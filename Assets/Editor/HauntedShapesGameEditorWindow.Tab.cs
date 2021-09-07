
using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public partial class HauntedShapesGameEditorWindow
		{
				private class Tab : Gui<HauntedShapesGameEditorWindow, Tab>
				{
						public delegate void OnTab(HauntedShapesGameEditorWindow window, Tab tabOwner, object data);
						public delegate void FetchDataHandler(object data);

						private readonly OnTab contentBuilder;
						private readonly FetchDataHandler fetchData;

						public Tab(string title, object data, OnTab contentBuilder, FetchDataHandler fetchData = null) : base(OnGUI)
						{
								Title = title;
								Data = data;
								this.contentBuilder = contentBuilder;
								this.fetchData = fetchData;
						}

						public string Title { get; set; }

						public object Data { get; }

						public bool IsExpanded { get; set; } = true;

						public static Vector2 ScrollPosition { get; set; }
						public TabControl TabControl { get; internal set; }

						private static void OnGUI(HauntedShapesGameEditorWindow window, Tab component)
						{
								ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
								component.contentBuilder.Invoke(window, component, component.Data);
								EditorGUILayout.EndScrollView();
						}

						public void FetchData()
						{
								this.fetchData?.Invoke(Data);
						}
				}
		}

#endif
}
