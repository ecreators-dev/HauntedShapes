using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		/// <summary>
		/// Movement only
		/// </summary>
		[RequireComponent(typeof(CharacterController))]
		[RequireComponent(typeof(Rigidbody))]
		[DisallowMultipleComponent]
		public class MovementForPlayer : MonoBehaviour
		{
				[Min(0)]
				[SerializeField] private float moveSpeed = 3;
				[Min(0)]
				[SerializeField] private float runSpeed = 5;
				[SerializeField] private CharacterController character;
				[SerializeField] private Animator animator;

				private float targetSpeed;
				private bool running;
				private bool crouching;
				private float actualSpeed;

				public Transform Transform { get; private set; }

				private Rigidbody Rigidbody { get; set; }

				private void Awake()
				{
						Transform = transform;
						Rigidbody = GetComponent<Rigidbody>();
				}

				private void Start()
				{
						// fix for character controller
						Rigidbody.isKinematic = true;
				}

				private void Update()
				{
						// can be nagative!
						UpdateTargetSpeed();

						float multiplier = 2;
						if (this.InputControls().Horizonal == 0
								&& this.InputControls().Vertical == 0)
						{
								targetSpeed = 0;
								multiplier = 50;
						}

						actualSpeed = Mathf.Lerp(actualSpeed, targetSpeed, Time.deltaTime * multiplier);
						animator.SetFloat("Speed", actualSpeed);
						animator.SetBool("Crouch", crouching);
				}

				private void UpdateTargetSpeed()
				{
						targetSpeed = moveSpeed;

						IInputControls inputControls = this.InputControls();
						running = inputControls.RunButton;
						if (running)
						{
								Debug.Log($"Running pressed {Time.realtimeSinceStartup}");
								targetSpeed = runSpeed;
						}

						crouching = inputControls.CrouchButton;
						if (crouching)
						{
								Debug.Log($"Crouching hold {Time.realtimeSinceStartup}");
								targetSpeed = 0;
						}
				}

				private void FixedUpdate()
				{
						IInputControls inputControls = this.InputControls();
						if (inputControls == null)
								return;

						float vertical = inputControls.Vertical;
						float horizontal = inputControls.Horizonal;

						Vector3 forwardMove = (Transform.forward * vertical) * actualSpeed;
						var movement = forwardMove;

						Vector3 sideMove = (Transform.right * horizontal) * actualSpeed;
						movement += sideMove;
						movement += Vector3.down * 9.81f;

						character.Move(movement * Time.fixedDeltaTime);
						character.detectCollisions = true;
				}
		}
}