using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public static class InputMapCoreActions
    {
        public static InputAction Horizontal { get; private set; } = new InputAction("Horizontal");
        public static InputAction Vertical { get; private set; } = new InputAction("Vertical");
        public static InputAction Up { get; private set; } = new InputAction("Up");
        public static InputAction Right { get; private set; } = new InputAction("Right");
        public static InputAction Down { get; private set; } = new InputAction("Down");
        public static InputAction Left { get; private set; } = new InputAction("Left");
        public static InputAction ScrollUp { get; private set; } = new InputAction("ScrollUp");
        public static InputAction ScrollDown { get; private set; } = new InputAction("ScrollDown");
        public static InputAction Back { get; private set; } = new InputAction("Back");
        public static InputAction Start { get; private set; } = new InputAction("Pause");
        public static InputAction LeftClick { get; private set; } = new InputAction("LeftClick");
        public static InputAction RightClick { get; private set; } = new InputAction("RightClick");
        public static InputAction MiddleClick { get; private set; } = new InputAction("MiddleClick");
        public static InputAction LeftTrigger { get; private set; } = new InputAction("LeftTrigger");
        public static InputAction RightTrigger { get; private set; } = new InputAction("RightTrigger");
    }

    public class InputUpdateEvent : EventArgs
    {
        public InputContext Context { get { return _inputMap.Context; } }

        private IInputMap _inputMap;

        public InputUpdateEvent(IInputMap inputMap)
        {
            _inputMap = inputMap;
        }

        public IReadOnlyDictionary<InputAction, InputButtonEvent> GetAllButtonEvents() { return _inputMap.ButtonEvents; }
        public void GetButtonEvent(InputAction action, Action<InputButtonEvent> onGet) => Get(_inputMap.ButtonEvents, action, onGet);
        public void GetSingleAxisEvent(InputAction action, Action<InputSingleAxisEvent> onGet) => Get(_inputMap.SingleAxisEvents, action, onGet);
        public void GetTwinAxesEvent(InputAction action, Action<InputTwinAxisEvent> onGet) => Get(_inputMap.TwinAxisEvents, action, onGet);

        void Get<TAction>(IReadOnlyDictionary<InputAction, TAction> dictionary, InputAction actionName, Action<TAction> onGet)
        {
            if (dictionary.ContainsKey(actionName))
                onGet(dictionary[actionName]);
        }
    }

    public class ButtonEventDictionary : Dictionary<InputAction, InputButtonEvent> { }
    public class SingleAxisEventDictionary : Dictionary<InputAction, InputSingleAxisEvent> { }
    public class TwinAxisEventDictionary : Dictionary<InputAction, InputTwinAxisEvent> { }

    public interface IInputMap
    {
        event EventHandler<InputUpdateEvent> OnUpdate;

        InputContext Context { get; }
        ButtonEventDictionary ButtonEvents { get; }
        SingleAxisEventDictionary SingleAxisEvents { get; }
        TwinAxisEventDictionary TwinAxisEvents { get; }
    }

    public abstract class InputMap<TInputButton, TInputAxis> : MonoBehaviour, IInputMap
    {
        public event EventHandler<InputUpdateEvent> OnUpdate;

        public abstract InputContext Context { get; }

        public ButtonEventDictionary ButtonEvents { get { return _buttonEvents; } }
        public SingleAxisEventDictionary SingleAxisEvents { get { return _singleAxisEvents; } }
        public TwinAxisEventDictionary TwinAxisEvents { get { return _twinAxisEvents; } }

        protected abstract bool Active { get; }

        private ButtonEventDictionary _buttonEvents { get; set; } = new ButtonEventDictionary();
        private SingleAxisEventDictionary _singleAxisEvents { get; set; } = new SingleAxisEventDictionary();
        private TwinAxisEventDictionary _twinAxisEvents { get; set; } = new TwinAxisEventDictionary();

        private Dictionary<TInputButton, InputAction> _buttonToEventBindings = new Dictionary<TInputButton, InputAction>();
        private Dictionary<TInputAxis, InputAction> _axesToEventBindings = new Dictionary<TInputAxis, InputAction>();

        private Dictionary<TInputButton, string> _buttonBindingsToUnityInputs = new Dictionary<TInputButton, string>();
        private Dictionary<TInputButton, InputUnityAxisToButtonConverter> _buttonBindingsToUnityAxes = new Dictionary<TInputButton, InputUnityAxisToButtonConverter>();
        private Dictionary<TInputAxis, InputSingleAxis> _singleAxes = new Dictionary<TInputAxis, InputSingleAxis>();
        private Dictionary<TInputAxis, InputTwinAxes> _twinAxes = new Dictionary<TInputAxis, InputTwinAxes>();

        private void Awake()
        {
            RegisterInputs();
        }

        protected abstract void RegisterInputs();

        public void BindButtonToAction(TInputButton button, InputAction action)
        {
            if (_buttonToEventBindings.ContainsKey(button))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button.ToString(), action);
                return;
            }

            _buttonToEventBindings.Add(button, action);
        }

        /// <summary>
        /// Bind a TInputButton to a Unity Input Button.
        /// </summary>
        public void BindAxisToAction(TInputAxis axis, InputAction action)
        {
            if (!_singleAxes.ContainsKey(axis) && !_twinAxes.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("Cannot bind axis '{0}' to {1}' since the axis has not been created. Did you use CreateSingleAxis() or CreateTwinAxis() in the InputMap?", axis.ToString(), action);
                return;
            }

            if (_axesToEventBindings.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", axis.ToString(), action);
                return;
            }

            _axesToEventBindings.Add(axis, action);
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

                DetectButtonState(button, action);

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

            OnUpdate?.Invoke(this, new InputUpdateEvent(this));

            _buttonEvents.Clear();
            _singleAxisEvents.Clear();
            _twinAxisEvents.Clear();
        }

        void CheckIfKeyIsActive(string keyName, InputAction namedEvent)
        {
            if (Input.anyKey)
            {
                if (Input.GetKeyDown(keyName))
                {
                    AddButtonEvent(namedEvent, InputActionType.Down);
                }
                else if (Input.GetKey(keyName))
                {
                    AddButtonEvent(namedEvent, InputActionType.Held);
                }
            }
            else if (Input.GetKeyUp(keyName))
            {
                AddButtonEvent(namedEvent, InputActionType.Up);
            }
        }

        protected virtual void DetectButtonState(TInputButton button, InputAction action)
        {
            if (_buttonBindingsToUnityInputs.ContainsKey(button))
            {
                var buttonToUnityInputName = _buttonBindingsToUnityInputs[button];

                var actionType = GetActionType(buttonToUnityInputName);
                if (actionType != InputActionType.None)
                    AddButtonEvent(action, actionType);
            }
        }

        protected InputActionType GetActionType(string buttonName)
        {
            if (Input.anyKey)
            {
                if (Input.GetKeyDown(buttonName))
                    return InputActionType.Down;

                if (Input.GetKey(buttonName))
                    return InputActionType.Held;
            }
            else if (Input.GetKeyUp(buttonName))
            {
                return InputActionType.Up;
            }

            return InputActionType.None;
        }

        protected InputActionType GetActionType(KeyCode keyCode)
        {
            if (Input.anyKey)
            {
                if (Input.GetKeyDown(keyCode))
                    return InputActionType.Down;

                if (Input.GetKey(keyCode))
                    return InputActionType.Held;
            }
            else if (Input.GetKeyUp(keyCode))
            {
                return InputActionType.Up;
            }

            return InputActionType.None;
        }

        protected void AddButtonEvent(InputAction action, InputActionType type) => _buttonEvents.Add(action, new InputButtonEvent(action, type));
        void AddSingleAxisEvent(InputAction action, float delta) => _singleAxisEvents.Add(action, new InputSingleAxisEvent(action, delta));
        void AddTwinAxisEvent(InputAction action, Vector2 delta) => _twinAxisEvents.Add(action, new InputTwinAxisEvent(action, delta));
    }
}
