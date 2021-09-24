using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets
{
		[ExecuteAlways]
		public class InputControlManagerSingleton : MonoBehaviour, IInputControls
		{
				public static IInputControls Instance { get; private set; }
				private ControlMappingInputAction inputActions;

				private InputActionPhase runButtonPressedOrHoldPhase;
				private InputActionPhase placeEquipmentButtonPressedPhase;
				private InputActionPhase crouchButtonPressedPhase;
				private InputActionPhase crosshairTargetInteractionButtonPressedPhase;
				private InputActionPhase dropEquipmentButtonPressedPhase;
				private InputActionPhase debugHuntToggleOnOffPhase;
				private InputActionPhase interactButtonPressedPhase;
				private InputActionPhase editorStopCameraPhase;
				private InputActionPhase exitGameButtonPhase;
				public bool IsEnabled { get; private set; }

				public Vector2 InputAxis => inputActions.Player.Movement.ReadValue<Vector2>();

				public float Horizonal => InputAxis.x;

				public float Vertical => InputAxis.y;

				public Vector2 MouseDelta => inputActions.Player.Looking.ReadValue<Vector2>();

				public float MouseDeltaX => MouseDelta.x;

				public float MouseDeltaY => MouseDelta.y;

				public bool ExitGameButton => inputActions.Player.ExitGame.IsButtonReleased(ref exitGameButtonPhase);

				public bool EditorStopCamera => inputActions.Player.EditorStopRotateCamera.IsButtonReleased(ref editorStopCameraPhase);

				/// <summary>
				/// Controller: /_\
				/// </summary>
				public bool InteractButtonPressed => inputActions.Player.EquipmentToggle.IsButtonDown(ref interactButtonPressedPhase);
				
				/// <summary>
				/// Controller: X
				/// </summary>
				public bool CrosshairTargetInteractionButtonPressed => inputActions.Player.Interaction.IsButtonDown(ref crosshairTargetInteractionButtonPressedPhase);

				public bool DebugHuntToggleOnOff => inputActions.Player.HuntToggleDebug.IsButtonDown(ref debugHuntToggleOnOffPhase);

				public bool DropEquipmentButtonPressed => inputActions.Player.EquipmentDrop.IsButtonReleased(ref dropEquipmentButtonPressedPhase);


				public bool CrouchButtonPressed => inputActions.Player.Crouch.IsButtonHold(ref crouchButtonPressedPhase);

				public bool RunButtonPressedOrHold => inputActions.Player.Run.IsButtonHold(ref runButtonPressedOrHoldPhase);

				public bool PlaceEquipmentButtonPressing => inputActions.Player.PlaceItem.IsButtonDown(ref placeEquipmentButtonPressedPhase);

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