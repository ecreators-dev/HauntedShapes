using System;

using UnityEditor;

using UnityEngine;
using System.Linq;

namespace Assets.Script.Behaviour.FirstPerson
{
		[RequireComponent(typeof(Rigidbody))]
		public class FirstPersonController : MonoBehaviour
		{
				[SerializeField] private Camera cam;
				[Range(1, 30)]
				[SerializeField] private float mouseSensity = 10;
				[Range(0, 200)]
				[SerializeField] private float fieldOfView = 65;
				[SerializeField] private float moveSpeed = 70;
				[SerializeField] private float runSpeed = 130;

				[Header("Animation")]
				[SerializeField] private Animator animator;
				[SerializeField] private string crouchBooleanName = "Crouch"; // verify name in Start()!
				private int crouchBooleanNameHash = Animator.StringToHash("Crouch");
				[SerializeField] private string moveSpeedName = "Speed"; // verify name in Start()!
				private int moveSpeedNameHash = Animator.StringToHash("Speed");
				private float actualSpeed;

				private Vector3 startingRotation;
				private bool stopMovementInputs;
				private IInputControls controls;
				private Vector3 oldMousePosition;
				private bool useOldInputSystem;

				private Transform Transform { get; set; }
				private Rigidbody RigidBody { get; set; }

				public IInputControls Input => controls ??= this.InputControls();

				private bool RequireOldInputSystem => Input.IsEnabled is false;

				private bool ExitGameButton => RequireOldInputSystem ? UnityEngine.Input.GetKeyDown(KeyCode.Escape) : Input.ExitGameButton;

				private void Awake()
				{
						Transform = transform;
						startingRotation = Transform.localRotation.eulerAngles;
						RigidBody = GetComponent<Rigidbody>();
						RigidBody.useGravity = true;
				}

				private void Update()
				{
						if (Input is null)
								return;

						HandleExitGameOrPlaymode();
						HandleCameraView();
						HandleCameraEditorStopOnButton();

						// stops inputs for movement and rotation
						if (stopMovementInputs)
								return;

						UpdateMovement();
				}

				private void UpdateMovement()
				{
						RotateCamera();
				}

				private void FixedUpdate()
				{
						if (Input is null)
								return;

						if (stopMovementInputs)
								return;

						MoveFixedUpdate();
				}

				private void HandleCameraEditorStopOnButton()
				{
						if (controls.EditorStopCamera && CanPlay())
						{
								stopMovementInputs = !stopMovementInputs;
								this.GetGameController()?.SetStopCameraEdit(stopMovementInputs);
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

				private bool CanPlay()
				{
#if UNITY_EDITOR
						return EditorApplication.isPlaying;
#else
						return true;
#endif
				}

				private void MoveFixedUpdate()
				{
						float vertical = Input.Vertical;
						bool isMoving = vertical != 0;
						float moveSpeed = this.moveSpeed;

						if (IsRunButtonPressed())
						{
								moveSpeed = this.runSpeed;
						}

						if (IsCrouchingButtonPressed())
						{
								moveSpeed = 0;
								animator.SetBool(crouchBooleanNameHash, true);
						}
						else
						{
								animator.SetBool(crouchBooleanNameHash, false);
						}

						const int frames = 25;
						actualSpeed = Mathf.Lerp(actualSpeed, moveSpeed, Time.deltaTime / frames); // animation accelleration
						actualSpeed *= Mathf.Abs(vertical); // stick accelleration

						float maxSpeed = runSpeed;
						animator.SetFloat(moveSpeedNameHash, actualSpeed / maxSpeed);

						if (vertical > 0)
						{
								RigidBody.MovePosition(transform.position + transform.forward.normalized * actualSpeed * Time.deltaTime);
						}
						else if (vertical < 0)
						{
								RigidBody.MovePosition(transform.position + transform.forward.normalized * -actualSpeed * Time.deltaTime);
						}

						float horizontal = Input.Horizonal;
						if (horizontal < 0)
						{
								RigidBody.MovePosition(transform.position + transform.right.normalized * -actualSpeed * Time.deltaTime);
						}
						else if (horizontal > 0)
						{
								RigidBody.MovePosition(transform.position + transform.right.normalized * actualSpeed * Time.deltaTime);
						}

						// slow down fast
						RigidBody.velocity = Vector3.Lerp(RigidBody.velocity, Vector3.zero, Time.deltaTime * 0.2f);
				}

				private bool IsCrouchingButtonPressed()
				{
						return UnityEngine.Input.GetKeyDown(KeyCode.C);
				}

				private static bool IsRunButtonPressed()
				{
						return UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) ||
														UnityEngine.Input.GetKeyDown(KeyCode.RightShift);
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

						// both: no roll!
						// cam: no pan -> but tilt
						cam.transform.localRotation = Quaternion.Euler(startingRotation.y, 0, 0);

						// player: no tilt -> but pan
						Transform.localRotation = Quaternion.Euler(0, startingRotation.x, 0);
				}

				private void LimitTilting()
				{
						const int UP = -66;
						const int DOWN = 86;
						startingRotation.y = Mathf.Clamp(startingRotation.y, UP, DOWN);
				}
		}
}
