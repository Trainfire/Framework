using UnityEngine;
using System.Linq;

namespace Framework
{
    public enum InputPS4Button
    {
        Cross,
        Circle,
        Triangle,
        Square,
        L1,
        L2,
        L3,
        R1,
        R2,
        R3,
        Back,
        Options,
        DPadUp,
        DPadRight,
        DPadLeft,
        DPadDown,
    }

    public enum InputPS4Axis
    {
        LStick,
        RStick,
        L2,
        R2,
    }

    public class InputMapPS4 : InputMap<InputPS4Button, InputPS4Axis>
    {
        public override InputContext Context { get { return InputContext.PS4; } }

        protected override bool Active => Input.GetJoystickNames().Any(x => x.Contains("Wireless Controller"));

        private void Awake()
        {
            CreateTwinAxes(InputPS4Axis.LStick, new InputTwinAxes("Left Stick", "Horizontal", "Vertical"));

            BindButtonToUnityInputButton(InputPS4Button.Square, "joystick button 0");
            BindButtonToUnityInputButton(InputPS4Button.Cross, "joystick button 1");
            BindButtonToUnityInputButton(InputPS4Button.Circle, "joystick button 2");
            BindButtonToUnityInputButton(InputPS4Button.Triangle, "joystick button 3");
            BindButtonToUnityInputButton(InputPS4Button.L1, "joystick button 4");
            BindButtonToUnityInputButton(InputPS4Button.R1, "joystick button 5");
            BindButtonToUnityInputButton(InputPS4Button.L2, "joystick button 6");
            BindButtonToUnityInputButton(InputPS4Button.R2, "joystick button 7");
            // BindButtonToUnityInput(InputPS4.Share, "joystick button 8"); (Not supported yet).
            BindButtonToUnityInputButton(InputPS4Button.Options, "joystick button 9");
            BindButtonToUnityInputButton(InputPS4Button.L3, "joystick button 10");
            BindButtonToUnityInputButton(InputPS4Button.R3, "joystick button 11");
            //BindButtonToUnityInput(InputPS4.PS, "joystick button 12"); (Not supported yet).
            //BindButtonToUnityInput(InputPS4.Pad, "joystick button 13"); (Not supported yet).

            BindButtonToUnityInputAxis(InputPS4Button.DPadLeft, new InputUnityAxisToButtonConverter("PS4 D Pad X", true));
            BindButtonToUnityInputAxis(InputPS4Button.DPadRight, new InputUnityAxisToButtonConverter("PS4 D Pad X"));
            BindButtonToUnityInputAxis(InputPS4Button.DPadUp, new InputUnityAxisToButtonConverter("PS4 D Pad Y"));
            BindButtonToUnityInputAxis(InputPS4Button.DPadDown, new InputUnityAxisToButtonConverter("PS4 D Pad Y", true));

            BindButtonToInputEvent(InputPS4Button.DPadUp, InputMapCoreEventsRegister.Up);
            BindButtonToInputEvent(InputPS4Button.DPadDown, InputMapCoreEventsRegister.Down);
            BindButtonToInputEvent(InputPS4Button.DPadLeft, InputMapCoreEventsRegister.Left);
            BindButtonToInputEvent(InputPS4Button.DPadRight, InputMapCoreEventsRegister.Right);
        }
    }
}
