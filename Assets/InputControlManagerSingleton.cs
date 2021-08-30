
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets
{
		[ExecuteAlways]
		public class InputControlManagerSingleton : MonoBehaviour, IInputControls
		{
				public static IInputControls Instance { get; private set; }
				private ControlMappingInputAction inputActions;

				public Vector2 InputAxis => inputActions.Player.Movement.ReadValue<Vector2>();

				public float Horizonal => InputAxis.x;

				public float Vertical => InputAxis.y;

				public Vector2 MouseDelta => inputActions.Player.Looking.ReadValue<Vector2>();

				public float MouseDeltaX => MouseDelta.x;

				public float MouseDeltaY => MouseDelta.y;

				public bool ExitGameButton => inputActions.Player.ExitGame.triggered;

				public bool EditorStopCamera => inputActions.Player.EditorStopRotateCamera.triggered;

				public bool IsEnabled { get; private set; }
				public bool PlayerToggleEquipmentOnOff
				{
						get
						{
								float value = inputActions.Player.EquipmentToggle.ReadValue<float>();
								return !(value is 0f);
						}
				}

				public bool DebugHuntToggleOnOff => inputActions.Player.HuntToggleDebug.triggered;

				public bool PlayerDropEquipment => inputActions.Player.EquipmentDrop.triggered;

				public bool InteractionCrosshairPressed => inputActions.Player.KeyboardInteraction.triggered;

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

				public static InputControls CreateInputControls()
				{
						return new InputControls(new ControlMappingInputAction());
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

		public class InputControls : IInputControls
		{
				private ControlMappingInputAction inputActions;

				public InputControls(ControlMappingInputAction controlMappingInputAction)
				{
						this.inputActions = controlMappingInputAction;
				}

				public void Enable()
				{
						IsEnabled = true;
						inputActions.Enable();
				}

				public void Disable()
				{
						IsEnabled = false;
						inputActions.Disable();
				}

				public Vector2 InputAxis => inputActions.Player.Movement.ReadValue<Vector2>();

				public float Horizonal => InputAxis.x;

				public float Vertical => InputAxis.y;

				public Vector2 MouseDelta => inputActions.Player.Looking.ReadValue<Vector2>();

				public float MouseDeltaX => MouseDelta.x;

				public float MouseDeltaY => MouseDelta.y;

				public bool ExitGameButton => inputActions.Player.ExitGame.triggered;

				public bool EditorStopCamera => inputActions.Player.EditorStopRotateCamera.triggered;

				public bool IsEnabled { get; private set; }

				public bool PlayerToggleEquipmentOnOff => inputActions.Player.EquipmentToggle.ReadValue<bool>();
				
				public bool PlayerDropEquipment => inputActions.Player.EquipmentDrop.triggered;

				public bool DebugHuntToggleOnOff => inputActions.Player.HuntToggleDebug.triggered;

				public bool InteractionCrosshairPressed => inputActions.Player.KeyboardInteraction.triggered;
		}
}