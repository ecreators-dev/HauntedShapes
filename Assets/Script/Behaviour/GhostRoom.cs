using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		/// <summary>
		/// Defines a box collider as area to spawn or move to ghost orbs
		/// </summary>
		[RequireComponent(typeof(BoxCollider))]
		[ExecuteAlways]
		public class GhostRoom : Room
		{
				public static bool showOrbPositions = false;

				[SerializeField] private Color roomColor;
				[SerializeField] private MeshRenderer meshRenderer;
				[SerializeField] private float ghostOrbY;
				[SerializeField] private bool updateHitlines = true;
				[SerializeField] private Color hitlineColor = new Color(1, 1, 1, 0.2f);

				private Material meshMaterial;

				private bool hitPositionUpdate;
				private List<(Vector3 source, Vector3 hit)> hitPositions;
				private int randomPositionIndex = -1;
				private readonly Dictionary<int, GhostBehaviour> ghosts = new Dictionary<int, GhostBehaviour>();

				private BoxCollider Collider { get; set; }

				public Vector3 Size => Collider.bounds.size;

				public float GhostOrbY => ghostOrbY;

				public Color Color => this.roomColor;

				public IReadOnlyList<GhostBehaviour> GetGhosts() => ghosts.Values.ToList().AsReadOnly();

				private void Awake()
				{
						Collider = GetComponent<BoxCollider>();
				}

				#region UNITY_EDITOR RANGE
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
								hitPositions = UpdateHitLines(0.5f, 0.5f);

								for (int i = 0; i < hitPositions.Count; i++)
								{
										(Vector3 source, Vector3 hit) = hitPositions[i];

										// this call will add the position at that index position
										if (IsGizmoHitLineMatch())
										{
												Handles.color = Color.yellow;
										}
										else
										{
												Handles.color = hitlineColor;
										}

										if (Application.isPlaying is false && showOrbPositions)
										{
												Handles.DrawLine(source, hit);
										}
								}
						}
				}
#endif
				#endregion

				public Vector3 GetRandomOrbPos()
				{
						UpdateHitlines();
						int index = Random.Range(0, hitPositions.Count);
						randomPositionIndex = index;
						return hitPositions[index].source;
				}

				public void UpdateHitlines()
				{
						hitPositions = UpdateHitLines(0.5f, 0.5f);
				}

				private List<(Vector3 source, Vector3 hit)> UpdateHitLines(float offsetX, float offsetZ)
				{
						hitPositionUpdate = true;
						hitPositions = new List<(Vector3 source, Vector3 hit)>();

						Vector3 size = Collider.bounds.size;
						int amountX = Mathf.FloorToInt((size.x / offsetX) / 2);
						int amountZ = Mathf.FloorToInt((size.z / offsetZ) / 2);
						Vector3 position = transform.position;
						position.y += ghostOrbY;

						// from center:
						if (CheckHitGround(position, 0, 0, out var orgin, out var originHit))
						{
								hitPositions.Add((orgin, originHit));
						}

						for (int x = 0; x < amountX; x++)
						{
								for (int z = 0; z < amountZ; z++)
								{
										if (x == 0 && z == 0) continue;

										// right forward
										if (CheckHitGround(position, offsetX * x, offsetZ * z, out var sourceA, out var hitA))
										{
												hitPositions.Add((sourceA, hitA));
										}

										// left forward
										if (CheckHitGround(position, offsetX * -x, offsetZ * z, out var sourceB, out var hitB))
										{
												hitPositions.Add((sourceB, hitB));
										}

										// right back
										if (CheckHitGround(position, offsetX * x, offsetZ * -z, out var sourceC, out var hitC))
										{
												hitPositions.Add((sourceC, hitC));
										}

										// left back
										if (CheckHitGround(position, offsetX * -x, offsetZ * -z, out var sourceD, out var hitD))
										{
												hitPositions.Add((sourceD, hitD));
										}
								}
						}
						hitPositionUpdate = false;
						return hitPositions;
				}

				private bool CheckHitGround(Vector3 center, float xOffset, float zOffset,
						out Vector3 collisionDown,
						out Vector3 collistionHit)
				{
						Ray ray = new Ray(new Vector3(center.x + xOffset, center.y, center.z + zOffset), Vector3.down);
						collisionDown = ray.origin;
						if (Physics.Raycast(ray, out RaycastHit hit, 10))
						{
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

				private void Start()
				{
						// no trigger, no force - for Y only
						Collider.isTrigger = true;

						if (meshRenderer != null)
						{
								meshMaterial = meshRenderer.sharedMaterial;
								meshRenderer.material = meshMaterial;
								meshMaterial.color = roomColor;
						}
				}

				private bool IsGhost(Collider other, out GhostBehaviour ghost)
				{
						ghost = null;
						return other.gameObject is { } && other.TryGetComponent(out ghost);
				}

				private static bool IsPlayer(Collider other)
				{
						return other.gameObject != null && other.TryGetComponent(out PlayerBehaviour player);
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
						else if (IsGhost(other, out GhostBehaviour ghost))
						{
								ghosts[ghost.GetInstanceID()] = ghost;
						}
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
						else if (IsGhost(other, out GhostBehaviour ghost))
						{
								ghosts.Remove(ghost.GetInstanceID());
						}
				}

				private void Update()
				{

				}
		}
}