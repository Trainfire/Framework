using UnityEngine;
using Framework;

public class YourGame : Game
{
    protected override void OnInitialize(params string[] args)
    {
        base.OnInitialize(args);

        DebugEx.Log<YourGame>("Let the game begin!");
    }
}
