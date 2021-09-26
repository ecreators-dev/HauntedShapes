using Assets.Script.Behaviour;
using Assets.Script.Behaviour.FirstPerson;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace Assets.Door
{
		[RequireComponent(typeof(BoxCollider))]
		public class TriggerTeleporter : MonoBehaviour
		{
				[SerializeField] private Transform exitTransform;

				private Collider passCollider;
				public List<PlayerBehaviour> TeleportedPlayers { get; } = new List<PlayerBehaviour>();

				private void Awake()
				{
						passCollider = GetComponent<BoxCollider>();
				}

				private void Start()
				{
						passCollider.isTrigger = true;
						Debug.Log($"setting {nameof(TriggerTeleporter)} collider.isTrigger = true at Start()");
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						if (exitTransform == null)
								return;

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
						if (other.TryGetComponent(out PlayerBehaviour player))
						{
								TeleportedPlayers.Add(player);
								player.SetTeleported(exitTransform);
						}
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
