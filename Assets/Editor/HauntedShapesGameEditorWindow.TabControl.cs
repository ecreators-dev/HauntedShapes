using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public partial class HauntedShapesGameEditorWindow
		{
				private class TabControl : Gui<HauntedShapesGameEditorWindow, TabControl>
				{
						public TabControl(IList<Tab> tabs) : base(OnGUI)
						{
								List<Tab> list = tabs.ToList();
								Tabs = list.AsReadOnly();
								ActiveTab = 0;

								list.ForEach(tab => tab.TabControl = this);
						}

						public IReadOnlyList<Tab> Tabs { get; }

						public Tab GetTabWithTitle(string title) => Tabs.FirstOrDefault(tab => string.Equals(tab.Title, title, StringComparison.OrdinalIgnoreCase));

						public int ActiveTab { get; private set; }

						public Vector2 ScrollPosition { get; private set; }

						private static void OnGUI(HauntedShapesGameEditorWindow window, TabControl component)
						{
								RenderTabButtons(component);

								if (ShowTab(component))
								{
										OnGUI_ShowActiveTab(window, component);
								}

								GUI.enabled = true;
						}

						private static void OnGUI_ShowActiveTab(HauntedShapesGameEditorWindow window, TabControl component)
						{
								Tab activeTab = component.Tabs[component.ActiveTab];
								EditorGUI.indentLevel += activeTab.RootIntentDelta;
								activeTab.OnGUI(window);
								EditorGUI.indentLevel -= activeTab.RootIntentDelta;
						}

						private static bool ShowTab(TabControl component)
						{
								return component.ActiveTab >= 0 && component.ActiveTab < component.Tabs.Count;
						}

						private static void RenderTabButtons(TabControl component)
						{
								component.ScrollPosition = EditorGUILayout.BeginScrollView(component.ScrollPosition);
								EditorGUILayout.BeginHorizontal();
								// center from left
								GUILayout.FlexibleSpace();
								for (int i = 0; i < component.Tabs.Count; i++)
								{
										Tab tab = component.Tabs[i];

										GUI.enabled = tab.Enabled;
										FixActiveTab(component, i, tab);
										bool activeTab = i == component.ActiveTab;
										Vector2 size = GUISizeHelper.CalculateSize(tab.Title, skin => skin.button, out var style);
										if (GUILayout.Toggle(activeTab, tab.Title, style, GUILayout.MinWidth(size.x)))
										{
												component.ActiveTab = i;
										}
								}
								// center from right
								GUILayout.FlexibleSpace();
								EditorGUILayout.EndHorizontal();
								EditorGUILayout.EndScrollView();
								GUI.enabled = true;
						}

						private static void FixActiveTab(TabControl component, int i, Tab tab)
						{
								if (component.ActiveTab > component.Tabs.Count)
								{
										component.ActiveTab = 0;
								}
								if (i == component.ActiveTab && tab.Enabled is false)
								{
										component.ActiveTab++;
								}
						}
				}
		}

#endif
}
