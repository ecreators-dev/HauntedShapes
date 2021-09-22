using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[DisallowMultipleComponent]
		public class LightInteractor : MonoBehaviour
		{
				private static readonly List<LightInteractor> lights = new List<LightInteractor>();

				public static IReadOnlyList<LightInteractor> Lights => lights.AsReadOnly();

				[SerializeField] private Light pointLight;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
						lights.Add(this);
				}

				private void Update()
				{
						if (pointLight.gameObject.activeSelf &&
								pointLight.enabled)
						{
								Vector3 myPosition = Transform.position;
								var colliders = Physics.OverlapSphere(myPosition, pointLight.range);
								foreach (Collider collider in colliders)
								{
										if (collider.TryGetComponent(out DarknessInfo darknessInfo))
										{
												Vector3 hitPoint = collider.ClosestPoint(myPosition);
												float distance = Vector3.Distance(hitPoint, myPosition);
												float intensity = pointLight.intensity * (1 / Mathf.Pow(distance, 2));
												lights.Add(this);
												darknessInfo.InLightUpdate(intensity);
										}
								}
						}
				}

				public static void ClearLights()
				{
						lights.Clear();
				}
		}
}