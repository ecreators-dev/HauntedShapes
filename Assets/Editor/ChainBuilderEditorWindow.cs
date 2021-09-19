using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Assets.Editor
{
		public class ChainBuilderEditorWindow : EditorWindow
		{
				[MenuItem("Game/Chain Builder")]
				public static void ShowWindow()
				{
						var window = GetWindow<ChainBuilderEditorWindow>("Chain Builder");
						window.Show();
				}

				private GameObject prefab;
				private Vector3 childOffset;
				private Vector3 childRotation;
				private EActionType actionType;
				private int childCounter;
				private GameObject previousChild;

				public enum EActionType
				{
						COPY_ONLY,
						CHAIN_JOINT
				}

				private void OnGUI()
				{
						var old = prefab;
						prefab = (GameObject)EditorGUILayout.ObjectField("Template", prefab, typeof(GameObject), allowSceneObjects: true);
						if (old != prefab || GUILayout.Button("Reset"))
						{
								actionType = EActionType.COPY_ONLY;
								childCounter = 0;
								previousChild = null;
						}

						GUI.enabled = prefab != null;
						childOffset = EditorGUILayout.Vector3Field("Extension Offset", childOffset);
						childRotation = EditorGUILayout.Vector3Field("Extension Rotate Offset", childRotation);
						actionType = (EActionType)EditorGUILayout.EnumPopup("Child Action Type", actionType);

						childCounter = EditorGUILayout.IntField("Next Number", childCounter + 1) - 1;
						if (GUILayout.Button("Neues Kindelement"))
						{
								childCounter++;
								var copy = Copy();

								if (actionType == EActionType.CHAIN_JOINT)
								{
										Transform parent = copy.transform.parent;
										Rigidbody parentRigidbody = parent.GetComponent<Rigidbody>();
										if (parentRigidbody == null)
										{
												parentRigidbody = parent.gameObject.AddComponent<Rigidbody>();
										}

										HingeJoint joint = copy.AddComponent<HingeJoint>();
										joint.connectedBody = parentRigidbody;
										Rigidbody childRb = copy.GetComponent<Rigidbody>();
										if (childRb == null)
										{
												copy.AddComponent<Rigidbody>();
										}
								}
						}
						GUI.enabled = true;
				}

				private GameObject Copy()
				{
						// with rigidbody
						if (previousChild == null)
						{
								previousChild = prefab;
						};
						GameObject result = Instantiate(prefab);
						result.name = $"{prefab.name}_{childCounter}";
						result.transform.SetParent(previousChild.transform);
						result.transform.localPosition = childOffset;
						result.transform.Rotate(childRotation, Space.World);
						previousChild = result;
						return result;
				}
		}
}