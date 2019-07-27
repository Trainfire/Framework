using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Framework
{
    public enum InputXboxButton
    {
        ButtonA,
        ButtonB,
        ButtonX,
        ButtonY,
        LBumper,
        RBumper,
        Back,
        Start,
        LStickClick,
        RStickClick,
        LTrigger,
        RTrigger,
        DPadUp,
        DPadRight,
        DPadLeft,
        DPadDown,
    }

    public enum InputXboxAxis
    {
        LStick,
        RStick,
        LTrigger,
        RTrigger,
    }

    public class InputMapXbox : InputMap<InputXboxButton, InputXboxAxis>
    {
        public override InputContext Context { get { return InputContext.Xbox; } }

        protected override bool Active => Input.GetJoystickNames().Any(x => x.Contains("XBOX"));

        public void Awake()
        {
            CreateTwinAxes(InputXboxAxis.LStick, new InputTwinAxes("LStick", "Horizontal", "Vertical"));

            BindButtonToUnityInputAxis(InputXboxButton.LTrigger, new InputUnityAxisToButtonConverter("Xbox Left Trigger"));
            BindButtonToUnityInputAxis(InputXboxButton.RTrigger, new InputUnityAxisToButtonConverter("Xbox Right Trigger"));
            BindButtonToUnityInputAxis(InputXboxButton.DPadLeft, new InputUnityAxisToButtonConverter("Xbox D Pad X", true));
            BindButtonToUnityInputAxis(InputXboxButton.DPadRight, new InputUnityAxisToButtonConverter("Xbox D Pad X"));
            BindButtonToUnityInputAxis(InputXboxButton.DPadUp, new InputUnityAxisToButtonConverter("Xbox D Pad Y"));
            BindButtonToUnityInputAxis(InputXboxButton.DPadDown, new InputUnityAxisToButtonConverter("Xbox D Pad Y", true));

            // Default bindings for face buttons.
            // Note: These are Windows-specific.
            BindButtonToUnityInputButton(InputXboxButton.ButtonA, "joystick button 0");
            BindButtonToUnityInputButton(InputXboxButton.ButtonB, "joystick button 1");
            BindButtonToUnityInputButton(InputXboxButton.ButtonX, "joystick button 2");
            BindButtonToUnityInputButton(InputXboxButton.ButtonY, "joystick button 3");
            BindButtonToUnityInputButton(InputXboxButton.LBumper, "joystick button 4");
            BindButtonToUnityInputButton(InputXboxButton.RBumper, "joystick button 5");
            BindButtonToUnityInputButton(InputXboxButton.Back, "joystick button 6");
            BindButtonToUnityInputButton(InputXboxButton.Start, "joystick button 7");
            BindButtonToUnityInputButton(InputXboxButton.LStickClick, "joystick button 8");
            BindButtonToUnityInputButton(InputXboxButton.RStickClick, "joystick button 9");

            // Bind to the generic actions defined in InputMap
            BindButtonToAction(InputXboxButton.DPadUp, InputMapCoreActions.Up);
            BindButtonToAction(InputXboxButton.DPadDown, InputMapCoreActions.Down);
            BindButtonToAction(InputXboxButton.DPadLeft, InputMapCoreActions.Left);
            BindButtonToAction(InputXboxButton.DPadRight, InputMapCoreActions.Right);
        }
    }
}
