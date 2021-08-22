using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[RequireComponent(typeof(Rigidbody))]
		public class FirstPersonController : MonoBehaviour
		{
				public Camera cam;
				public Transform body;

				[SerializeField]
				private float mouseSensity = 30;

				[SerializeField]
				[Range(0, 200)]
				private float fieldOfView = 65;

				[SerializeField]
				private float moveSpeed = 7;
				private Vector3 startingRotation;
				private Rigidbody rigidBody;
				private bool stopCamera;
				private IInputControls controls;
				private Vector3 oldMousePosition;
				private bool useOldInputSystem;

				public IInputControls Input => controls ??= this.InputControls();
				private bool RequireOldInputSystem => Input.IsEnabled is false;

				private void Awake()
				{
						startingRotation = body.localRotation.eulerAngles;
						rigidBody = GetComponent<Rigidbody>();
						rigidBody.useGravity = false;
				}

#if UNITY_EDITOR
				private void OnDrawGizmos()
				{

				}
#endif

				private void Update()
				{
						if (Input is null)
								return;

						HandleExitGameOrPlaymode();
						HandleCameraView();
						HandleCameraEditorStopOnButton();

						if (stopCamera)
								return;

						UpdateMovement();
				}

				private void UpdateMovement()
				{
						RotateCamera();
				}

				private void FixedUpdate()
				{
						MoveCamera();
				}

				private void HandleCameraEditorStopOnButton()
				{
						if (controls.EditorStopCamera && CanPlay())
						{
								stopCamera = !stopCamera;
								this.GetGameController()?.SetStopCameraEdit(stopCamera);
						}
				}

				private void HandleCameraView()
				{
						cam.fieldOfView = fieldOfView;
				}

				private void HandleExitGameOrPlaymode()
				{
						if (ExitGameButton)
						{
#if UNITY_EDITOR
								EditorApplication.ExitPlaymode();
#else
								Application.Quit();
#endif
						}
				}

				private bool ExitGameButton => RequireOldInputSystem ? UnityEngine.Input.GetKeyDown(KeyCode.Escape) : Input.ExitGameButton;

				private bool CanPlay()
				{
#if UNITY_EDITOR
						return EditorApplication.isPlaying;
#else
						return true;
#endif
				}

				private void MoveCamera()
				{
						float vertical = Input.Vertical;
						if (vertical > 0)
						{
								rigidBody.MovePosition(transform.position + transform.forward.normalized * moveSpeed * Time.deltaTime);
						}
						else if (vertical < 0)
						{
								rigidBody.MovePosition(transform.position + transform.forward.normalized * -moveSpeed * Time.deltaTime);
						}

						float horizontal = Input.Horizonal;
						if (horizontal < 0)
						{
								rigidBody.MovePosition(transform.position + transform.right.normalized * -moveSpeed * Time.deltaTime);
						}
						else if (horizontal > 0)
						{
								rigidBody.MovePosition(transform.position + transform.right.normalized * moveSpeed * Time.deltaTime);
						}

						// slow down fast
						rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime * 0.2f);
				}

				private void RotateCamera()
				{
						float mouseDeltaY = Input.MouseDeltaY;
						float mouseDeltaX = Input.MouseDeltaX;

						if (RequireOldInputSystem)
						{
								if (useOldInputSystem is false)
								{
										Debug.Log("Need to use old input system. No use any more.");
										return;
								}

								useOldInputSystem = true;
								Vector3 currentPos = UnityEngine.Input.mousePosition;
								var delta = currentPos - oldMousePosition;
								mouseDeltaY = delta.y;
								mouseDeltaX = delta.x;
								oldMousePosition = currentPos;
						}
						else
						{
								useOldInputSystem = false;
						}

						float deltaTilt = -1 * mouseDeltaY * mouseSensity * Time.deltaTime;
						float deltaPan = +1 * mouseDeltaX * mouseSensity * Time.deltaTime;

						startingRotation.x += deltaPan;
						startingRotation.y += deltaTilt;

						LimitTilting();

						body.rotation = Quaternion.Euler(startingRotation.y, startingRotation.x, 0);
				}

				private void LimitTilting()
				{
						const int UP = -66;
						const int DOWN = 86;
						startingRotation.y = Mathf.Clamp(startingRotation.y, UP, DOWN);
				}
		}
}
