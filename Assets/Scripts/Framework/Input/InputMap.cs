using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class InputMapUpdateEvent : EventArgs
    {
        public InputContext Context { get; private set; }

        Dictionary<InputEventID, InputButtonEvent> _buttonEvents = new Dictionary<InputEventID, InputButtonEvent>();
        Dictionary<InputEventID, InputSingleAxisEvent> _singleAxesEvents = new Dictionary<InputEventID, InputSingleAxisEvent>();
        Dictionary<InputEventID, InputTwinAxisEvent> _twinAxesEvents = new Dictionary<InputEventID, InputTwinAxisEvent>();

        public InputMapUpdateEvent(IInputMap inputMap)
        {
            Context = inputMap.Context;
            _buttonEvents = inputMap.ButtonEvents;
            _singleAxesEvents = inputMap.SingleAxisEvents;
            _twinAxesEvents = inputMap.TwinAxisEvents;
        }

        public void GetButtonEvent(InputEventID actionName, Action<InputButtonEvent> onGet) => Get(_buttonEvents, actionName, onGet);
        public void GetSingleAxisEvent(InputEventID actionName, Action<InputSingleAxisEvent> onGet) => Get(_singleAxesEvents, actionName, onGet);
        public void GetTwinAxesEvent(InputEventID actionName, Action<InputTwinAxisEvent> onGet) => Get(_twinAxesEvents, actionName, onGet);

        void Get<TAction>(IDictionary<InputEventID, TAction> dictionary, InputEventID actionName, Action<TAction> onGet)
        {
            if (dictionary.ContainsKey(actionName))
                onGet(dictionary[actionName]);
        }
    }

    public interface IInputMap
    {
        event EventHandler<InputMapUpdateEvent> OnUpdate;
        InputContext Context { get; }
        Dictionary<InputEventID, InputButtonEvent> ButtonEvents { get; }
        Dictionary<InputEventID, InputSingleAxisEvent> SingleAxisEvents { get; }
        Dictionary<InputEventID, InputTwinAxisEvent> TwinAxisEvents { get; }
    }

    /// <summary>
    /// Wrapper for sanity's sake.
    /// </summary>
    public class InputEventID
    {
        public string Name { get; }
        public InputEventID(string name) => Name = name;
    }

    public static class InputMapCoreBindings
    {
        #region Core Generic Bindings
        public static InputEventID Horizontal = new InputEventID("Horizontal");
        public static InputEventID Vertical = new InputEventID("Vertical");
        public static InputEventID Up = new InputEventID("Up");
        public static InputEventID Right = new InputEventID("Right");
        public static InputEventID Down = new InputEventID("Down");
        public static InputEventID Left = new InputEventID("Left");
        public static InputEventID ScrollUp = new InputEventID("ScrollUp");
        public static InputEventID ScrollDown = new InputEventID("ScrollDown");
        public static InputEventID Back = new InputEventID("Back");
        public static InputEventID Start = new InputEventID("Pause");
        public static InputEventID LeftClick = new InputEventID("LeftClick");
        public static InputEventID RightClick = new InputEventID("RightClick");
        public static InputEventID MiddleClick = new InputEventID("MiddleClick");
        public static InputEventID LeftTrigger = new InputEventID("LeftTrigger");
        public static InputEventID RightTrigger = new InputEventID("RightTrigger");
        #endregion
    }

    // Maps bindings to an input
    public abstract class InputMap<TInputButton, TInputAxis> : MonoBehaviour, IInputMap
    {
        public event EventHandler<InputMapUpdateEvent> OnUpdate;

        public abstract InputContext Context { get; }
        protected abstract bool Active { get; }

        public Dictionary<InputEventID, InputButtonEvent> ButtonEvents { get; private set; } = new Dictionary<InputEventID, InputButtonEvent>();
        public Dictionary<InputEventID, InputSingleAxisEvent> SingleAxisEvents { get; private set; } = new Dictionary<InputEventID, InputSingleAxisEvent>();
        public Dictionary<InputEventID, InputTwinAxisEvent> TwinAxisEvents { get; private set; } = new Dictionary<InputEventID, InputTwinAxisEvent>();

        private Dictionary<TInputButton, InputEventID> _buttonToEventBindings = new Dictionary<TInputButton, InputEventID>();
        private Dictionary<TInputAxis, InputEventID> _axesToEventBindings = new Dictionary<TInputAxis, InputEventID>();

        private Dictionary<TInputButton, string> _buttonBindingsToUnityInputs = new Dictionary<TInputButton, string>();
        private Dictionary<TInputButton, InputUnityAxisToButtonConverter> _buttonBindingsToUnityAxes = new Dictionary<TInputButton, InputUnityAxisToButtonConverter>();
        private Dictionary<TInputAxis, InputSingleAxis> _singleAxes = new Dictionary<TInputAxis, InputSingleAxis>();
        private Dictionary<TInputAxis, InputTwinAxes> _twinAxes = new Dictionary<TInputAxis, InputTwinAxes>();

        public void BindButtonToInputEvent(TInputButton button, InputEventID inputEventID)
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
        public void BindAxisToInputEvent(TInputAxis axis, InputEventID inputEventID)
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
                            AddButtonEvent(action, InputActionType.Down);

                        if (Input.GetKey(buttonToUnityInputName))
                            AddButtonEvent(action, InputActionType.Held);
                    }

                    if (Input.GetKeyUp(buttonToUnityInputName))
                        AddButtonEvent(action, InputActionType.Up);
                }

                if (_buttonBindingsToUnityAxes.ContainsKey(button))
                {
                    var unityAxisToButtonConverter = _buttonBindingsToUnityAxes[button];
                    unityAxisToButtonConverter.Update();

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

            OnUpdate?.Invoke(this, new InputMapUpdateEvent(this));

            ButtonEvents.Clear();
            SingleAxisEvents.Clear();
            TwinAxisEvents.Clear();
        }

        void AddButtonEvent(InputEventID actionName, InputActionType actionType) => ButtonEvents.Add(actionName, new InputButtonEvent(actionName, actionType));
        void AddSingleAxisEvent(InputEventID axisName, float delta) => SingleAxisEvents.Add(axisName, new InputSingleAxisEvent(axisName, delta));
        void AddTwinAxisEvent(InputEventID axisName, Vector2 delta) => TwinAxisEvents.Add(axisName, new InputTwinAxisEvent(axisName, delta));
    }
}
