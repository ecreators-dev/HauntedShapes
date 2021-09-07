// GENERATED AUTOMATICALLY FROM 'Assets/Control Mapping (Input Action).inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ControlMappingInputAction : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ControlMappingInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Control Mapping (Input Action)"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""49e128fd-c973-48c6-a0fa-b5906880a32a"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""75a39aae-6dc7-47ba-a7c1-438a0a713922"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Looking"",
                    ""type"": ""PassThrough"",
                    ""id"": ""493f5146-1eec-41b2-b2ce-e3358dfb211b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Exit Game"",
                    ""type"": ""Button"",
                    ""id"": ""752302a7-2463-485e-8542-1729eaf097c8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Editor Stop Rotate Camera"",
                    ""type"": ""Button"",
                    ""id"": ""2c7210b7-014f-478c-afd5-3b96f320c4f7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""26318fdb-4333-4428-9192-00589aed46dc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Equipment Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""fd3ae3b8-52ae-481a-9aa2-a681193030fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Equipment Drop"",
                    ""type"": ""Button"",
                    ""id"": ""bfd09ad3-e785-4050-9099-0e11fb5e8674"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Hunt Toggle (Debug)"",
                    ""type"": ""Button"",
                    ""id"": ""55fba300-6b61-4a38-97fc-8ac581f64d9f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Place Item"",
                    ""type"": ""Button"",
                    ""id"": ""96c0c9a0-39df-43de-9585-191f7d26acaf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""e4c16749-fb27-453d-bef8-4a525af621aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""182c65d0-73b1-47c1-9851-aff4ae1a8010"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.1,pressPoint=0.001)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""420368da-ef2c-47d5-acbe-9e579d7dc74a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f99493b5-472d-437f-83bf-0e626a04d1d5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""fc6b7582-3794-4924-893a-346c2c46d8c3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""70430a83-1705-481d-a8c1-bc90632b00ff"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""118f23f3-9beb-4114-9ce9-a63b21180f2c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4cfd2848-8ec0-413e-919a-c0ac7161f585"",
                    ""path"": ""<DualShockGamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52bfa26a-3dbf-4354-a094-393f6f54c0d0"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Looking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48b8a3d7-d79b-4878-bfa7-1352f7dac07e"",
                    ""path"": ""<DualShockGamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Looking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""341a3316-8a61-492b-8595-c42ec9f00483"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit Game"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f45cc3a-123e-48a2-9703-0d6c421fa979"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit Game"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03a07993-4a1d-4b00-95a6-8824faffa225"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Editor Stop Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f6b7225-a6f2-4f22-b71b-97df5caa2840"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Editor Stop Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ffb63846-b2db-40e9-bfd4-7f87fb22d373"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Equipment Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0927114-a34e-4f74-9951-2c601e5a8dfe"",
                    ""path"": ""<DualShockGamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Equipment Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93faaecc-6a7d-485e-8430-83c581937a01"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Hunt Toggle (Debug)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""afdbc34b-814a-40a1-ad5c-a211968a1187"",
                    ""path"": ""<DualShockGamepad>/touchpadButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hunt Toggle (Debug)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca66cb1a-e861-45a1-a7b4-5014b79b3c0e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Equipment Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""29f23c65-4b6c-43e3-993a-5dc97e7d1b82"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Equipment Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5c7a406-131c-4c46-93cf-2de3fd6b88a1"",
                    ""path"": ""<DualShockGamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Equipment Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac83b9a2-b318-4aa3-8c97-54e560c7fae6"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cb114669-1903-4695-b49b-770a3e03866f"",
                    ""path"": ""<DualShockGamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6670b3e-878c-492b-b2a3-5938389df315"",
                    ""path"": ""<DualShockGamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aeef0dbc-bd60-4823-94ae-f130a5a2bc7c"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3f5a3c3f-7f9d-44c1-b902-5cc9105970d4"",
                    ""path"": ""<DualShockGamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b35968e0-0783-414b-a659-1976362e2284"",
                    ""path"": ""<DualShockGamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3bb9d89-12c3-4d97-b664-09fefb5baf38"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9af0f257-88a3-4be2-abe4-7d27fcc4ebb8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e566ac89-27a0-457a-991e-40204231235d"",
                    ""path"": ""<DualShockGamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6bda184-88a9-49ad-9dd3-552e9e3b1020"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Place Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48239a39-e936-4ee9-97d4-5fdbea3d58b2"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""Place Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cf330cd-b1b3-4b77-96f9-4ab5ae9be702"",
                    ""path"": ""<DualShockGamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Place Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Classic"",
            ""bindingGroup"": ""Classic"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<DualShockGamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""bindingGroup"": ""Debug"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Looking = m_Player.FindAction("Looking", throwIfNotFound: true);
        m_Player_ExitGame = m_Player.FindAction("Exit Game", throwIfNotFound: true);
        m_Player_EditorStopRotateCamera = m_Player.FindAction("Editor Stop Rotate Camera", throwIfNotFound: true);
        m_Player_Interaction = m_Player.FindAction("Interaction", throwIfNotFound: true);
        m_Player_EquipmentToggle = m_Player.FindAction("Equipment Toggle", throwIfNotFound: true);
        m_Player_EquipmentDrop = m_Player.FindAction("Equipment Drop", throwIfNotFound: true);
        m_Player_HuntToggleDebug = m_Player.FindAction("Hunt Toggle (Debug)", throwIfNotFound: true);
        m_Player_PlaceItem = m_Player.FindAction("Place Item", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_Run = m_Player.FindAction("Run", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Looking;
    private readonly InputAction m_Player_ExitGame;
    private readonly InputAction m_Player_EditorStopRotateCamera;
    private readonly InputAction m_Player_Interaction;
    private readonly InputAction m_Player_EquipmentToggle;
    private readonly InputAction m_Player_EquipmentDrop;
    private readonly InputAction m_Player_HuntToggleDebug;
    private readonly InputAction m_Player_PlaceItem;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_Run;
    public struct PlayerActions
    {
        private @ControlMappingInputAction m_Wrapper;
        public PlayerActions(@ControlMappingInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Looking => m_Wrapper.m_Player_Looking;
        public InputAction @ExitGame => m_Wrapper.m_Player_ExitGame;
        public InputAction @EditorStopRotateCamera => m_Wrapper.m_Player_EditorStopRotateCamera;
        public InputAction @Interaction => m_Wrapper.m_Player_Interaction;
        public InputAction @EquipmentToggle => m_Wrapper.m_Player_EquipmentToggle;
        public InputAction @EquipmentDrop => m_Wrapper.m_Player_EquipmentDrop;
        public InputAction @HuntToggleDebug => m_Wrapper.m_Player_HuntToggleDebug;
        public InputAction @PlaceItem => m_Wrapper.m_Player_PlaceItem;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @Run => m_Wrapper.m_Player_Run;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Looking.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLooking;
                @Looking.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLooking;
                @Looking.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLooking;
                @ExitGame.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnExitGame;
                @ExitGame.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnExitGame;
                @ExitGame.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnExitGame;
                @EditorStopRotateCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEditorStopRotateCamera;
                @EditorStopRotateCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEditorStopRotateCamera;
                @EditorStopRotateCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEditorStopRotateCamera;
                @Interaction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteraction;
                @Interaction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteraction;
                @Interaction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteraction;
                @EquipmentToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentToggle;
                @EquipmentToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentToggle;
                @EquipmentToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentToggle;
                @EquipmentDrop.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentDrop;
                @EquipmentDrop.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentDrop;
                @EquipmentDrop.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentDrop;
                @HuntToggleDebug.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHuntToggleDebug;
                @HuntToggleDebug.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHuntToggleDebug;
                @HuntToggleDebug.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHuntToggleDebug;
                @PlaceItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlaceItem;
                @PlaceItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlaceItem;
                @PlaceItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPlaceItem;
                @Crouch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Run.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRun;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Looking.started += instance.OnLooking;
                @Looking.performed += instance.OnLooking;
                @Looking.canceled += instance.OnLooking;
                @ExitGame.started += instance.OnExitGame;
                @ExitGame.performed += instance.OnExitGame;
                @ExitGame.canceled += instance.OnExitGame;
                @EditorStopRotateCamera.started += instance.OnEditorStopRotateCamera;
                @EditorStopRotateCamera.performed += instance.OnEditorStopRotateCamera;
                @EditorStopRotateCamera.canceled += instance.OnEditorStopRotateCamera;
                @Interaction.started += instance.OnInteraction;
                @Interaction.performed += instance.OnInteraction;
                @Interaction.canceled += instance.OnInteraction;
                @EquipmentToggle.started += instance.OnEquipmentToggle;
                @EquipmentToggle.performed += instance.OnEquipmentToggle;
                @EquipmentToggle.canceled += instance.OnEquipmentToggle;
                @EquipmentDrop.started += instance.OnEquipmentDrop;
                @EquipmentDrop.performed += instance.OnEquipmentDrop;
                @EquipmentDrop.canceled += instance.OnEquipmentDrop;
                @HuntToggleDebug.started += instance.OnHuntToggleDebug;
                @HuntToggleDebug.performed += instance.OnHuntToggleDebug;
                @HuntToggleDebug.canceled += instance.OnHuntToggleDebug;
                @PlaceItem.started += instance.OnPlaceItem;
                @PlaceItem.performed += instance.OnPlaceItem;
                @PlaceItem.canceled += instance.OnPlaceItem;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_ClassicSchemeIndex = -1;
    public InputControlScheme ClassicScheme
    {
        get
        {
            if (m_ClassicSchemeIndex == -1) m_ClassicSchemeIndex = asset.FindControlSchemeIndex("Classic");
            return asset.controlSchemes[m_ClassicSchemeIndex];
        }
    }
    private int m_DebugSchemeIndex = -1;
    public InputControlScheme DebugScheme
    {
        get
        {
            if (m_DebugSchemeIndex == -1) m_DebugSchemeIndex = asset.FindControlSchemeIndex("Debug");
            return asset.controlSchemes[m_DebugSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLooking(InputAction.CallbackContext context);
        void OnExitGame(InputAction.CallbackContext context);
        void OnEditorStopRotateCamera(InputAction.CallbackContext context);
        void OnInteraction(InputAction.CallbackContext context);
        void OnEquipmentToggle(InputAction.CallbackContext context);
        void OnEquipmentDrop(InputAction.CallbackContext context);
        void OnHuntToggleDebug(InputAction.CallbackContext context);
        void OnPlaceItem(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
    }
}
