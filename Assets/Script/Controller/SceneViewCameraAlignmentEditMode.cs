using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Controller
{
		/// <summary>
		/// Keeps the object linked to the current scene view (simulate in game view always).
		/// Automated "align object with scene view"
		/// </summary>
		[ExecuteInEditMode]
		public class SceneViewCameraAlignmentEditMode : MonoBehaviour
		{
#if UNITY_EDITOR
				[SerializeField] bool update = true;
				[SerializeField] bool linkPosition = true;
				[SerializeField] bool linkRotation = true;

				private Transform Transform;

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						update = false;
				}

				private void OnRenderObject()
				{
						if (update is false)
								return;

						SceneView scene = SceneView.lastActiveSceneView;
						if (scene is null)
								return;

						Camera sceneCamera = scene.camera;
						if (sceneCamera is null)
								return;

						AlignWithSceneView(sceneCamera);
				}

				public void StopSynchronizing() => update = false;

				public void StartSynchronizing() => update = true;

				private void AlignWithSceneView(Camera sceneCamera)
				{
						Transform targetTransform = sceneCamera.transform;
						if (targetTransform != default)
						{
								Transform ??= transform;

								Vector3 oldPosition = Transform.position;
								Vector3 oldEuler = Transform.eulerAngles;
								bool changedPosition = false;

								if (linkPosition && oldPosition != targetTransform.position)
								{
										changedPosition = true;
										Transform.position = targetTransform.position;
								}

								bool changedRotation = false;
								if (linkRotation && oldEuler != targetTransform.eulerAngles)
								{
										changedRotation = true;
										Transform.rotation = targetTransform.rotation;
								}

								if (changedPosition || changedRotation)
								{
										EditorUtility.SetDirty(gameObject);
								}
						}
				}

				public bool IsUpdating() => this.update;
#endif
		}
}