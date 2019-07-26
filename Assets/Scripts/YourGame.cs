using UnityEngine;
using Framework;

namespace YourGame
{
    /// <summary>
    /// Define your input events here.
    /// </summary>
    public static class InputEventsRegister
    {
        public static InputEvent ExampleInputEvent { get; private set; } = new InputEvent("ExampleInputEvent");
    }

    public class YourGame : Game, IInputUpdateHandler
    {
        protected override void OnRegisterInputs(InputHelper inputHelper)
        {
            inputHelper.Xbox.BindButtonToInputEvent(InputXboxButton.ButtonA, InputEventsRegister.ExampleInputEvent);
            inputHelper.PS4.BindButtonToInputEvent(InputPS4Button.Cross, InputEventsRegister.ExampleInputEvent);
        }

        protected override void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);

            InputManager.RegisterHandler(this);

            DebugEx.Log<YourGame>("Let the game begin!");
        }

        public void HandleInputUpdate(InputMapUpdateEvent inputUpdateEvent)
        {
            inputUpdateEvent.GetButtonEvent(InputEventsRegister.ExampleInputEvent, (args) =>
            {
                Debug.Log($"Received: { args.ID.Name } { args.Type } from { inputUpdateEvent.Context }");
            });
        }
    }
}

