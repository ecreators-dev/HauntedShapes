using Assets.Script.Behaviour;
using Assets.Script.Controller;
using Assets.Script.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public class HauntedShapesGameEditorWindow : EditorWindow
		{
				private Vector2 camerasScrollPosition;
				private bool viewsCanBeSynchronized;
				private Camera activeLinkedCamera;
				private SceneViewCameraAlignmentEditMode viewsSyncComponent;
				private bool cameraLinked;

				private bool interactiblesFoldout = true;
				private Vector2 interactiblesScrollPositions;
				private bool cameraListFoldoutStatus;

				[MenuItem("Game/Haunted-Shapes Window ...")]
				public static void Init()
				{
						HauntedShapesGameEditorWindow window = GetWindow<HauntedShapesGameEditorWindow>();
						window.titleContent = new GUIContent($"Haunted Shapes Einstellung (Editor only) - {nameof(HauntedShapesGameEditorWindow)}");
						window.Show();
				}

				void OnGUI()
				{
						EditorGUILayout.ObjectField("Script", this, typeof(HauntedShapesGameEditorWindow), allowSceneObjects: true);

						OnGUI_CameraSettings();

						HorizonalLine();

						OnGUI_Crosshair();

						HorizonalLine();

						OnGUI_Interactibles();

						GUILayout.FlexibleSpace();
				}

				private void HorizonalLine()
				{
						EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
				}

				private static void TryRemoveComponent<T>(GameObject gameObject)
						where T : Component
				{
						if (gameObject.TryGetComponent(out T linkerScript))
						{
								DestroyImmediate(linkerScript);
						}
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

						Camera[] cameras = FindObjectsOfType<Camera>();
						EditorGUILayout.LabelField($"Kameras in Scene: {cameras.Length}");

						EditorGUI.indentLevel++;
						cameraListFoldoutStatus = EditorGUILayout.Foldout(cameraListFoldoutStatus, "Auswahl sync Kameras (Scene)", true);
						EditorGUI.indentLevel--;

						if (cameraListFoldoutStatus)
						{
								ShowScrollArea(ref camerasScrollPosition, () =>
								{
										viewsCanBeSynchronized = false;
										foreach (Camera cam in cameras)
										{
												ShowCameraWithCheckBox(cam, cameras);
										}
								});
						}

						EditorGUI.indentLevel++;
						ShowSelectedCameraObject();

						ShowSynchronizingStatusCheckBox();
						EditorGUI.indentLevel--;

						GUI.enabled = activeLinkedCamera != null;
						if (GUI.enabled is false)
						{
								EditorGUILayout.HelpBox("Wähle eine aktive Kamera", MessageType.Info, true);
						}
						if (GUILayout.Button("Zeige Kamera Sicht"))
						{
								SceneView sceneView = SceneView.lastActiveSceneView;
								sceneView.ResetCameraSettings();
								sceneView.camera.projectionMatrix = activeLinkedCamera.projectionMatrix;
								sceneView.rotation = activeLinkedCamera.transform.rotation;
								sceneView.pivot = activeLinkedCamera.transform.position;
								sceneView.camera.CopyFrom(activeLinkedCamera);
								Debug.Log("Scenen Kamera aktualisiert");
						}
						GUI.enabled = true;

						//-- Local Functions -------------------------------------------

						void ShowCameraWithCheckBox(Camera listItemCam, IEnumerable<Camera> allCameras)
						{
								EditorGUILayout.BeginHorizontal();

								// checked status. camera is active linked
								bool cameraWasActive = activeLinkedCamera != null && listItemCam == activeLinkedCamera;
								EditorGUILayout.ObjectField(listItemCam, typeof(Camera), true);
								bool cameraIsActive = EditorGUILayout.Toggle("Verwenden", cameraWasActive);
								// check status changed
								if (cameraWasActive != cameraIsActive)
								{
										Debug.Log($"{cameraIsActive}");
										// changed to link now:
										if (cameraIsActive)
										{
												viewsCanBeSynchronized = true;
												activeLinkedCamera = listItemCam;
												TryAddComponentIfNotPresent(listItemCam.gameObject, out viewsSyncComponent);

												// if old was linked?! then link again (TODO)
												if (cameraLinked)
												{
														viewsSyncComponent.StartSynchronizing();
												}
										}
										// always: only a single camera can be selected!
										// if unchoose the current camera, then the selected must be null!
										else
										{
												RemoveScriptFromOthers<Camera, SceneViewCameraAlignmentEditMode>(allCameras, listItemCam);
												TryRemoveComponent<SceneViewCameraAlignmentEditMode>(listItemCam.gameObject);
												activeLinkedCamera = null;
												viewsCanBeSynchronized = false;
												cameraLinked = false;
										}
								}

								EditorGUILayout.EndHorizontal();
						}
				}

				private void ShowSelectedCameraObject()
				{
						GUI.enabled = false;
						EditorGUILayout.ObjectField("Sync-View-Kamera", activeLinkedCamera, typeof(Camera), allowSceneObjects: true);
						GUI.enabled = true;
				}

				private static void RemoveScriptFromOthers<T, C>(IEnumerable<T> others, T keep)
						where T : Component
						where C : Component
				{
						// remove from others:
						foreach (T camera in others.SkipWhile(c => c == keep))
						{
								TryRemoveComponent<C>(keep.gameObject);
						}
				}

				private void ShowSynchronizingStatusCheckBox()
				{
						bool oldStatus = cameraLinked;
						cameraLinked = EditorGUILayout.Toggle("Scene/Game Sync", cameraLinked, GUILayout.ExpandWidth(true));
						if (oldStatus != cameraLinked)
						{
								// sync now:
								if (cameraLinked)
								{
										viewsSyncComponent.StartSynchronizing();
										Debug.Log("Synchronizing Scene/Game View: running");
								}
								else if (viewsSyncComponent is { })
								{
										viewsSyncComponent.StopSynchronizing();
										Debug.Log("Synchronizing Scene/Game View: stopped");
								}
						}
				}

				private static void ShowScrollArea(ref Vector2 scrollPosition, Action content)
				{
						scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
						EditorGUI.indentLevel++;
						content.Invoke();
						EditorGUI.indentLevel--;
						EditorGUILayout.EndScrollView();
				}

				private static bool TryAddComponentIfNotPresent<C>(GameObject target, out C add)
						where C : Component
				{
						if (target == null)
						{
								add = null;
								return false;
						}

						if (target.TryGetComponent(out add) is false)
						{
								add = target.AddComponent<C>();
						}
						return true;
				}
		}
#endif
}
