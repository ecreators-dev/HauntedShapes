using System;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[DisallowMultipleComponent]
		public class LightInteractor : MonoBehaviour
		{
				[SerializeField] private Light pointLight;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
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
										if (collider.TryGetComponent(out PlayerBehaviour player))
										{
												Vector3 hitPoint = collider.ClosestPoint(myPosition);
												float distance = Vector3.Distance(hitPoint, myPosition);
												float intensity = pointLight.intensity * (1 / Mathf.Pow(distance, 2));
												player.InLightUpdate(this, intensity);
										}
								}
						}
				}
		}
}