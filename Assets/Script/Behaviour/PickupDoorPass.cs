using Assets.Script.Behaviour;

using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Door
{
		[RequireComponent(typeof(BoxCollider))]
		public class PickupDoorPass : MonoBehaviour
		{
				[SerializeField] private Transform exitTransform;

				private Collider passCollider;

				private void Awake()
				{
						passCollider = GetComponent<BoxCollider>();
				}

				private void Start()
				{
						passCollider.isTrigger = true;
						Debug.Log($"setting {nameof(PickupDoorPass)} collider.isTrigger = true at Start()");
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						// remarks: remember to enble Gizmos to see this!
						Handles.color = Color.green;
						Handles.DrawLine(exitTransform.position, transform.position);
						Handles.color = Color.blue;
						Handles.DrawWireCube(exitTransform.position, Vector3.one * 0.25f);
						RaycastHit? floorHit = GetFloorHitDownside(exitTransform);
						if (floorHit.HasValue)
						{
								Handles.DrawLine(exitTransform.position, floorHit.Value.point);
								Handles.color = new Color(0, 0, 0.4f, 0.1f);
								Handles.DrawSolidDisc(floorHit.Value.point, Vector3.up, 0.5f);
						}
						Handles.color = Color.white;
						Handles.Label(exitTransform.position, exitTransform.gameObject.name);
				}
#endif

				private void OnTriggerEnter(Collider other)
				{
						if (other.TryGetComponent(out PlayerBehaviour behaviour))
						{
								TeleportTo(behaviour, exitTransform);
						}
				}

				private void TeleportTo(Component behaviour, Transform exitTransform)
				{
						float distanceToGround = GetDistanceToGround(behaviour);
						Vector3 position = exitTransform.position;
						position.y = exitTransform.position.y + distanceToGround;
						behaviour.transform.position = position;
						behaviour.transform.rotation = exitTransform.rotation;
				}

				private float GetDistanceToGround(Component behaviour)
				{
						Transform myTransform = behaviour.transform;
						GameObject go = behaviour.gameObject;
						if (Physics.Raycast(myTransform.position, -Vector3.up, out RaycastHit hit))
						{
								Vector3 targetPosition = hit.point;
								if (go.GetComponent<MeshFilter>() != null)
								{
										Bounds bounds = go.GetComponent<MeshFilter>().sharedMesh.bounds;
										targetPosition.y += bounds.extents.y;
								}
								return Vector3.Distance(myTransform.position, targetPosition);
						}
						return 0;
				}

				private RaycastHit? GetFloorHitDownside(Transform from)
				{
						if (Physics.Raycast(from.position, -Vector3.up, out RaycastHit hit))
						{
								return hit;
						}
						return null;
				}
		}
}
