using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Script.Behaviour
{
		[RequireComponent(typeof(SphereCollider))]
		public class GeisterorbBehaviour : MonoBehaviour
		{
				[SerializeField] private List<GhostRoom> existenceSpace;
				[SerializeField] [Min(0.1f)] private float fadeSpeed = 1.0f;
				[SerializeField] [Min(0)] private float moveSpeed = 5;
				[SerializeField] private bool canChangeRoom = false;

				private SphereCollider orbCollider;

				// change position, change room
				private float changeRoomRandomNextTime;
				private bool changeRoomMovementEnabled;
				private Vector3? flyPositionOld;
				private Vector3? flyToPosition;

				public GhostRoom RoomActive { get; private set; }

				private void Awake()
				{
						orbCollider = GetComponent<SphereCollider>();
						orbCollider.isTrigger = true;
				}

				private void Start()
				{
						ChangeRoomExplicit();
				}

				private void ChangeRoomExplicit()
				{
						bool oldValue = canChangeRoom;
						canChangeRoom = true;
						TryChangeRoom();
						canChangeRoom = oldValue;
				}

				private void TryChangeRoom()
				{
						if (Time.timeSinceLevelLoad >= changeRoomRandomNextTime
								&& canChangeRoom)
						{
								Debug.Log("Orb change Room");
								RoomActive = existenceSpace[Random.Range(0, existenceSpace.Count)];
								changeRoomRandomNextTime = Time.timeSinceLevelLoad + Random.Range(15, 50);
								// center at room
								changeRoomMovementEnabled = true;
								flyToPosition = RoomActive.transform.position;
						}
				}

				private void Update()
				{
						if (changeRoomMovementEnabled && flyToPosition.HasValue)
						{
								// move
								if (MoveToRoomPosition(flyToPosition.Value))
								{
										// on end:
										changeRoomMovementEnabled = false;
								}
						}
						else
						{
								if (flyToPosition is null)
								{
										Vector3 pos = GetNextPositionInsideRoom();
										if (flyPositionOld is null || (flyPositionOld.Value - pos).magnitude >= moveSpeed / 2)
										{
												flyToPosition = pos;
												flyPositionOld = pos;
										}
								}
								// move
								else if (MoveToRoomPosition(flyToPosition.Value))
								{
										// on end:
										flyToPosition = null;
								}
						}

						TryChangeRoom();
						UpdateInvariantPosition();
				}

				private Vector3 GetNextPositionInsideRoom()
				{
						return RoomActive.GetRandomOrbPos();

						Vector3 size = RoomActive.Size;
						float radius = Mathf.Min(size.x / 2, size.z / 2);
						Vector3 pos = RoomActive.transform.position + Random.insideUnitSphere * radius;
						pos.y = RoomActive.GhostOrbY;
						return pos;
				}

				private bool MoveToRoomPosition(Vector3 worldPositionDestintion)
				{
						transform.position = Vector3.Lerp(transform.position, worldPositionDestintion, moveSpeed * Time.deltaTime);

						if (Vector3.Distance(worldPositionDestintion, transform.position) < 0.1f)
						{
								transform.position = worldPositionDestintion;
								return true;
						}
						return false;
				}

				private float angleUp;
				private float angleRight;

				private void UpdateInvariantPosition()
				{
						const float distanceUp = 0.5f;
						const float speedUp = 2;
						angleUp += Time.deltaTime * speedUp;
						angleUp %= 360;
						transform.position = Vector3.Lerp(transform.position,
								transform.position + transform.up * distanceUp * Mathf.Sin(angleUp * Mathf.Deg2Rad),
								Time.deltaTime);

						const float distanceRight = 1;
						const float speedRight = 2;
						angleRight += Time.deltaTime * speedRight;
						angleRight %= 360;
						transform.position = Vector3.Lerp(transform.position,
								transform.position + transform.right * distanceRight * Mathf.Cos(angleRight * Mathf.Deg2Rad),
								Time.deltaTime);
				}
		}
}