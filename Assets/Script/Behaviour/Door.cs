using Assets.Script.Components;

using System.Collections;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		[ExecuteAlways]
		public class Door : Interactible
		{
				[Tooltip("Sets an angle to open")]
				[SerializeField] private float openYAngle = -90;
				[Tooltip("Can be opened?")]
				[SerializeField] private bool unlocked = true;
				[Tooltip("How fast to open")]
				[SerializeField] private float openSpeed = 4;
				[Tooltip("Init open status")]
				[SerializeField] private bool currentlyOpen = false;

				private float initAngle;
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
						initAngle = Transform.eulerAngles.y;

						if (currentlyOpen)
						{
								Open();
						}
						else
						{
								Close();
						}
				}

				public void Unlock()
				{
						unlocked = true;
				}

				public void Lock()
				{
						unlocked = false;
						Close();
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
						toAngle = initAngle;
						Debug.Log("Door close event");
				}

#if UNITY_EDITOR
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

				private void Update()
				{
						UpdateAngle();
				}

				private void UpdateAngle()
				{
						Vector3 eulerAngles = Transform.eulerAngles;
						Transform.eulerAngles = Vector3.Lerp(eulerAngles,
								new Vector3(eulerAngles.x, toAngle, eulerAngles.z),
								Time.deltaTime * Mathf.Abs(openSpeed));
				}

				public override bool CanInteract(GameObject sender)
				{
						// always to open and close
						return unlocked;
				}

				public override void Interact(GameObject sender)
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
		}
}