using Assets.Script.Model;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class Door : MonoBehaviour, IToggleInteractable, IGate, IMonoBehaviour
		{
				[SerializeField]
				private float openYAngle = -90;

				public bool canOpen = true;
				public float openSpeed = 4;

				[SerializeField]
				private bool currentlyOpen = false;

				public bool IsOpened { get => currentlyOpen; }

				/// <summary>
				/// Pickable only if not in hand!
				/// </summary>
				public bool IsPickable => transform.parent == null;

				public string GameObjectName => this.GetGameObjectName();
				public string ImplementationTypeName => this.GetImplementationTypeName();

				private float initAngle;
				private float toAngle;
				private bool mouseDown;

				private void Start()
				{
						initAngle = transform.rotation.eulerAngles.y;
				}

				public void Open()
				{
						if (canOpen)
						{
								if (currentlyOpen)
										return;

								// toggle
								currentlyOpen = true;
								toAngle = initAngle + openYAngle;
								Debug.Log("Door open event");
						}
				}

				public void Close()
				{
						if (currentlyOpen)
						{
								// toggle
								currentlyOpen = false;
								toAngle = initAngle;
								Debug.Log("Door close event");
						}
				}

				private void Update()
				{
						transform.rotation = Quaternion.Lerp(transform.rotation,
								Quaternion.Euler(transform.eulerAngles.x, toAngle, transform.eulerAngles.z),
								Time.deltaTime * Mathf.Abs(openSpeed));
				}

				public void TouchOverUpdate()
				{
						if (mouseDown)
						{
								// ... Tür berühren ohne klick
						}

						Debug.Log("Door touch");
						mouseDown = false;
				}

				public void TouchClickUpdate()
				{
						mouseDown = true;
						if (IsOpened)
						{
								ToggleOff();
						}
						else
						{
								ToggleOn();
						}
				}

				public void ToggleOn() => Open();

				public void ToggleOff() => Close();

				public void Drop()
				{ }

				public void OnPickup(PlayerBehaviour player)
				{ }
		}
}