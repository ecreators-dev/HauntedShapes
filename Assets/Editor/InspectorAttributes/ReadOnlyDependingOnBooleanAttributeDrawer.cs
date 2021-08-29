using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Controller
{
		[CustomPropertyDrawer(typeof(ReadOnlyDependingOnBooleanAttribute))]
		public sealed class ReadOnlyDependingOnBooleanAttributeDrawer : PropertyDrawer
		{
				public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
				{
						return EditorGUI.GetPropertyHeight(property, label, true);
				}

				public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
				{
						GUI.enabled = FindBoolean(base.attribute as ReadOnlyDependingOnBooleanAttribute, property);
						EditorGUI.PropertyField(position, property, label, true);
						GUI.enabled = true;
				}

				private bool FindBoolean(ReadOnlyDependingOnBooleanAttribute attribute, SerializedProperty property)
				{
						SerializedObject obj = property.serializedObject;
						SerializedProperty depentProperty = obj.FindProperty(attribute.BooleanFieldName);
						// serialized class / no game object?
						if (depentProperty == null)
						{
								string[] tree = property.propertyPath.Split('.');
								do
								{
										tree = string.Join(".", tree.Take(tree.Length - 1)).Split('.');
										string objPropertyName = tree.Last();
										SerializedProperty ownerObject = obj.FindProperty(objPropertyName);
										depentProperty = ownerObject.FindPropertyRelative(attribute.BooleanFieldName);
								} while (depentProperty == null && tree.Length > 0);

								if (tree.Length == 0 && depentProperty == null)
								{
										Debug.LogError("Unable to find property depending on boolean (EDIT MODE only)");
										return true;
								}
						}
						return depentProperty.boolValue ? attribute.TrueIfState : !attribute.TrueIfState;
				}
		}
}