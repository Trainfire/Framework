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

    public class Game : GameBase
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

            AddGameRule<GameRule>();

            DebugEx.Log<Game>("Let the game begin!");
        }
    }
}

