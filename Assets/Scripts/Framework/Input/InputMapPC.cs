using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    public enum InputPCAxis
    {
        Mouse,
    }

    public class InputMapPC : InputMap<KeyCode, InputPCAxis>
    {
        private Dictionary<string, KeyCode> _bindings;

        protected override bool Active => true; // TODO.
        public override InputContext Context { get { return InputContext.PC; } }

        public void Awake()
        {
            _bindings = new Dictionary<string, KeyCode>();
        }

        public void Bind(string action, KeyCode key)
        {
            if (_bindings.ContainsKey(action))
            {
                DebugEx.LogError<InputMapPC>("'{0}' is already bound to '{1}'", action, key);
            }
            else
            {
                _bindings.Add(action, key);
            }
        }

        //protected override void OnLateUpdate()
        //{
        //    foreach (var kvp in _bindings)
        //    {
        //        if (Input.anyKey)
        //        {
        //            if (Input.GetKeyDown(kvp.Value))
        //                AddButtonActionEvent(kvp.Key, InputActionType.Down);

        //            if (Input.GetKey(kvp.Value))
        //                AddButtonActionEvent(kvp.Key, InputActionType.Held);
        //        }

        //        if (Input.GetKeyUp(kvp.Value))
        //            AddButtonActionEvent(kvp.Key, InputActionType.Up);
        //    }

        //    if (Input.GetAxis("Mouse ScrollWheel") > 0f) AddButtonActionEvent(ScrollUp, InputActionType.Down);
        //    if (Input.GetAxis("Mouse ScrollWheel") < 0f) AddButtonActionEvent(ScrollDown, InputActionType.Down);
        //    if (Input.GetMouseButtonDown(0)) AddButtonActionEvent(LeftClick, InputActionType.Down);
        //    if (Input.GetMouseButtonDown(1)) AddButtonActionEvent(RightClick, InputActionType.Down);
        //}
    }
}
