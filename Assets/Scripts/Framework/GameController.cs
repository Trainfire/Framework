using UnityEngine;

namespace Framework
{
    public class GameController : IInputHandler
    {
        private const string FrontEndScene = "frontend";
        private const string InGameScene = "ingame";

        private StateManager _stateManager;
        private ZoneManager<GameZone> _zoneManager;

        public Game Game { get; private set; }

        public GameController(Game game, StateManager stateManager, ZoneManager<GameZone> zoneManager)
        {
            Game = game;
            _stateManager = stateManager;
            _zoneManager = zoneManager;

            InputManager.RegisterHandler(this);
        }

        public void StartGame()
        {
            LoadLevel("level");
        }

        public void Resume()
        {
            _stateManager.SetState(State.Running);
        }

        public void Pause()
        {
            _stateManager.SetState(State.Paused);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void LoadFrontEnd()
        {
            _zoneManager.SetZone(GameZone.InFrontEnd, FrontEndScene);
        }

        public void LoadLevel(string sceneName)
        {
            // Find a bootstrapper in the scene and remove it.
            var bootstrapper = GameObject.FindObjectOfType<Bootstrapper>();
            if (bootstrapper != null)
                GameObject.Destroy(bootstrapper);

            _zoneManager.SetZone(GameZone.InGame, InGameScene, sceneName);
        }

        public void ReloadLevel()
        {
            _zoneManager.SetZone(GameZone.InGame, InGameScene, _zoneManager.ActiveScene);
        }

        void IInputHandler.HandleInput(InputButtonEvent action)
        {
            if (action.ID == InputMapCoreEventsRegister.Start && action.Type == InputActionType.Down)
            {
                _stateManager.ToggleState();
                Debug.Log("Game is now " + _stateManager.State);
            }
        }
    }
}
