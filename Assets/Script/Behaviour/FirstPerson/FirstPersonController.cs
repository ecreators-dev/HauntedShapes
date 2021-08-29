using Assets.Script.Controller;
using Assets.Script.GameMenu;
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
				[ReadOnlyDependingOnBoolean(nameof(canMove), true)] // this is readonly if canMove is false
				[SerializeField] private float moveSpeed = 70;
				[ReadOnlyDependingOnBoolean(nameof(canMove), true)] // this is readonly if canMove is false
				[SerializeField] private float runSpeed = 130;
				[SerializeField] private CharacterController characterController;
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
				private float targetSpeed;
				private Vector3 playerVelocity;

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

						if (Input is null)
								return;

						if (stopMovementInputs || canMove is false)
								return;

						MoveUpdate();
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

				private void MoveUpdate()
				{
						if (characterController.isGrounded)
						{
								playerVelocity.y = 0;
						}

						// can be nagative!
						float vertical = Input.Vertical;
						float horizontal = Input.Horizonal;
						UpdateTargetSpeed();

						Vector3 motion = new Vector2(horizontal, vertical);
						characterController.Move(motion * Time.deltaTime * actualSpeed);
						RigidBody.isKinematic = true;

						if (motion.Equals(Vector3.zero) is false)
						{
								Transform.forward = motion;
						}

						playerVelocity.y += -9.81f * Time.deltaTime;
						characterController.Move(playerVelocity * Time.deltaTime);

#if UNITY_EDITOR && false
						if (Application.isPlaying)
						{
								HauntedShapesGameEditorWindow existingWindow = EditorWindow.GetWindow<HauntedShapesGameEditorWindow>();
								existingWindow.UpdateSceneViewCamera(this.cam);
						}
#endif
				}

				private void UpdateTargetSpeed()
				{
						targetSpeed = this.moveSpeed;

						if (IsRunButtonPressed())
								targetSpeed = runSpeed;

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

						actualSpeed = Mathf.Lerp(actualSpeed, targetSpeed, Time.deltaTime);
						animator.SetFloat(moveSpeedName, actualSpeed);
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
		}
}
