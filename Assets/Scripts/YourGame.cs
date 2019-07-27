using UnityEngine;
using Framework;

namespace YourGame
{
    /// <summary>
    /// Define your input events here.
    /// </summary>
    public static class InputActions
    {
        public static InputAction ExampleInputEvent { get; private set; } = new InputAction("ExampleInputEvent");
    }

    public class YourGame : Game, IInputHandler
    {
        protected override void OnRegisterInputs(InputHelper inputHelper)
        {
            inputHelper.PC.BindButtonToAction(KeyCode.Space, InputActions.ExampleInputEvent);
            inputHelper.Xbox.BindButtonToAction(InputXboxButton.ButtonA, InputActions.ExampleInputEvent);
            inputHelper.PS4.BindButtonToAction(InputPS4Button.Cross, InputActions.ExampleInputEvent);
        }

        protected override void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);

            InputManager.RegisterHandler(this);

            DebugEx.Log<YourGame>("Let the game begin!");
        }

        public void HandleUpdate(InputUpdateEvent inputUpdateEvent)
        {
            inputUpdateEvent.GetButtonEvent(InputActions.ExampleInputEvent, (args) =>
            {
                Debug.Log($"Received: { args.Action.Name } { args.Type } from { inputUpdateEvent.Context }");
            });
        }
    }
}

