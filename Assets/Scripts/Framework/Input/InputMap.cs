using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public static class InputMapCoreEventsRegister
    {
        public static InputNamedEvent Horizontal { get; private set; } = new InputNamedEvent("Horizontal");
        public static InputNamedEvent Vertical { get; private set; } = new InputNamedEvent("Vertical");
        public static InputNamedEvent Up { get; private set; } = new InputNamedEvent("Up");
        public static InputNamedEvent Right { get; private set; } = new InputNamedEvent("Right");
        public static InputNamedEvent Down { get; private set; } = new InputNamedEvent("Down");
        public static InputNamedEvent Left { get; private set; } = new InputNamedEvent("Left");
        public static InputNamedEvent ScrollUp { get; private set; } = new InputNamedEvent("ScrollUp");
        public static InputNamedEvent ScrollDown { get; private set; } = new InputNamedEvent("ScrollDown");
        public static InputNamedEvent Back { get; private set; } = new InputNamedEvent("Back");
        public static InputNamedEvent Start { get; private set; } = new InputNamedEvent("Pause");
        public static InputNamedEvent LeftClick { get; private set; } = new InputNamedEvent("LeftClick");
        public static InputNamedEvent RightClick { get; private set; } = new InputNamedEvent("RightClick");
        public static InputNamedEvent MiddleClick { get; private set; } = new InputNamedEvent("MiddleClick");
        public static InputNamedEvent LeftTrigger { get; private set; } = new InputNamedEvent("LeftTrigger");
        public static InputNamedEvent RightTrigger { get; private set; } = new InputNamedEvent("RightTrigger");
    }

    public class InputHandlerEvent : EventArgs
    {
        public InputContext Context { get { return _inputMap.Context; } }

        private IInputMap _inputMap;

        public InputHandlerEvent(IInputMap inputMap)
        {
            _inputMap = inputMap;
        }

        public IReadOnlyDictionary<InputNamedEvent, InputButtonEvent> GetAllButtonEvents() { return _inputMap.ButtonEvents; }
        public void GetButtonEvent(InputNamedEvent actionName, Action<InputButtonEvent> onGet) => Get(_inputMap.ButtonEvents, actionName, onGet);
        public void GetSingleAxisEvent(InputNamedEvent actionName, Action<InputSingleAxisEvent> onGet) => Get(_inputMap.SingleAxisEvents, actionName, onGet);
        public void GetTwinAxesEvent(InputNamedEvent actionName, Action<InputTwinAxisEvent> onGet) => Get(_inputMap.TwinAxisEvents, actionName, onGet);

        void Get<TAction>(IReadOnlyDictionary<InputNamedEvent, TAction> dictionary, InputNamedEvent actionName, Action<TAction> onGet)
        {
            if (dictionary.ContainsKey(actionName))
                onGet(dictionary[actionName]);
        }
    }

    /// <summary>
    /// Sanity wrappers.
    /// </summary>
    public class ButtonEventDictionary : Dictionary<InputNamedEvent, InputButtonEvent> { }
    public class SingleAxisEventDictionary : Dictionary<InputNamedEvent, InputSingleAxisEvent> { }
    public class TwinAxisEventDictionary : Dictionary<InputNamedEvent, InputTwinAxisEvent> { }

    public interface IInputMap
    {
        event EventHandler<InputHandlerEvent> OnUpdate;
        InputContext Context { get; }

        ButtonEventDictionary ButtonEvents { get; }
        SingleAxisEventDictionary SingleAxisEvents { get; }
        TwinAxisEventDictionary TwinAxisEvents { get; }
    }

    // Maps bindings to an input
    public abstract class InputMap<TInputButton, TInputAxis> : MonoBehaviour, IInputMap
    {
        public event EventHandler<InputHandlerEvent> OnUpdate;

        public abstract InputContext Context { get; }

        public ButtonEventDictionary ButtonEvents { get { return _buttonEvents; } }
        public SingleAxisEventDictionary SingleAxisEvents { get { return _singleAxisEvents; } }
        public TwinAxisEventDictionary TwinAxisEvents { get { return _twinAxisEvents; } }

        protected abstract bool Active { get; }

        private ButtonEventDictionary _buttonEvents { get; set; } = new ButtonEventDictionary();
        private SingleAxisEventDictionary _singleAxisEvents { get; set; } = new SingleAxisEventDictionary();
        private TwinAxisEventDictionary _twinAxisEvents { get; set; } = new TwinAxisEventDictionary();

        private Dictionary<TInputButton, InputNamedEvent> _buttonToEventBindings = new Dictionary<TInputButton, InputNamedEvent>();
        private Dictionary<TInputAxis, InputNamedEvent> _axesToEventBindings = new Dictionary<TInputAxis, InputNamedEvent>();

        private Dictionary<TInputButton, string> _buttonBindingsToUnityInputs = new Dictionary<TInputButton, string>();
        private Dictionary<TInputButton, InputUnityAxisToButtonConverter> _buttonBindingsToUnityAxes = new Dictionary<TInputButton, InputUnityAxisToButtonConverter>();
        private Dictionary<TInputAxis, InputSingleAxis> _singleAxes = new Dictionary<TInputAxis, InputSingleAxis>();
        private Dictionary<TInputAxis, InputTwinAxes> _twinAxes = new Dictionary<TInputAxis, InputTwinAxes>();

        public void BindButtonToInputEvent(TInputButton button, InputNamedEvent inputEventID)
        {
            if (_buttonToEventBindings.ContainsKey(button))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button.ToString(), inputEventID);
                return;
            }

            _buttonToEventBindings.Add(button, inputEventID);
        }

        /// <summary>
        /// Bind a TInputButton to a Unity Input Button.
        /// </summary>
        public void BindAxisToInputEvent(TInputAxis axis, InputNamedEvent inputEventID)
        {
            if (!_singleAxes.ContainsKey(axis) && !_twinAxes.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("Cannot bind axis '{0}' to {1}' since the axis has not been created. Did you use CreateSingleAxis() or CreateTwinAxis() in the InputMap?", axis.ToString(), inputEventID);
                return;
            }

            if (_axesToEventBindings.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", axis.ToString(), inputEventID);
                return;
            }

            _axesToEventBindings.Add(axis, inputEventID);
        }

        /// <summary>
        /// Bind a TInputButton to a Unity Input Axis. For example: The D Pad X axis to left and right button presses.
        /// </summary>
        protected void BindButtonToUnityInputAxis(TInputButton button, InputUnityAxisToButtonConverter unityAxisToButtonConverter)
        {
            if (_buttonBindingsToUnityAxes.ContainsKey(button))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button.ToString(), unityAxisToButtonConverter.UnityAxis);
                return;
            }

            _buttonBindingsToUnityAxes.Add(button, unityAxisToButtonConverter);
        }

        /// <summary>
        /// Bind a TInputButton to a Unity Input Button.
        /// </summary>
        protected void BindButtonToUnityInputButton(TInputButton button, string unityInputName)
        {
            if (_buttonBindingsToUnityInputs.ContainsKey(button))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button.ToString(), unityInputName);
                return;
            }

            _buttonBindingsToUnityInputs.Add(button, unityInputName);
        }

        protected void CreateSingleAxis(TInputAxis axis, InputSingleAxis singleAxis)
        {
            if (_singleAxes.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", axis.ToString(), _singleAxes[axis].Name);
                return;
            }

            _singleAxes.Add(axis, singleAxis);
        }

        protected void CreateTwinAxes(TInputAxis axis, InputTwinAxes twinAxes)
        {
            if (_twinAxes.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", axis.ToString(), _twinAxes[axis].Name);
                return;
            }

            _twinAxes.Add(axis, twinAxes);
        }

        private void LateUpdate()
        {
            if (!Active)
                return;

            foreach (var buttonToActionBinding in _buttonToEventBindings)
            {
                var button = buttonToActionBinding.Key;
                var action = buttonToActionBinding.Value;

                if (_buttonBindingsToUnityInputs.ContainsKey(button))
                {
                    var buttonToUnityInputName = _buttonBindingsToUnityInputs[button];

                    if (Input.anyKey)
                    {
                        if (Input.GetKeyDown(buttonToUnityInputName))
                        {
                            AddButtonEvent(action, InputActionType.Down);
                        }
                        else if (Input.GetKey(buttonToUnityInputName))
                        {
                            AddButtonEvent(action, InputActionType.Held);
                        }    
                    }
                    else if (Input.GetKeyUp(buttonToUnityInputName))
                    {
                        AddButtonEvent(action, InputActionType.Up);
                    }
                }

                if (_buttonBindingsToUnityAxes.ContainsKey(button))
                {
                    var unityAxisToButtonConverter = _buttonBindingsToUnityAxes[button];
                    unityAxisToButtonConverter.Update();

                    if (unityAxisToButtonConverter.DidUpdate)
                        AddButtonEvent(action, unityAxisToButtonConverter.LastAction);
                }
            }

            foreach (var axisToActionBinding in _axesToEventBindings)
            {
                var axis = axisToActionBinding.Key;
                var action = axisToActionBinding.Value;

                if (_twinAxes.ContainsKey(axis))
                {
                    var twinAxes = _twinAxes[axis];
                    AddTwinAxisEvent(action, twinAxes.Delta);
                }

                if (_singleAxes.ContainsKey(axis))
                {
                    var singleAxis = _singleAxes[axis];
                    AddSingleAxisEvent(action, singleAxis.Delta);
                }
            }

            OnUpdate?.Invoke(this, new InputHandlerEvent(this));

            _buttonEvents.Clear();
            _singleAxisEvents.Clear();
            _twinAxisEvents.Clear();
        }

        void AddButtonEvent(InputNamedEvent actionName, InputActionType actionType) => _buttonEvents.Add(actionName, new InputButtonEvent(actionName, actionType));
        void AddSingleAxisEvent(InputNamedEvent axisName, float delta) => _singleAxisEvents.Add(axisName, new InputSingleAxisEvent(axisName, delta));
        void AddTwinAxisEvent(InputNamedEvent axisName, Vector2 delta) => _twinAxisEvents.Add(axisName, new InputTwinAxisEvent(axisName, delta));
    }
}
