using Assets;
using Assets.MiniFirstPersonController.Scripts;
using Assets.Script.Behaviour.FirstPerson;

using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonLook : MonoBehaviour
{
		[SerializeField] private Transform character;
		[SerializeField] private float sensitivity = 2;
		[SerializeField] private float smoothing = 1.5f;
		[SerializeField] private float limitDown = -90;
		[SerializeField] private float limitUp = 0;

		private Vector2 velocity;
		private Vector2 frameVelocity;
		private FirstPersonView fpsController;

		private Transform Transform { get; set; }
		
		private bool GamepadDisconnected => this.GetGameController()?.IsGamepadDisconnected ?? false;

		private void Reset()
		{
				// Get the character from the FirstPersonMovement in parents.
				character = GetComponentInParent<PlayerMovement>().transform;
				fpsController = GetComponentInParent<FirstPersonView>();
		}

		private void Awake()
		{
				Transform = transform;
		}

		private void Start()
		{
				// Lock the mouse cursor to the game screen.
				//Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
				// only in editor possible!
				if (this.GetGameController().IsCameraRotateStop)
				{
						return;
				}

				// Get smooth velocity.
				Vector2 mouseDelta = this.InputControls().MouseDelta; 
				//new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
																															
				// only mouse, then it is too fast:
				float sensitivity = this.sensitivity;
				if (GamepadDisconnected)
				{
						sensitivity /= 5f;
				}
				Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
				frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
				velocity += frameVelocity;
				velocity.y = Mathf.Clamp(velocity.y, limitDown, limitUp);

				// Rotate camera up-down and controller left-right from velocity.
				Transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
				character.rotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

				// fix rolling
				//var angles = Transform.eulerAngles;
				//angles.z = 0;
				//Transform.eulerAngles = angles;
		}
}
