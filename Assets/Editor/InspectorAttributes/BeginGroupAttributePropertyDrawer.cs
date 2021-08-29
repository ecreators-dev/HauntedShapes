using UnityEditor;

using UnityEngine;

namespace Assets.Script.InspectorAttibutes
{
		[CustomPropertyDrawer(typeof(BeginGroupAttribute))]
		public class BeginGroupAttributePropertyDrawer : PropertyDrawer
		{
				private const int V_PROPERTIES_COUNT = 2;
				private const int Y_GAP = 5;
				private float singleHeight;

				public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
				{
						singleHeight = EditorGUI.GetPropertyHeight(property, label, true);
						return singleHeight * V_PROPERTIES_COUNT + Y_GAP;
				}

				public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
				{
						EditorGUI.LabelField(position, label, GUI.skin.horizontalSlider);
						position.y += (Y_GAP + singleHeight) / V_PROPERTIES_COUNT + Y_GAP;
						position = EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);
						position.y -= (Y_GAP + singleHeight) / V_PROPERTIES_COUNT;
						property.boolValue = EditorGUI.Toggle(position, property.boolValue);
				}
		}
}
