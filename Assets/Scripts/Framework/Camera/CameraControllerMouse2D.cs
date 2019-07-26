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

        void IInputHandler.HandleUpdate(InputHandlerEvent handlerEvent)
        {
            var x = 0.0f;
            var y = 0.0f;

            handlerEvent.GetSingleAxisEvent(InputMapCoreEventsRegister.Horizontal, (args) => x = args.Delta);
            handlerEvent.GetSingleAxisEvent(InputMapCoreEventsRegister.Vertical, (args) => y = args.Delta);

            movePosition += new Vector2(x, y) * _sensitivity;
        }
    }
}