using Assets.Script.Components;

using System.Collections;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[ExecuteAlways]
		public class Door : Interactible
		{
				[Tooltip("Sets an angle on close")]
				[SerializeField] private float closeYAngle = 0;
				[Tooltip("Sets an angle on open")]
				[SerializeField] private float openYAngle = -90;
				[Tooltip("Can be opened?")]
				[SerializeField] private bool unlocked = true;
				[Tooltip("How fast to open")]
				[SerializeField] private float openSpeed = 4;
				[Tooltip("Init open status")]
				[SerializeField] private bool currentlyOpen = false;

				[SerializeField] private GhostRoom roomToOpen;

				[Header("Preview (Editor)")]
				[SerializeField] private Color previewColor = new Color(1, 1, 0.3f, 0.2f);
				[Min(0)]
				[SerializeField] private float doorLength = 1.4f;

				private float toAngle;
				private bool lastOpen;
				private bool changedInFrame;

				public bool IsOpened { get => currentlyOpen; }

				protected Transform Transform { get; private set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void Start()
				{
						if (currentlyOpen)
						{
								Open();
						}
						else
						{
								Close();
						}
				}

				public new void Unlock()
				{
						unlocked = true;
						base.Unlock();
				}

				public new void Lock()
				{
						unlocked = false;
						Close();
						base.Lock();
				}

				public void Open()
				{
						if (unlocked)
						{
								// toggle
								currentlyOpen = true;
								toAngle = openYAngle;
								Debug.Log("Door open event");
						}
				}

				public void Close()
				{
						// toggle
						currentlyOpen = false;
						toAngle = closeYAngle;
						Debug.Log("Door close event");
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{
						Handles.color = previewColor;
						float angle = openYAngle - closeYAngle;
						float range = Mathf.Abs(angle);
						Vector3 normal = Quaternion.Euler(0, 90, 0) * Transform.parent.forward;
						Handles.DrawSolidArc(Transform.position, Transform.up,
								normal,
								range, // degrees
								doorLength);

						if (roomToOpen)
						{
								Handles.color = roomToOpen.Color;
								var center = GetComponent<BoxCollider>().bounds.center;
								Handles.DrawDottedLine(center, roomToOpen.transform.position, 0.25f);
								Handles.DrawSolidDisc(center, Transform.right, 0.3f);
								Handles.color = Color.white;
								Handles.Label(center, $"Eingang (Door): {roomToOpen.RoomName}");
						}
				}

				private void OnValidate()
				{
						if (currentlyOpen != lastOpen)
						{
								changedInFrame = true;
						}
				}

				private void OnRenderObject()
				{
						if (Application.isPlaying is false)
						{
								lastOpen = currentlyOpen;
								if (changedInFrame)
								{
										Debug.Log("Change Door Status");
										changedInFrame = !changedInFrame;

										if (currentlyOpen)
										{
												Open();
										}
										else
										{
												Close();
										}

										Vector3 eulerAngles = Transform.localEulerAngles;
										Transform.localEulerAngles = new Vector3(eulerAngles.x, toAngle, eulerAngles.z);
										changedInFrame = false;
								}

								return;
						}
				}
#endif

				protected override void Update()
				{
						base.Update();
						UpdateAngle();
				}

				private void UpdateAngle()
				{
						// no collision!
						Vector3 eulerAngles = Transform.eulerAngles;
						Transform.eulerAngles = Vector3.Lerp(eulerAngles,
								new Vector3(eulerAngles.x, toAngle, eulerAngles.z),
								Time.deltaTime * Mathf.Abs(openSpeed));
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						// always to open and close
						return unlocked;
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsOpened)
						{
								Close();
						}
						else
						{
								Open();
						}
				}

				protected override void OnHuntStart()
				{
						// nothing
				}

				protected override void OnHuntStop()
				{
						// nothing
				}

				public override string GetTargetName()
				{
						return GetLockCloseStatusName(roomToOpen?.RoomName ?? "??");
				}

				private string GetLockCloseStatusName(string name)
				{
						if (IsLocked)
						{
								name = $"{name} - locked";
						}
						else if (!IsOpened)
						{
								name = $"{name} - closed";
						}
						return name;
				}
		}
}