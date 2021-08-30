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
                    ""interactions"": """"
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
                    ""name"": ""EquipmentToggle"",
                    ""type"": ""Button"",
                    ""id"": ""fd3ae3b8-52ae-481a-9aa2-a681193030fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EquipmentDrop"",
                    ""type"": ""Button"",
                    ""id"": ""bfd09ad3-e785-4050-9099-0e11fb5e8674"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HuntToggle (Debug)"",
                    ""type"": ""Button"",
                    ""id"": ""55fba300-6b61-4a38-97fc-8ac581f64d9f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Keyboard Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""26318fdb-4333-4428-9192-00589aed46dc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
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
                    ""id"": ""341a3316-8a61-492b-8595-c42ec9f00483"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
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
                    ""id"": ""ffb63846-b2db-40e9-bfd4-7f87fb22d373"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic"",
                    ""action"": ""EquipmentToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""014fc77a-9a17-471d-9f69-030184c984d5"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Debug;Classic"",
                    ""action"": ""HuntToggle (Debug)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""29f23c65-4b6c-43e3-993a-5dc97e7d1b82"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Classic;Debug"",
                    ""action"": ""EquipmentDrop"",
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
                    ""action"": ""Keyboard Interaction"",
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
        m_Player_EquipmentToggle = m_Player.FindAction("EquipmentToggle", throwIfNotFound: true);
        m_Player_EquipmentDrop = m_Player.FindAction("EquipmentDrop", throwIfNotFound: true);
        m_Player_HuntToggleDebug = m_Player.FindAction("HuntToggle (Debug)", throwIfNotFound: true);
        m_Player_KeyboardInteraction = m_Player.FindAction("Keyboard Interaction", throwIfNotFound: true);
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
    private readonly InputAction m_Player_EquipmentToggle;
    private readonly InputAction m_Player_EquipmentDrop;
    private readonly InputAction m_Player_HuntToggleDebug;
    private readonly InputAction m_Player_KeyboardInteraction;
    public struct PlayerActions
    {
        private @ControlMappingInputAction m_Wrapper;
        public PlayerActions(@ControlMappingInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Looking => m_Wrapper.m_Player_Looking;
        public InputAction @ExitGame => m_Wrapper.m_Player_ExitGame;
        public InputAction @EditorStopRotateCamera => m_Wrapper.m_Player_EditorStopRotateCamera;
        public InputAction @EquipmentToggle => m_Wrapper.m_Player_EquipmentToggle;
        public InputAction @EquipmentDrop => m_Wrapper.m_Player_EquipmentDrop;
        public InputAction @HuntToggleDebug => m_Wrapper.m_Player_HuntToggleDebug;
        public InputAction @KeyboardInteraction => m_Wrapper.m_Player_KeyboardInteraction;
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
                @EquipmentToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentToggle;
                @EquipmentToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentToggle;
                @EquipmentToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentToggle;
                @EquipmentDrop.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentDrop;
                @EquipmentDrop.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentDrop;
                @EquipmentDrop.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEquipmentDrop;
                @HuntToggleDebug.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHuntToggleDebug;
                @HuntToggleDebug.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHuntToggleDebug;
                @HuntToggleDebug.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHuntToggleDebug;
                @KeyboardInteraction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnKeyboardInteraction;
                @KeyboardInteraction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnKeyboardInteraction;
                @KeyboardInteraction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnKeyboardInteraction;
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
                @EquipmentToggle.started += instance.OnEquipmentToggle;
                @EquipmentToggle.performed += instance.OnEquipmentToggle;
                @EquipmentToggle.canceled += instance.OnEquipmentToggle;
                @EquipmentDrop.started += instance.OnEquipmentDrop;
                @EquipmentDrop.performed += instance.OnEquipmentDrop;
                @EquipmentDrop.canceled += instance.OnEquipmentDrop;
                @HuntToggleDebug.started += instance.OnHuntToggleDebug;
                @HuntToggleDebug.performed += instance.OnHuntToggleDebug;
                @HuntToggleDebug.canceled += instance.OnHuntToggleDebug;
                @KeyboardInteraction.started += instance.OnKeyboardInteraction;
                @KeyboardInteraction.performed += instance.OnKeyboardInteraction;
                @KeyboardInteraction.canceled += instance.OnKeyboardInteraction;
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
        void OnEquipmentToggle(InputAction.CallbackContext context);
        void OnEquipmentDrop(InputAction.CallbackContext context);
        void OnHuntToggleDebug(InputAction.CallbackContext context);
        void OnKeyboardInteraction(InputAction.CallbackContext context);
    }
}
