
using UnityEditor;

namespace Assets.Editor
{
		public class HierarchyEditor : EditorWindow
		{
				[MenuItem("Tools/Hierarchy Editor")]
				public static void ShowWindow()
				{
						GetWindow<HierarchyEditor>("HierarchyEditor");
				}
				private void OnGUI()
				{
						HauntedShapesHierarchyView.gameObjectFontColor = EditorGUILayout.ColorField("Original Font Color", HauntedShapesHierarchyView.gameObjectFontColor);
						HauntedShapesHierarchyView.prefabOrgFontColor = EditorGUILayout.ColorField("Prefab Original Font Color", HauntedShapesHierarchyView.prefabOrgFontColor);
						HauntedShapesHierarchyView.prefabModFontColor = EditorGUILayout.ColorField("Prefab Modified Font Color", HauntedShapesHierarchyView.prefabModFontColor);
						HauntedShapesHierarchyView.backgroundColor = EditorGUILayout.ColorField("Default Background Color", HauntedShapesHierarchyView.backgroundColor);
						HauntedShapesHierarchyView.inActiveColor = EditorGUILayout.ColorField("Inactive Color", HauntedShapesHierarchyView.inActiveColor);
				}
		}
}