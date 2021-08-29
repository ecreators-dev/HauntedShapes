using Assets.Script.Controller;
using Assets.Script.InspectorAttibutes;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[RequireComponent(typeof(Rigidbody))]
		[DisallowMultipleComponent]
		public class FirstPersonController : MonoBehaviour
		{
				[SerializeField] private Camera cam;
				[Range(0, 200)]
				[SerializeField] private float fieldOfView = 65;
				[SerializeField] private Renderer bodyRenderer;

				[Header("Movement/Looking")]
				#region can move inspector group
				[SerializeField] private bool canMove = true;
				[ReadOnlyDependingOnBoolean(nameof(canMove), true)]	// this is readonly if canMove is false
				[SerializeField] private float moveSpeed = 70;
				#endregion

				#region can turn inspector group
				[BeginGroup] // draws a line before drawing the property
				[SerializeField] private bool canTurn = true;
				[ReadOnlyDependingOnBoolean(nameof(canTurn), true)] // this is readonly if canTurn is false
				[Range(1, 30)]
				[SerializeField] private float mouseSensity = 10;
				[ReadOnlyDependingOnBoolean(nameof(canTurn), true)]
				[SerializeField] private float runSpeed = 130;
				#endregion

				[Header("Animation")]
				[SerializeField] private Animator animator;
				[SerializeField] private string crouchBooleanName = "Crouch"; // verify name in Start()!
				[SerializeField] private string moveSpeedName = "Speed"; // verify name in Start()!
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

				private void Start()
				{
						//bodyRenderer.enabled = false;
				}

				private void Update()
				{
						if (Input is null)
								return;

						HandleExitGameOrPlaymode();
						HandleCameraView();
						HandleCameraEditorStopOnButton();

						// stops inputs for movement and rotation
						if (stopMovementInputs || canTurn is false)
								return;

						RotateCamera();
				}

				private void FixedUpdate()
				{
						if (Input is null)
								return;

						if (stopMovementInputs || canMove is false)
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
						// can be nagative!
						float inputForward = Input.Vertical;
						float targetSpeed = this.moveSpeed;

						if (IsRunButtonPressed())
						{
								targetSpeed = runSpeed;
						}

						if (IsCrouchingButtonPressed())
						{
								targetSpeed = 0;
								if (animator.GetBool(crouchBooleanName) is false)
								{
										animator.SetBool(crouchBooleanName, true);
								}
						}
						else if (animator.GetBool(crouchBooleanName))
						{
								animator.SetBool(crouchBooleanName, false);
						}

						const int accellerationSteps = 15;
						const int deccellerationSteps = 5;
						float frames = accellerationSteps;
						if (inputForward == 0f)
						{
								targetSpeed = 0;
								frames = deccellerationSteps;
						}
						else
						{
								frames = accellerationSteps;
						}

						actualSpeed = Mathf.Lerp(actualSpeed, targetSpeed, Time.deltaTime / frames); // animation accelleration
						actualSpeed = Mathf.Min(actualSpeed, runSpeed);

						animator.SetFloat(moveSpeedName, actualSpeed * Time.deltaTime);

						if (inputForward > 0)
						{
								RigidBody.MovePosition(transform.position + transform.forward.normalized * actualSpeed * Time.deltaTime);
						}
						else if (inputForward < 0)
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
