using Framework;

namespace YourGame
{
    public static class PlayerHelper
    {
        public static Player ToPlayer(this PlayerBase basePlayer)
        {
            return basePlayer as Player;
        }
    }

    public class Player : PlayerBase
    {
        protected override void OnPossess(IPawn pawn)
        {
            DebugEx.Log<Player>("Possessed a thing.");
        }
    }
}
