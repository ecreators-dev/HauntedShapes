﻿using Assets.Door;
using Assets.Script.Controller;

using System;

using UnityEditor;

using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[RequireComponent(typeof(Rigidbody))]
		[DisallowMultipleComponent]
		public class FirstPersonController : MonoBehaviour
		{
				[SerializeField] private CameraMoveType camType = new CameraMoveType(CameraMoveType.TypeEnum.NOT_BUMPING);
				private Camera cam;

				[Range(0, 200)]
				[SerializeField] private float fieldOfView = 65;

				[Header("Movement/Looking")]
				[SerializeField] private bool canMove = true;
				[ReadOnlyDependingOnBoolean(nameof(canMove), true)] // this is readonly if canMove is false
				[SerializeField] private float moveSpeed = 70;
				[ReadOnlyDependingOnBoolean(nameof(canMove), true)] // this is readonly if canMove is false
				[SerializeField] private float runSpeed = 130;
				[SerializeField] private CharacterController characterController;

				[Header("Animation")]
				[SerializeField] private Animator animator;
				[SerializeField] private string crouchBooleanName = "Crouch";
				[SerializeField] private string moveSpeedName = "Speed";

				private float actualSpeed;

				private bool stopMovementInputs;
				private float targetSpeed;
				private float oldX;
				private bool crouching;
				private bool running;
				private Vector3 teleportedPosition;
				private TriggerTeleporter teleportedSource;

				private Transform Transform { get; set; }

				private void Awake()
				{
						Transform = transform;
				}

				private void Update()
				{
						cam = camType.GetCamera();

						HandleExitGameOrPlaymode();
						HandleCameraView();
						HandleCameraEditorStopOnButton();

						if (stopMovementInputs || canMove is false)
								return;

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
						animator.SetFloat(moveSpeedName, actualSpeed);
						animator.SetBool(crouchBooleanName, crouching);
				}

				private void FixedUpdate()
				{
						IInputControls inputControls = this.InputControls();
						float vertical = inputControls.Vertical;
						float horizontal = inputControls.Horizonal;

						Vector3 forwardMove = (Transform.forward * vertical) * actualSpeed;
						var movement = forwardMove;

						Vector3 sideMove = (Transform.right * horizontal) * actualSpeed;
						movement += sideMove;
						movement += Vector3.down * 9.81f;

						characterController.Move(movement * Time.fixedDeltaTime);
						if (teleportedSource != null)
						{
								Transform.position = teleportedPosition;
								teleportedSource = null;
						}
				}

				public void SetTeleported(TriggerTeleporter triggerTeleporter)
				{
						this.teleportedPosition = Transform.position;
						this.teleportedSource = triggerTeleporter;
				}

				private void OnCollisionEnter(Collision collision)
				{
						// immer, wenn falsch bewegt wird
						if (oldX != 0f && Transform.position.x < oldX)
						{
								Debug.Log($"{Time.realtimeSinceStartup} - [Player][Collision]: {collision.collider.GetType().Name} = {collision.gameObject.name}");
						}
						oldX = Transform.position.x;
				}

				private void HandleCameraEditorStopOnButton()
				{
						if (this.InputControls().EditorStopCamera && CanPlay())
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
						if (this.InputControls().ExitGameButton)
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

				private void UpdateTargetSpeed()
				{
						targetSpeed = this.moveSpeed;

						IInputControls inputControls = this.InputControls();
						running = inputControls.RunButtonPressedOrHold;
						if (running)
						{
								Debug.Log($"Running pressed {Time.realtimeSinceStartup}");
								targetSpeed = runSpeed;
						}

						crouching = inputControls.CrouchButtonPressed;
						if (crouching)
						{
								Debug.Log($"Crouching hold {Time.realtimeSinceStartup}");
								targetSpeed = 0;
						}
				}
		}
}
