//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/GameManager/InputGameManage.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputGameManage: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputGameManage()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputGameManage"",
    ""maps"": [
        {
            ""name"": ""InputGame"",
            ""id"": ""ae63787b-949a-4b5f-a1e4-30d6facf9025"",
            ""actions"": [
                {
                    ""name"": ""Sonar"",
                    ""type"": ""Button"",
                    ""id"": ""4c9e2380-41fe-485c-a3d4-fa2c9b924718"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0995ca74-38a2-4324-8dfa-709d0b181e3b"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sonar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // InputGame
        m_InputGame = asset.FindActionMap("InputGame", throwIfNotFound: true);
        m_InputGame_Sonar = m_InputGame.FindAction("Sonar", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // InputGame
    private readonly InputActionMap m_InputGame;
    private List<IInputGameActions> m_InputGameActionsCallbackInterfaces = new List<IInputGameActions>();
    private readonly InputAction m_InputGame_Sonar;
    public struct InputGameActions
    {
        private @InputGameManage m_Wrapper;
        public InputGameActions(@InputGameManage wrapper) { m_Wrapper = wrapper; }
        public InputAction @Sonar => m_Wrapper.m_InputGame_Sonar;
        public InputActionMap Get() { return m_Wrapper.m_InputGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InputGameActions set) { return set.Get(); }
        public void AddCallbacks(IInputGameActions instance)
        {
            if (instance == null || m_Wrapper.m_InputGameActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InputGameActionsCallbackInterfaces.Add(instance);
            @Sonar.started += instance.OnSonar;
            @Sonar.performed += instance.OnSonar;
            @Sonar.canceled += instance.OnSonar;
        }

        private void UnregisterCallbacks(IInputGameActions instance)
        {
            @Sonar.started -= instance.OnSonar;
            @Sonar.performed -= instance.OnSonar;
            @Sonar.canceled -= instance.OnSonar;
        }

        public void RemoveCallbacks(IInputGameActions instance)
        {
            if (m_Wrapper.m_InputGameActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInputGameActions instance)
        {
            foreach (var item in m_Wrapper.m_InputGameActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InputGameActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InputGameActions @InputGame => new InputGameActions(this);
    public interface IInputGameActions
    {
        void OnSonar(InputAction.CallbackContext context);
    }
}
