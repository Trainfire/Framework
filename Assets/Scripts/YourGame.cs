using UnityEngine;
using Framework;

public class YourGame : Game
{
    protected override void OnInitialize(params string[] args)
    {
        base.OnInitialize(args);

        DebugEx.Log<YourGame>("Let the game's begin!");

        // Determines where to go first.
        if (args != null && args.Length != 0)
        {
            Controller.LoadLevel(args[0]);
        }
        else
        {
            Controller.LoadFrontEnd();
        }
    }
}
