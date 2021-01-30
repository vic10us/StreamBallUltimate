// GENERATED AUTOMATICALLY FROM 'Assets/Settings/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Settings"",
            ""id"": ""f0ab6d3b-bf36-4b2c-89c2-e111726a39b1"",
            ""actions"": [
                {
                    ""name"": ""PlayGame"",
                    ""type"": ""Button"",
                    ""id"": ""bfa4d777-d28e-4b58-818a-574f3653a2c8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""StopGame"",
                    ""type"": ""Button"",
                    ""id"": ""8a570270-12a8-4e7f-98fc-5c0f7b1329d7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ClearMarbles"",
                    ""type"": ""Button"",
                    ""id"": ""19b7bfcb-771e-4e00-8eda-9cf4c1d2454e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ResetShop"",
                    ""type"": ""Button"",
                    ""id"": ""1b1273ec-c096-4af1-ad20-742d38b90384"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""eda552a4-4859-4ee6-a60b-ce6aa129831d"",
                    ""path"": ""<Keyboard>/#(P)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc796a3b-4e64-4626-9e14-3775baafe5b5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StopGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""00af6d70-c708-4dd0-8351-eee2cb952d3e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClearMarbles"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f069927c-ccf7-433e-8122-28600d812885"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetShop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Settings
        m_Settings = asset.FindActionMap("Settings", throwIfNotFound: true);
        m_Settings_PlayGame = m_Settings.FindAction("PlayGame", throwIfNotFound: true);
        m_Settings_StopGame = m_Settings.FindAction("StopGame", throwIfNotFound: true);
        m_Settings_ClearMarbles = m_Settings.FindAction("ClearMarbles", throwIfNotFound: true);
        m_Settings_ResetShop = m_Settings.FindAction("ResetShop", throwIfNotFound: true);
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

    // Settings
    private readonly InputActionMap m_Settings;
    private ISettingsActions m_SettingsActionsCallbackInterface;
    private readonly InputAction m_Settings_PlayGame;
    private readonly InputAction m_Settings_StopGame;
    private readonly InputAction m_Settings_ClearMarbles;
    private readonly InputAction m_Settings_ResetShop;
    public struct SettingsActions
    {
        private @Controls m_Wrapper;
        public SettingsActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @PlayGame => m_Wrapper.m_Settings_PlayGame;
        public InputAction @StopGame => m_Wrapper.m_Settings_StopGame;
        public InputAction @ClearMarbles => m_Wrapper.m_Settings_ClearMarbles;
        public InputAction @ResetShop => m_Wrapper.m_Settings_ResetShop;
        public InputActionMap Get() { return m_Wrapper.m_Settings; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SettingsActions set) { return set.Get(); }
        public void SetCallbacks(ISettingsActions instance)
        {
            if (m_Wrapper.m_SettingsActionsCallbackInterface != null)
            {
                @PlayGame.started -= m_Wrapper.m_SettingsActionsCallbackInterface.OnPlayGame;
                @PlayGame.performed -= m_Wrapper.m_SettingsActionsCallbackInterface.OnPlayGame;
                @PlayGame.canceled -= m_Wrapper.m_SettingsActionsCallbackInterface.OnPlayGame;
                @StopGame.started -= m_Wrapper.m_SettingsActionsCallbackInterface.OnStopGame;
                @StopGame.performed -= m_Wrapper.m_SettingsActionsCallbackInterface.OnStopGame;
                @StopGame.canceled -= m_Wrapper.m_SettingsActionsCallbackInterface.OnStopGame;
                @ClearMarbles.started -= m_Wrapper.m_SettingsActionsCallbackInterface.OnClearMarbles;
                @ClearMarbles.performed -= m_Wrapper.m_SettingsActionsCallbackInterface.OnClearMarbles;
                @ClearMarbles.canceled -= m_Wrapper.m_SettingsActionsCallbackInterface.OnClearMarbles;
                @ResetShop.started -= m_Wrapper.m_SettingsActionsCallbackInterface.OnResetShop;
                @ResetShop.performed -= m_Wrapper.m_SettingsActionsCallbackInterface.OnResetShop;
                @ResetShop.canceled -= m_Wrapper.m_SettingsActionsCallbackInterface.OnResetShop;
            }
            m_Wrapper.m_SettingsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PlayGame.started += instance.OnPlayGame;
                @PlayGame.performed += instance.OnPlayGame;
                @PlayGame.canceled += instance.OnPlayGame;
                @StopGame.started += instance.OnStopGame;
                @StopGame.performed += instance.OnStopGame;
                @StopGame.canceled += instance.OnStopGame;
                @ClearMarbles.started += instance.OnClearMarbles;
                @ClearMarbles.performed += instance.OnClearMarbles;
                @ClearMarbles.canceled += instance.OnClearMarbles;
                @ResetShop.started += instance.OnResetShop;
                @ResetShop.performed += instance.OnResetShop;
                @ResetShop.canceled += instance.OnResetShop;
            }
        }
    }
    public SettingsActions @Settings => new SettingsActions(this);
    public interface ISettingsActions
    {
        void OnPlayGame(InputAction.CallbackContext context);
        void OnStopGame(InputAction.CallbackContext context);
        void OnClearMarbles(InputAction.CallbackContext context);
        void OnResetShop(InputAction.CallbackContext context);
    }
}
