using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
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