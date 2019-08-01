using Framework;
using Framework.Components;
using UnityEngine;
using YourGame;

namespace YourGame
{
    /// <summary>
    /// Implement a controllable character here.
    /// </summary>
    class PlayerPawn : MonoBehaviour, IPawn
    {
        void IPawn.HandleInput(InputUpdateEvent inputUpdateEvent)
        {
            DebugEx.Log<PlayerPawn>("Receiving input...");
        }
    }
}
