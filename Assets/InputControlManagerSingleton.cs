using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets
{
		[ExecuteAlways]
		public class InputControlManagerSingleton : MonoBehaviour, IInputControls
		{
				public static IInputControls Instance { get; private set; }
				private ControlMappingInputAction inputActions;

				public bool IsEnabled { get; private set; }

				public Vector2 InputAxis => inputActions.Player.Movement.ReadValue<Vector2>();

				public float Horizonal => InputAxis.x;

				public float Vertical => InputAxis.y;

				public Vector2 MouseDelta => inputActions.Player.Looking.ReadValue<Vector2>();

				public float MouseDeltaX => MouseDelta.x;

				public float MouseDeltaY => MouseDelta.y;

				public bool ExitGameButton => inputActions.Player.ExitGame.IsButtonReleased();

				public bool StopCameraRotationButton => inputActions.Player.EditorStopRotateCamera.IsButtonReleased();

				/// <summary>
				/// Controller: Triangle
				/// </summary>
				public bool InteractWithEquipmentButton => inputActions.Player.EquipmentRightHandToggle.IsButtonDown();
				
				/// <summary>
				/// Controller: Up Arrow or 1 (Hold)
				/// </summary>
				public bool InteractWithEquipmentUpButton => inputActions.Player.EquipmentHeadToggle.IsButtonHold();
				
				/// <summary>
				/// Controller: Cross
				/// </summary>
				public bool InteractWithCrosshairTargetButton => inputActions.Player.Interaction.IsButtonDown();

				public bool DebugHuntingButton => inputActions.Player.HuntToggleDebug.IsButtonDown();

				public bool DropEquipmentButton => inputActions.Player.EquipmentDrop.IsButtonReleased();


				public bool CrouchButton => inputActions.Player.Crouch.IsButtonHold();

				public bool RunButton => inputActions.Player.Run.IsButtonHold();

				public bool PlacingButton => inputActions.Player.PlaceItem.IsButtonDown();

				private void Awake()
				{
						if (Instance is null)
						{
								Init();
						}
						else if (Instance is InputControlManagerSingleton v && v != this)
						{
								Debug.Log($"Duplicated {nameof(InputControlManagerSingleton)}. Deleted.");
						}
				}

				private void Start()
				{
						if (Instance is null)
						{
								Instance = this;
						}
				}

				private void Init()
				{
						Debug.Log($"Instanciate {nameof(InputControlManagerSingleton)}");

						Instance = this;
						inputActions = new ControlMappingInputAction();
						inputActions.Enable();
				}

				private void OnValidate()
				{
						if (Instance is null)
						{
								Init();
						}
				}

				private void OnEnable()
				{
						inputActions?.Enable();
				}

				private void OnDisable()
				{
						inputActions?.Disable();
				}

				public void Disable()
				{
						IsEnabled = inputActions is { } ? false : IsEnabled;
						inputActions?.Disable();
				}

				public void Enable()
				{
						IsEnabled = inputActions is { } ? true : IsEnabled;
						inputActions.Enable();
				}
		}
}