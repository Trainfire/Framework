using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class InputMapUpdateEvent : EventArgs
    {
        public InputContext Context { get; private set; }

        Dictionary<string, InputButtonEvent> _buttonEvents = new Dictionary<string, InputButtonEvent>();
        Dictionary<string, InputSingleAxisEvent> _singleAxesEvents = new Dictionary<string, InputSingleAxisEvent>();
        Dictionary<string, InputTwinAxisEvent> _twinAxesEvents = new Dictionary<string, InputTwinAxisEvent>();

        public InputMapUpdateEvent(IInputMap inputMap)
        {
            Context = inputMap.Context;
            _buttonEvents = inputMap.ButtonEvents;
            _singleAxesEvents = inputMap.SingleAxisEvents;
            _twinAxesEvents = inputMap.TwinAxisEvents;
        }

        public void GetButtonEvent(string actionName, Action<InputButtonEvent> onGet) => Get(_buttonEvents, actionName, onGet);
        public void GetSingleAxisEvent(string actionName, Action<InputSingleAxisEvent> onGet) => Get(_singleAxesEvents, actionName, onGet);
        public void GetTwinAxesEvent(string actionName, Action<InputTwinAxisEvent> onGet) => Get(_twinAxesEvents, actionName, onGet);

        void Get<TAction>(IDictionary<string, TAction> dictionary, string actionName, Action<TAction> onGet)
        {
            if (dictionary.ContainsKey(actionName))
                onGet(dictionary[actionName]);
        }
    }

    public interface IInputMap
    {
        event EventHandler<InputMapUpdateEvent> OnUpdate;
        InputContext Context { get; }
        Dictionary<string, InputButtonEvent> ButtonEvents { get; }
        Dictionary<string, InputSingleAxisEvent> SingleAxisEvents { get; }
        Dictionary<string, InputTwinAxisEvent> TwinAxisEvents { get; }
    }

    public static class InputMapCoreBindings
    {
        #region Core Generic Bindings
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";
        public const string Up = "Up";
        public const string Right = "Right";
        public const string Down = "Down";
        public const string Left = "Left";
        public const string ScrollUp = "ScrollUp";
        public const string ScrollDown = "ScrollDown";
        public const string Back = "Back";
        public const string Start = "Pause";
        public const string LeftClick = "LeftClick";
        public const string RightClick = "RightClick";
        public const string MiddleClick = "MiddleClick";
        public const string LeftTrigger = "LeftTrigger";
        public const string RightTrigger = "RightTrigger";
        #endregion
    }

    // Maps bindings to an input
    public abstract class InputMap<TInputButton, TInputAxis> : MonoBehaviour, IInputMap
    {
        public event EventHandler<InputMapUpdateEvent> OnUpdate;

        public abstract InputContext Context { get; }
        protected abstract bool Active { get; }

        public Dictionary<string, InputButtonEvent> ButtonEvents { get; private set; } = new Dictionary<string, InputButtonEvent>();
        public Dictionary<string, InputSingleAxisEvent> SingleAxisEvents { get; private set; } = new Dictionary<string, InputSingleAxisEvent>();
        public Dictionary<string, InputTwinAxisEvent> TwinAxisEvents { get; private set; } = new Dictionary<string, InputTwinAxisEvent>();

        private Dictionary<TInputButton, string> _buttonToActionBindings = new Dictionary<TInputButton, string>();
        private Dictionary<TInputAxis, string> _axesToActionBindings = new Dictionary<TInputAxis, string>();

        private Dictionary<TInputButton, string> _buttonBindingsToUnityInputs = new Dictionary<TInputButton, string>();
        private Dictionary<TInputButton, InputUnityAxisToButtonConverter> _buttonBindingsToUnityAxes = new Dictionary<TInputButton, InputUnityAxisToButtonConverter>();
        private Dictionary<TInputAxis, InputSingleAxis> _singleAxes = new Dictionary<TInputAxis, InputSingleAxis>();
        private Dictionary<TInputAxis, InputTwinAxes> _twinAxes = new Dictionary<TInputAxis, InputTwinAxes>();

        public void BindButtonToAction(TInputButton button, string actionName)
        {
            if (_buttonToActionBindings.ContainsKey(button))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button.ToString(), actionName);
                return;
            }

            _buttonToActionBindings.Add(button, actionName);
        }

        /// <summary>
        /// Bind a TInputButton to a Unity Input Button.
        /// </summary>
        public void BindAxisToAction(TInputAxis axis, string actionName)
        {
            if (!_singleAxes.ContainsKey(axis) && !_twinAxes.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("Cannot bind axis '{0}' to {1}' since the axis has not been created. Did you use CreateSingleAxis() or CreateTwinAxis() in the InputMap?", axis.ToString(), actionName);
                return;
            }

            if (_axesToActionBindings.ContainsKey(axis))
            {
                DebugEx.LogError<InputMap<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", axis.ToString(), actionName);
                return;
            }

            _axesToActionBindings.Add(axis, actionName);
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

            foreach (var buttonToActionBinding in _buttonToActionBindings)
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

            foreach (var axisToActionBinding in _axesToActionBindings)
            {
                var axis = axisToActionBinding.Key;
                var action = axisToActionBinding.Value;

                if (_twinAxes.ContainsKey(axis))
                {
                    var twinAxes = _twinAxes[axis];
                    twinAxes.Update();

                    AddTwinAxisEvent(action, twinAxes.Delta);
                }

                if (_singleAxes.ContainsKey(axis))
                {
                    var singleAxis = _singleAxes[axis];
                    singleAxis.Update();

                    AddSingleAxisEvent(action, singleAxis.Delta);
                }
            }

            OnUpdate?.Invoke(this, new InputMapUpdateEvent(this));

            ButtonEvents.Clear();
            SingleAxisEvents.Clear();
            TwinAxisEvents.Clear();

            //foreach (var twinAxes in _twinAxesBindings)
            //{
            //    twinAxes.Value.Update();
            //}

            //foreach (var kvp in _buttonBindingsToActions)
            //{
            //    TInputButton actionBoundButton = kvp.Key;
            //    string actionBoundName = kvp.Value;

            //    if (!_buttonBindingsToUnityInputs.ContainsKey(actionBoundButton))
            //        continue;

            //    // The name of the input defined in Unity's Input Manager.
            //    string buttonBoundUnityInputName = _buttonBindingsToUnityInputs[actionBoundButton];

            //    if (Input.anyKey)
            //    {
            //        if (Input.GetKeyDown(buttonBoundUnityInputName))
            //            AddButtonEvent(actionBoundName, InputActionType.Down);

            //        if (Input.GetKey(buttonBoundUnityInputName))
            //            AddButtonEvent(actionBoundName, InputActionType.Held);
            //    }

            //    if (Input.GetKeyUp(buttonBoundUnityInputName))
            //        AddButtonEvent(actionBoundName, InputActionType.Up);
            //}

            //foreach (var kvp in _actionToAxesBindings)
            //{
            //    var inputType = kvp.Key;
            //    var inputAxisWrapper = kvp.Value;

            //    if (!_buttonBindingsToActions.ContainsKey(inputType))
            //        continue;

            //    inputAxisWrapper.Update();

            //    var actionName = _buttonBindingsToActions[inputType];

            //    AddButtonEvent(actionName, inputAxisWrapper.LastAction);
            //}

            //foreach (var kvp in _axesBindingsToActions)
            //{
            //    var axis = kvp.Key;
            //    var action = kvp.Value;

            //    if (_twinAxesBindings.ContainsKey(axis))
            //        AddTwinAxisEvent(_axesBindingsToActions[axis], _twinAxesBindings[axis].Delta);
            //}
        }

        protected void AddButtonEvent(string actionName, InputActionType actionType)
        {
            ButtonEvents.Add(actionName, new InputButtonEvent(actionName, actionType));
        }

        protected void AddSingleAxisEvent(string axisName, float delta)
        {
            SingleAxisEvents.Add(axisName, new InputSingleAxisEvent(axisName, delta));
        }

        protected void AddTwinAxisEvent(string axisName, Vector2 delta)
        {
            TwinAxisEvents.Add(axisName, new InputTwinAxisEvent(axisName, delta));
        }
    }
}
