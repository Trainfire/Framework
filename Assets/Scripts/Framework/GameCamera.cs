using UnityEngine;
using UnityEngine.Assertions;
using Framework.Components;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// Interface for a GameCameraController. This will get called by a GameCamera on every update.
    /// </summary>
    public interface IGameCameraController
    {
        void Update(GameCamera gameCamera);
    }

    public class GameCamera : MonoBehaviour
    {
        private Game _game;
        private IGameCameraController _controller;
        private List<ScreenEffect> _screenEffects;

        public Camera Camera { get; private set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Assert.IsNotNull(Camera);

            _screenEffects = new List<ScreenEffect>();
        }

        public void SetController(IGameCameraController controller)
        {
            _controller = controller;
        }

        private void FixedUpdate()
        {
            if (_controller != null)
                _controller.Update(this);

            var currentEffects = _screenEffects.ToArray();

            foreach (var effect in currentEffects)
            {
                if (effect.Finished)
                {
                    _screenEffects.Remove(effect);
                    Destroy(effect);
                }
                else
                {
                    effect.ProcessEffect();
                }
            }
        }

        public T AddScreenEffect<T>() where T : ScreenEffect
        {
            var effect = gameObject.AddComponent<T>();
            _screenEffects.Add(effect);
            return effect;
        }
    }

}
