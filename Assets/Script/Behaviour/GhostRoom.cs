using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Defines a box collider as area to spawn or move to ghost orbs
		/// </summary>
		[RequireComponent(typeof(BoxCollider))]
		[ExecuteAlways]
		public class GhostRoom : MonoBehaviour
		{
				public static bool showOrbPositions = true;

				[SerializeField] private string roomName;
				[SerializeField] private Color roomColor;
				[SerializeField] private MeshRenderer meshRenderer;
				[SerializeField] private float ghostOrbY;
				[SerializeField] private bool updateHitlines = true;
				[SerializeField] private Color hitlineColor = new Color(1, 1, 1, 0.2f);

				private Material meshMaterial;

				private bool hitPositionUpdate;
				private List<(Vector3 source, Vector3 hit)> hitPositions;
				private int randomPositionIndex = -1;

				private BoxCollider Collider { get; set; }

				public string RoomName => roomName;

				public Vector3 Size => Collider.bounds.size;

				public float GhostOrbY => ghostOrbY;

				public Color Color => this.roomColor;

				private void Awake()
				{
						Collider = GetComponent<BoxCollider>();
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						Handles.color = roomColor;
						Transform Transform = transform;
						Vector3 center = Transform.position;
						Handles.Label(center, RoomName);

						Vector3 size = Collider.bounds.size;
						size.y = 0.02f;
						Vector3 position = Transform.position;
						position.y += ghostOrbY;
						Gizmos.color = roomColor;
						Gizmos.DrawCube(position, size);

						if (hitPositions == null || updateHitlines)
						{
								hitPositions = UpdateHitLines(hitlineColor);
						}
				}

				public Vector3 GetRandomOrbPosition()
				{
						UpdateHitlines();
						int index = Random.Range(0, hitPositions.Count);
						randomPositionIndex = index;
						return hitPositions[index].source;
				}

				public void UpdateHitlines()
				{
						hitPositions = UpdateHitLines(hitlineColor);
				}

				private List<(Vector3 source, Vector3 hit)> UpdateHitLines(Color color, float offsetX = 0.5f, float offsetZ = 0.5f)
				{
#if UNITY_EDITOR
						Handles.color = color;
#endif
						hitPositionUpdate = true;
						hitPositions = new List<(Vector3 source, Vector3 hit)>();

						Vector3 size = Collider.bounds.size;
						int amountX = Mathf.FloorToInt((size.x / offsetX) / 2);
						int amountZ = Mathf.FloorToInt((size.z / offsetZ) / 2);
						Vector3 position = transform.position;
						position.y += ghostOrbY;

						// from center:
						if (DrawGizmoHitLine(color, position, 0, 0, out var orgin, out var originHit))
						{
								hitPositions.Add((orgin, originHit));
						}

						for (int x = 0; x < amountX; x++)
						{
								for (int z = 0; z < amountZ; z++)
								{
										if (x == 0 && z == 0) continue;

										// right forward
										if (DrawGizmoHitLine(color, position, offsetX * x, offsetZ * z, out var sourceA, out var hitA))
										{
												hitPositions.Add((sourceA, hitA));
										}

										// left forward
										if (DrawGizmoHitLine(color, position, offsetX * -x, offsetZ * z, out var sourceB, out var hitB))
										{
												hitPositions.Add((sourceB, hitB));
										}

										// right back
										if (DrawGizmoHitLine(color, position, offsetX * x, offsetZ * -z, out var sourceC, out var hitC))
										{
												hitPositions.Add((sourceC, hitC));
										}

										// left back
										if (DrawGizmoHitLine(color, position, offsetX * -x, offsetZ * -z, out var sourceD, out var hitD))
										{
												hitPositions.Add((sourceD, hitD));
										}
								}
						}
						hitPositionUpdate = false;
						return hitPositions;
				}

				private bool DrawGizmoHitLine(Color color, Vector3 center, float xOffset, float zOffset, out Vector3 collisionDown, out Vector3 collistionHit)
				{
						Ray ray = new Ray(new Vector3(center.x + xOffset, center.y, center.z + zOffset), Vector3.down);
						collisionDown = ray.origin;
						if (Physics.Raycast(ray, out RaycastHit hit, 10))
						{
#if UNITY_EDITOR
								if (Application.isPlaying is false)
								{
										// this call will add the position at that index position
										if (IsGizmoHitLineMatch())
										{
												Handles.color = Color.yellow;
										}
										else
										{
												Handles.color = color;
										}

										if (showOrbPositions)
										{
												Handles.DrawLine(ray.origin, hit.point);
										}
								}
#endif
								collistionHit = hit.point;
								return true;
						}
						collistionHit = default;
						return false;
				}

				private bool IsGizmoHitLineMatch()
				{
						return randomPositionIndex >= 0 && hitPositions != null
																&& randomPositionIndex == hitPositions.Count - 1;
				}
#endif

				private void Start()
				{
						// no trigger, no force - for Y only
						Collider.isTrigger = true;

						if (meshRenderer is { })
						{
								meshMaterial = meshRenderer.sharedMaterial;
								meshRenderer.material = meshMaterial;
								meshMaterial.color = roomColor;
						}
				}

				private void OnTriggerEnter(Collider other)
				{
						if (IsPlayer(other))
						{
								if (meshMaterial != null)
								{
										meshMaterial.color = Color.red;
								}
						}
				}

				private static bool IsPlayer(Collider other)
				{
						return other.gameObject != null && other.TryGetComponent(out PlayerBehaviour player);
				}

				private void OnTriggerExit(Collider other)
				{
						if (IsPlayer(other))
						{
								if (meshMaterial != null)
								{
										meshMaterial.color = roomColor;
								}
						}
				}

				private void Update()
				{

				}
		}
}