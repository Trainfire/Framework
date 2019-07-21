using UnityEngine;
using Framework;
using System.Linq;

public class YourGame : Game, IInputUpdateHandler
{
    public static InputEventID actionMove = new InputEventID("Move");

    protected override void OnInitialize(params string[] args)
    {
        base.OnInitialize(args);

        DebugEx.Log<YourGame>("Let the game begin!");

        var inputMapXbox = gameObject.GetOrAddComponent<InputMapXbox>();
        inputMapXbox.BindAxisToInputEvent(InputXboxAxis.LStick, actionMove);
        InputManager.RegisterMap(inputMapXbox);

        var inputMapPS4 = gameObject.GetOrAddComponent<InputMapPS4>();
        inputMapPS4.BindAxisToInputEvent(InputPS4Axis.LStick, actionMove);
        InputManager.RegisterMap(inputMapPS4);

        InputManager.RegisterHandler(this);
    }

    public void HandleInputUpdate(InputMapUpdateEvent inputUpdateEvent)
    {
        //inputUpdateEvent.GetButtonEvent("Up", (args) =>
        //{
        //    if (args.Type != InputActionType.None)
        //        Debug.Log($"Received: { args.Action } { args.Type }");
        //});

        inputUpdateEvent.GetTwinAxesEvent(actionMove, (args) =>
        {
            Debug.Log($"Received: { args.ID.Name } { args.Delta } from { inputUpdateEvent.Context }");
        });
    }
}
