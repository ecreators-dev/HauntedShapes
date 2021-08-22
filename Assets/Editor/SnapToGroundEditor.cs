using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
		public class SnapToGroundEditor : EditorSnapToGround
		{
				[MenuItem("Game/Move Selection to ground (Strg + g) %g")]
				public static void GroundAllInSelection()
				{
						foreach (Transform selfTransform in Selection.transforms)
						{
								GroundTransform(selfTransform);
						}
				}
		}

		[CustomEditor(typeof(EditorSnapToGround))]
		public class EditorSnapToGroundInspectorWindow : UnityEditor.Editor
		{
				public override void OnInspectorGUI()
				{
						var targetType = (EditorSnapToGround)base.target;
						base.OnInspectorGUI();

						if (GUILayout.Button("Snap To Ground"))
						{
								EditorSnapToGround.GroundTransform(targetType.transform);
						}
				}
		}
}