using UnityEngine;

namespace Framework
{
    public class CameraControllerMouse2D : MonoBehaviour, IGameCameraController, IInputHandler
    {
        [SerializeField] private float _sensitivity = 1f;

        private Vector2 movePosition;

        public float Sensitivity
        {
            get { return _sensitivity; }
            set { _sensitivity = value; }
        }

        public void Awake()
        {
            InputManager.RegisterHandler(this);
        }

        void IGameCameraController.Update(GameCamera gameCamera)
        {
            gameCamera.transform.position += movePosition.ToVec3();
            movePosition = Vector2.zero;
        }

        void IInputHandler.HandleInput(InputButtonEvent action)
        {
            if (action.Type == InputActionType.Axis)
            {
                if (action.Action == InputMapCoreBindings.Horizontal)
                    movePosition.x += action.Delta * _sensitivity;

                if (action.Action == InputMapCoreBindings.Vertical)
                    movePosition.y += action.Delta * _sensitivity;
            }
        }
    }
}