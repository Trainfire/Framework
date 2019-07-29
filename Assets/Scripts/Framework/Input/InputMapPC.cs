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
        public override InputContext Context { get { return InputContext.PC; } }

        protected override bool Active => true; // TODO.

        protected override void RegisterInputs()
        {
            CreateTwinAxes(InputPCAxis.Mouse, new InputTwinAxes("Mouse", "Mouse X", "Mouse Y"));
        }

        protected override void DetectButtonState(KeyCode button, InputAction namedEvent)
        {
            var actionType = GetActionType(button);
            if (actionType != InputActionType.None)
                AddButtonEvent(namedEvent, actionType);
        }
    }
}
