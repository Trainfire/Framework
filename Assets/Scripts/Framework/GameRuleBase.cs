namespace Framework
{
    public interface ILevelObject
    {
        void OnLevelStart();
    }

    /// <summary>
    /// A type of behaviour associated with a Game. 
    /// Attach to a Game and this will be automatically invoked once the game state is InGame.
    /// </summary>
    public abstract class GameRuleBase : MonoBehaviourEx
    {
        public void Initialize(GameController gameController)
        {
            OnLevelStart(gameController);
        }

        protected virtual void OnLevelStart(GameController gameController)
        {
            foreach (var levelObject in InterfaceHelper.FindObjects<ILevelObject>())
            {
                levelObject.OnLevelStart();
            }
        }
    }
}
