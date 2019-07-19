using UnityEngine;
using Framework;
using System.Linq;

public class YourGame : Game, IInputUpdateHandler
{
    protected override void OnInitialize(params string[] args)
    {
        base.OnInitialize(args);

        DebugEx.Log<YourGame>("Let the game begin!");

        var inputMapXbox = gameObject.GetOrAddComponent<InputMapXbox>();
        inputMapXbox.BindAxisToAction(InputXboxAxis.LStick, "Move");
        InputManager.RegisterMap(inputMapXbox);

        var inputMapPS4 = gameObject.GetOrAddComponent<InputMapPS4>();
        inputMapPS4.BindAxisToAction(InputPS4Axis.LStick, "Move");
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

        inputUpdateEvent.GetTwinAxesEvent("Move", (args) =>
        {
            Debug.Log($"Received: { args.Axis } { args.Delta } from { inputUpdateEvent.Context }");
        });
    }
}
