using UnityEngine;

namespace Assets.Script.Behaviour.FirstPerson
{
		[DisallowMultipleComponent]
		public class CameraFirstPerson : MonoBehaviour
		{
				[SerializeField] private MovementForPlayer character;
				[SerializeField] private float sensitivityGamepad = 2;
				[SerializeField] private float sensitivityMouse = 1 / 5f;
				[SerializeField] private float smoothing = 1.5f;
				[SerializeField] private float limitDown = -90;
				[SerializeField] private float limitUp = 90;
				[Range(0, 200)]
				[SerializeField] private float fieldOfView = 65;

				private Vector2 euler;
				private Vector2 frameVelocity;

				private bool GamepadDisconnected => this.GetGameController()?.IsGamepadDisconnected ?? false;

				private void Update()
				{
						Vector2 mouseDelta = this.InputControls().MouseDelta;
						float sensitivity = GetSensitivityByInputType();
						Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
						frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
						euler += frameVelocity;
						euler.y = Mathf.Clamp(euler.y, limitDown, limitUp);

						// Rotate camera up-down and controller left-right from velocity.
						Camera cam = CameraMoveType.Instance.GetCamera();
						cam.fieldOfView = fieldOfView;

						if (this.InputControls().StopCameraRotationButton)
						{
								ToggleStopRotation();
						}

						Transform camera = cam.transform;
						camera.localRotation = Quaternion.AngleAxis(-euler.y, Vector3.right);
						character.Transform.rotation = Quaternion.AngleAxis(euler.x, Vector3.up);
				}

				private void ToggleStopRotation()
				{
						Model.IGameController gameController = this.GetGameController();
						gameController.SetStopCameraEdit(!gameController.IsCameraRotateStop);

						Debug.Log($"Camera rotation stop: {gameController.IsCameraRotateStop}");
				}

				private float GetSensitivityByInputType()
				{
						float sensitivity;
						if (GamepadDisconnected)
						{
								sensitivity = sensitivityMouse;
						}
						else
						{
								sensitivity = sensitivityGamepad;
						}

						return sensitivity;
				}
		}
}