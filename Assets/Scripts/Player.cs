using Framework;

namespace YourGame
{
    public static class PlayerHelper
    {
        public static Player ToPlayer(this BasePlayer basePlayer)
        {
            return basePlayer as Player;
        }
    }

    public class Player : BasePlayer
    {
        protected override void OnPossess(IPawn pawn)
        {
            DebugEx.Log<Player>("Possessed a thing.");
        }
    }
}
