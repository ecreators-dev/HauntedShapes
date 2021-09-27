using System;

using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Script.Behaviour.FirstPerson
{
		[Serializable]
		public sealed class CameraMoveType
		{
				[SerializeField] private Camera camBumping;
				[SerializeField] private Camera camNotBumping;
				[SerializeField] private TypeEnum type;
				private bool neverLoad = true;
				private TypeEnum? oldType = null;
				private Camera current;

				public static CameraMoveType Instance { get; private set; }

				public CameraMoveType(TypeEnum type)
				{
						this.type = type;
						Instance = this;
				}

				public TypeEnum Type => type;

				public void SetType(TypeEnum type) => this.type = type;

				internal void FixCameraMissing()
				{
						camNotBumping ??= Find(camNotBumping.gameObject.name);
						camBumping ??= Find(camBumping.gameObject.name);
						camBumping.gameObject.SetActive(false);
				}

				private Camera Find(string name)
				{
						Debug.LogError($"Camera missing: {name}");
						Camera result = null;

#if UNITY_EDITOR
						string[] paths = new string[0];
						paths = AssetDatabase.FindAssets(name);
						foreach (var item in paths)
						{

								result = AssetDatabase.LoadAssetAtPath<Camera>(item);
								if (result != null)
								{
										Debug.LogError($"Camera found in assets: {name} at {item}");
										break;
								}
						}
#endif

						if (result == null)
						{
								Debug.LogError($"Camera not found in assets: {name}. Make my own.");
								GameObject cam = new GameObject(name);
								cam.tag = "MainCamera";
								result = cam.AddComponent<Camera>();
								var _ = cam.GetComponent<AudioListener>() ?? cam.AddComponent<AudioListener>();
								cam.AddComponent<HDAdditionalCameraData>();
						}
						return result;
				}

				public Camera GetCamera()
				{
						bool initOrChanged = oldType == null || oldType.Value != type;
						if (initOrChanged)
						{
								if (Type == TypeEnum.BUMPING)
								{
										SwitchCamera(camBumping, camNotBumping);
										current = camBumping;
								}
								else
								{
										SwitchCamera(camNotBumping, camBumping);
										current = camNotBumping;
								}
						}
						oldType = type;
						return current;
				}

				private void SwitchCamera(Camera enable, Camera disable)
				{
						if (neverLoad)
						{
								if (Camera.main == enable)
								{
										disable.projectionMatrix = enable.projectionMatrix;
										disable.CopyFrom(enable);
								}
								else if (Camera.main == disable)
								{
										enable.projectionMatrix = disable.projectionMatrix;
										enable.CopyFrom(disable);
								}
						}

						disable.gameObject.SetActive(false);
						enable.transform.position = disable.transform.position;
						enable.transform.rotation = disable.transform.rotation;
						enable.gameObject.SetActive(true);
						neverLoad = false;
				}

				public enum TypeEnum
				{
						NOT_BUMPING,
						BUMPING
				}
		}
}