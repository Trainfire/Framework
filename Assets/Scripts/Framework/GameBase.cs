using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

namespace Framework
{
    public enum GameZone
    {
        None,
        Loading,
        InFrontEnd,
        InGame,
    }

    public abstract class GameBase : MonoBehaviour
    {
        private List<GameRuleBase> _rules;

        [SerializeField] private PlayerBase _playerPrototype;
        [SerializeField] private GameCamera _cameraPrototype;
        [SerializeField] private ConsoleView _consolePrototype;

        public PlayerBase BasePlayer { get; private set; }
        public GameController Controller { get; private set; }
        protected ConsoleController Console { get; private set; }
        protected SceneLoader SceneLoader { get; private set; }
        private InputHelper InputHelper { get; set; }

        private StateManager _stateManager;
        public StateListener StateListener
        {
            get { return _stateManager.Listener; }
        }

        private ZoneManager<GameZone> _zoneManager;
        public ZoneListener<GameZone> ZoneListener
        {
            get { return _zoneManager.Listener; }
        }

        public bool Initialized { get; private set; }
        public GameCamera Camera { get; private set; }

        public void Initialize(params string[] args)
        {
            if (Initialized)
            {
                Debug.LogError("Game has already been initialized. This should not happen!");
                return;
            }

            Initialized = true;

            // Validate Prototypes
            if (!IsPrototypeValid(_playerPrototype))
                return;

            // Camera
            if (IsPrototypeValid(_cameraPrototype))
            {
                Camera = Instantiate(_cameraPrototype);
                Camera.name = "Camera";

                Assert.IsTrue(Camera.tag == "MainCamera", "Expected the GameCamera tag to be MainCamera");
            }

            // Console
            if (IsPrototypeValid(_consolePrototype))
            {
                Console = new ConsoleController();

                var consoleView = GameObject.Instantiate(_consolePrototype);
                consoleView.SetConsole(Console);
            }

            // Relay
            gameObject.GetOrAddComponent<MonoEventRelay>();

            // State
            _stateManager = new StateManager();
            _stateManager.Listener.StateChanged += Listener_StateChanged;

            // Scene Loader
            SceneLoader = gameObject.GetOrAddComponent<SceneLoader>();
            SceneLoader.LoadingScene = "Loader";

            // Zone
            _zoneManager = new ZoneManager<GameZone>(SceneLoader);
            _zoneManager.Listener.ZoneChanged += Listener_ZoneChanged;

            // Allows the control of the game, such as level loading, resuming and pausing the game.
            Controller = new GameController(this, _stateManager, _zoneManager);

            // Audio
            Services.Provide(FindObjectOfType<AudioSystem>());

            // Game Entities
            new GameEntityManager(this);

            // Rules
            _rules = new List<GameRuleBase>();

            // Input
            InputHelper = new InputHelper(gameObject);
            InputManager.RegisterMaps(InputHelper.Maps);
            OnRegisterInputs(InputHelper);

            if (args != null && args.Length != 0 && args[0] != "Main")
            {
                Controller.LoadLevel(args[0]);
            }
            else
            {
                Controller.LoadFrontEnd();
            }

            // Game implementation
            OnInitialize(args);
        }

        protected virtual void OnInitialize(params string[] args) { }
        protected virtual void OnRegisterInputs(InputHelper inputHelper) { }

        protected virtual void AddGameRule<T>() where T : GameRuleBase
        {
            var rule = gameObject.GetOrAddComponent<T>();
            _rules.Add(rule);
        }

        private bool IsPrototypeValid<T>(T component) where T : Component
        {
            if (component == null)
            {
                DebugEx.LogError<GameBase>($"{ typeof(T).Name } reference is invalid. Make sure you are referencing a prefab with this component attached.");
                return false;
            }

            return true;
        }

        private void Listener_StateChanged(State state)
        {
            // Resume / Pause Game
            Time.timeScale = state == State.Running ? 1f : 0f;
        }

        private void Listener_ZoneChanged(GameZone gameZone)
        {
            _stateManager.SetState(State.Running);

            if (gameZone == GameZone.InGame)
            {
                if (IsPrototypeValid(_playerPrototype))
                {
                    BasePlayer = Instantiate(_playerPrototype);
                    BasePlayer.name = "Player";
                }

                _rules.ForEach(x => x.Initialize(Controller));
            }
        }
    }
}
