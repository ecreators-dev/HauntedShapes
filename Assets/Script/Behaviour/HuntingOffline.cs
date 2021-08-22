using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class HuntingOffline : MonoBehaviour
		{
				[SerializeField] private GameObject[] onOffGameObjects;
				[SerializeField] private HuntingOffRenderer[] onOffMaterials;
				[SerializeField] private Light[] onOffLights;

				private void Update()
				{
						SetActive(HuntingStateBean.Instance.InHunt);
				}

				private void SetActive(bool huntActive)
				{
						bool turnOff = huntActive;
						//Debug.Log($"Hunting Change to {huntActive}");

						bool offState = turnOff ? false : true;
						foreach (GameObject obj in onOffGameObjects)
						{
								if (turnOff)
								{
										if (huntActive)
										{
												string parentName = obj.transform.parent == null ? "" : obj.transform.parent.gameObject.name;
												Debug.Log($"turn off: {parentName}.{obj.name}");
										}

								}
								if (obj.activeSelf != offState)
								{
										obj.SetActive(offState);
								}
						}

						foreach (HuntingOffRenderer mat in onOffMaterials)
						{
								if (mat.initMaterial is null)
								{
										mat.initMaterial = mat.renderer.material;
								}

								if (huntActive)
								{
										TurnOffMaterial(huntActive, mat);
								}
								else if (mat.initMaterial is { } && mat.renderer.material != mat.initMaterial)
								{
										TurnOnMaterial(mat);
								}
						}

						foreach (Light light in onOffLights)
						{
								if (light.enabled != offState)
								{
										light.enabled = offState;

										if (turnOff)
										{
												string parentName = light.gameObject.transform.parent == null ? "" : light.gameObject.transform.parent.gameObject.name;
												Debug.Log($"turn off (light): {parentName}.{light.gameObject.name}");
										}
								}
						}
				}

				private static void TurnOnMaterial(HuntingOffRenderer mat)
				{
						mat.renderer.material = mat.initMaterial;
				}

				private static void TurnOffMaterial(bool huntActive, HuntingOffRenderer mat)
				{
						if (mat.offMaterial is { })
						{
								mat.renderer.material = mat.offMaterial;

								if (huntActive is false)
								{
										string parentName = mat.renderer.gameObject.transform.parent == null ? "" : mat.renderer.gameObject.transform.parent.gameObject.name;
										Debug.Log($"turn off (material): {parentName}.{mat.renderer.gameObject.name}");
								}
						}
				}
		}

		[Serializable]
		public class HuntingOffRenderer
		{
				public MeshRenderer renderer;
				[NonSerialized]
				public Material initMaterial;
				public Material offMaterial;
		}
}