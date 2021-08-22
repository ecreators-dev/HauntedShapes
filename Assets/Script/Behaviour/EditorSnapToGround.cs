
using System.Linq;

using UnityEngine;

namespace Assets.Editor
{
		[ExecuteAlways]
		public class EditorSnapToGround : MonoBehaviour
		{
				[Tooltip("Raycasts (also in Edit Mode) to the ground. If another collider was found, then this object is placed at hit point")]
				[SerializeField] private bool snapToGroundActive = true;

				private void Update()
				{
						if (snapToGroundActive)
						{
								GroundTransform(transform);
						}
				}

				public static void GroundTransform(Transform selfTransform)
				{
						Vector3 lowerUp = selfTransform.position + Vector3.up * 0.25f;
						RaycastHit[] hits = Physics.RaycastAll(lowerUp, Vector3.down, 10f);
						foreach (RaycastHit hit in hits)
						{
								if (IsSelf(selfTransform, hit))
										continue;

								selfTransform.position = hit.point;
						}

						if (hits.Any() is false)
						{
								Debug.LogWarning("Unable to find Ground! (Target Collider may be Convex)");
						}

						static bool IsSelf(Transform selfTransform, RaycastHit hit)
						{
								return hit.collider.gameObject == selfTransform.gameObject;
						}
				}
		}
}