using Assets.Script.Behaviour;
using Assets.Script.Controller;
using Assets.Script.Model;

using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public class HauntedShapesGameEditorWindow : EditorWindow
		{
				private bool cameraLinked;
				private MonoBehaviour targetObject;
				private SceneViewCameraAlignmentEditMode linkedCameraScript;
				private bool interactiblesFoldout = true;
				private Vector2 interactiblesScrollPositions;

				[MenuItem("Game/Haunted Shapes Game Editor Settings ...")]
				public static void Init()
				{
						HauntedShapesGameEditorWindow window = GetWindow<HauntedShapesGameEditorWindow>();
						window.titleContent = new GUIContent($"Haunted Shapes Einstellung (Editor only) - {nameof(HauntedShapesGameEditorWindow)}");
						window.Show();
				}

				void OnGUI()
				{
						targetObject = (MonoBehaviour)EditorGUILayout.ObjectField("Camera Player", targetObject, typeof(PlayerBehaviour), allowSceneObjects: true);
						OnGUI_CameraSettings();
						OnGUI_Crosshair();
						OnGUI_Interactibles();
				}

				private void OnGUI_Crosshair()
				{
						var oneCrosshair = FindObjectOfType<CrosshairHitVisual>();
						EditorGUILayout.LabelField("Crosshair (Klick-Steuerung) (Script)");
						if (oneCrosshair != null)
						{
								EditorGUILayout.ObjectField(oneCrosshair, oneCrosshair.GetType(), allowSceneObjects: true);
						}
						else
						{
								EditorGUILayout.HelpBox($"Kein {nameof(CrosshairHitVisual)} gefunden!", MessageType.Warning, true);
						}
				}

				private void OnGUI_Interactibles()
				{
						interactiblesFoldout = EditorGUILayout.Foldout(interactiblesFoldout, "Interaktive Objekte - Übersicht", true);
						if (interactiblesFoldout)
						{
								var all = (from obj in FindObjectsOfType<MonoBehaviour>()
													 let interactible = obj.GetComponent<IInteractible>()
													 where interactible is { }
													 let typeName = interactible.GetType().Name
													 orderby obj.gameObject.name ascending, typeName
													 select (@object: obj, interactible, typeName)).ToList();

								interactiblesScrollPositions = EditorGUILayout.BeginScrollView(interactiblesScrollPositions);
								EditorGUI.indentLevel++;
								foreach ((MonoBehaviour @object, IInteractible interactible, string typeName) in all)
								{
										EditorGUILayout.ObjectField(@object, typeof(IInteractible), allowSceneObjects: true);
								}
								EditorGUI.indentLevel--;
								EditorGUILayout.EndScrollView();
						}
				}

				private void OnGUI_CameraSettings()
				{
						GUILayout.Label("Kamera Einstellungen", EditorStyles.boldLabel);

						bool oldStatus = cameraLinked;
						if (targetObject != null && targetObject.gameObject != null && targetObject.TryGetComponent(out SceneViewCameraAlignmentEditMode component))
						{
								cameraLinked = component.IsUpdating();
								linkedCameraScript = component;
						}
						cameraLinked = EditorGUILayout.Toggle("Scene/Camera Live View", cameraLinked);
						if (oldStatus != cameraLinked)
						{
								if (cameraLinked)
								{
										if (targetObject is { } && targetObject.gameObject == null)
										{
												Debug.Log($"{nameof(HauntedShapesGameEditorWindow)}: reset Player Camera (to null)");
												targetObject = null;
										}

										targetObject ??= FindObjectsOfType<PlayerBehaviour>().FirstOrDefault();
										if (targetObject is { })
										{
												EditorGUIUtility.PingObject(targetObject);
												TryAddLiveLinkSceneViewComponent(out var linker);
												linker.AlignWithViewLive();
												linkedCameraScript = linker;
										}
								}
								else if (linkedCameraScript is { })
								{
										linkedCameraScript.Unlink();
										Debug.Log("Scene View/Camera: Link Live disabled");
								}
						}
				}

				private void TryAddLiveLinkSceneViewComponent(out SceneViewCameraAlignmentEditMode linker)
				{
						if (targetObject.gameObject == null)
						{
								linker = null;
								return;
						}

						if (targetObject.TryGetComponent(out linker) is false)
						{
								linker = targetObject.gameObject.AddComponent<SceneViewCameraAlignmentEditMode>();
						}
				}
		}
#endif
}
