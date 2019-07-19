using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public abstract class InputMapController<TInputButton, TInputAxis> : InputMap
    {
        protected abstract bool Active { get; }

        private Dictionary<TInputButton, string> _buttonBindingsToActions = new Dictionary<TInputButton, string>();
        private Dictionary<TInputAxis, string> _axesBindingsToActions = new Dictionary<TInputAxis, string>();

        private Dictionary<TInputButton, string> _buttonBindingsToUnityInputs = new Dictionary<TInputButton, string>();
        private Dictionary<TInputButton, InputAxisToActionConverter> _actionToAxesBindings = new Dictionary<TInputButton, InputAxisToActionConverter>();
        private Dictionary<TInputAxis, InputTwinAxes> _twinAxesBindings = new Dictionary<TInputAxis, InputTwinAxes>();

        public void BindAction(TInputButton button, string actionName)
        {
            if (_buttonBindingsToActions.ContainsKey(button))
            {
                DebugEx.LogError<InputMapController<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", actionName, button);
                return;
            }

            _buttonBindingsToActions.Add(button, actionName);
        }

        public void BindTwinAxes(TInputAxis axis, string actionName)
        {
            if (_axesBindingsToActions.ContainsKey(axis))
            {
                DebugEx.LogError<InputMapController<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", actionName, axis);
                return;
            }

            _axesBindingsToActions.Add(axis, actionName);
        }

        protected void BindButtonToUnityInput(TInputButton button, string unityInputName)
        {
            // NB: Runtime will crash here if the unityInputName provided doesn't match an input in Unity's Input Manager.
            // (There is no way of checking if it is through code...)

            if (_buttonBindingsToUnityInputs.ContainsKey(button))
            {
                DebugEx.LogError<InputMapController<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button, unityInputName);
                return;
            }

            _buttonBindingsToUnityInputs.Add(button, unityInputName);
        }

        protected void BindButtonToUnityInput(TInputButton button, InputAxisToActionConverter axisWrapper)
        {
            if (_actionToAxesBindings.ContainsKey(button))
            {
                DebugEx.LogError<InputMapController<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", button, axisWrapper.Axis);
                return;
            }

            _actionToAxesBindings.Add(button, axisWrapper);
        }

        protected void BindTwinAxes(TInputAxis axis, InputTwinAxes twinAxes)
        {
            if (_twinAxesBindings.ContainsKey(axis))
            {
                DebugEx.LogError<InputMapController<TInputButton, TInputAxis>>("'{0}' is already bound to '{1}'", axis, twinAxes.Name);
                return;
            }

            _twinAxesBindings.Add(axis, twinAxes);
        }

        protected override void OnLateUpdate()
        {
            if (!Active)
                return;

            foreach (var twinAxes in _twinAxesBindings)
            {
                twinAxes.Value.Update();
            }

            foreach (var kvp in _buttonBindingsToActions)
            {
                TInputButton actionBoundButton = kvp.Key;
                string actionBoundName = kvp.Value;

                if (!_buttonBindingsToUnityInputs.ContainsKey(actionBoundButton))
                    continue;

                // The name of the input defined in Unity's Input Manager.
                string buttonBoundUnityInputName = _buttonBindingsToUnityInputs[actionBoundButton];

                if (Input.anyKey)
                {
                    if (Input.GetKeyDown(buttonBoundUnityInputName))
                        AddButtonActionEvent(actionBoundName, InputActionType.Down);

                    if (Input.GetKey(buttonBoundUnityInputName))
                        AddButtonActionEvent(actionBoundName, InputActionType.Held);
                }

                if (Input.GetKeyUp(buttonBoundUnityInputName))
                    AddButtonActionEvent(actionBoundName, InputActionType.Up);
            }

            foreach (var kvp in _actionToAxesBindings)
            {
                var inputType = kvp.Key;
                var inputAxisWrapper = kvp.Value;

                if (!_buttonBindingsToActions.ContainsKey(inputType))
                    continue;

                inputAxisWrapper.Update();

                var actionName = _buttonBindingsToActions[inputType];

                AddButtonActionEvent(actionName, inputAxisWrapper.LastAction);
            }

            foreach (var kvp in _axesBindingsToActions)
            {
                var axis = kvp.Key;
                var action = kvp.Value;

                if (_twinAxesBindings.ContainsKey(axis))
                    AddTwinAxisEvent(_axesBindingsToActions[axis], _twinAxesBindings[axis].Delta);
            }
        }
    }
}
