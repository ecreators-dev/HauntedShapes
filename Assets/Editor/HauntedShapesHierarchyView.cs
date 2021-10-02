using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
		[InitializeOnLoad]
		public class HauntedShapesHierarchyView
		{
				private static readonly Dictionary<int, HierarchyItemData> data = new Dictionary<int, HierarchyItemData>();

				static HauntedShapesHierarchyView()
				{
						EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
				}

				public static Color gameObjectFontColor = Color.black;
				public static Color prefabOrgFontColor = Color.black;
				public static Color prefabModFontColor = Color.white;
				public static Color backgroundColor = new Color(.76f, .76f, .76f);
				public static Color inActiveColor = new Color(0.01f, 0.4f, 0.25f, 0);

				private static Vector2 offset = new Vector2(0, 2);

				private class HierarchyItemData
				{
						public bool HideChildren { get; set; } = false;
						public bool HideAsChild { get; internal set; }
				}

				private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
				{
						Color fontColor = gameObjectFontColor;
						FontStyle styleFont = FontStyle.Normal;
						var target = EditorUtility.InstanceIDToObject(instanceID);
						if (Selection.instanceIDs.Contains(instanceID))
						{
								backgroundColor = new Color(0.24f, 0.48f, 0.90f);
						}

						if (target != null)
						{
								GameObject gameObj = target as GameObject;
								if (gameObj.activeInHierarchy == false)
								{
										backgroundColor = inActiveColor;
								}
								if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Regular)
								{
										styleFont = FontStyle.Bold;
										PropertyModification[] prefabMods = PrefabUtility.GetPropertyModifications(target);
										foreach (PropertyModification prefabMod in prefabMods)
										{
												if (prefabMod.propertyPath.ToString() != "m_Name" && prefabMod.propertyPath.ToString() != "m_LocalPosition.x" && prefabMod.propertyPath.ToString() != "m_LocalPosition.y" && prefabMod.propertyPath.ToString() != "m_LocalPosition.z" && prefabMod.propertyPath.ToString() != "m_LocalRotation.x" && prefabMod.propertyPath.ToString() != "m_LocalRotation.y" && prefabMod.propertyPath.ToString() != "m_LocalRotation.z" && prefabMod.propertyPath.ToString() != "m_LocalRotation.w" && prefabMod.propertyPath.ToString() != "m_RootOrder" && prefabMod.propertyPath.ToString() != "m_IsActive")
												{
														fontColor = prefabModFontColor;
														break;
												}
										}
										if (fontColor != prefabModFontColor)
												fontColor = prefabOrgFontColor;
								}

								ShowLabeledItem(selectionRect, fontColor, styleFont, target);
						}
				}

				private static void ShowLabeledItem(Rect selectionRect, Color fontColor, FontStyle styleFont, UnityEngine.Object target)
				{
						var myData = GetData(target);
						target.hideFlags = myData.HideAsChild ? HideFlags.HideInInspector : HideFlags.None;
						if (myData.HideAsChild)
						{
								return;
						}
						Rect offsetRect = ReplaceBackground(selectionRect);
						GUIStyle style = new GUIStyle()
						{
								normal = new GUIStyleState() { textColor = fontColor },
								fontStyle = styleFont,
								imagePosition = ImagePosition.ImageLeft
						};
						EditorGUI.LabelField(offsetRect, target.name, style);

						float buttonWidth = 50;
						offsetRect.xMin += selectionRect.width - buttonWidth;

						if (target is GameObject go)
						{
								if (go.transform.childCount > 0)
								{
										HierarchyItemData data = GetData(target);
										data.HideChildren = EditorGUI.ToggleLeft(offsetRect, "Hide", data.HideChildren);
										Transform reference = go.transform;
										foreach (Transform child in reference)
										{
												var model = GetData(child.gameObject);
												model.HideAsChild = data.HideChildren;
										}
								}
						}
				}

				private static HierarchyItemData GetData(UnityEngine.Object target)
				{
						if (HauntedShapesHierarchyView.data.TryGetValue(target.GetInstanceID(), out var data) is false)
						{
								data = new HierarchyItemData();
								HauntedShapesHierarchyView.data[target.GetInstanceID()] = data;
						}

						return data;
				}

				private static Rect ReplaceBackground(Rect selectionRect)
				{
						Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
						EditorGUI.DrawRect(selectionRect, backgroundColor);
						return offsetRect;
				}
		}
}