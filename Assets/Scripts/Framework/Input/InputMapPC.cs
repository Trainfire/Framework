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
    }
}
