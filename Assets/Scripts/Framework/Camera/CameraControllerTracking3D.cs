using UnityEngine;

namespace Framework
{
    public class CameraControllerTracking3D : MonoBehaviour, IGameCameraController
    {
        [SerializeField] private Vector3 _lerpSpeed;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _lookAtOffset;
        [SerializeField] private Vector2 _panLimit;

        public Transform Target { get; set; }

        private Vector2 _panningOffset;

        void IGameCameraController.Update(GameCamera gameCamera)
        {
            if (Target == null)
                return;

            var trackingPosition = Target.position.Multiply(Vector3Ex.XZ) + new Vector3(_panningOffset.x, 0f, _panningOffset.y);

            var cameraPosition = gameCamera.transform.position;

            cameraPosition.x = Mathf.Lerp(gameCamera.transform.position.x, trackingPosition.x + _offset.x, Time.deltaTime / _lerpSpeed.x);
            cameraPosition.y = Mathf.Lerp(gameCamera.transform.position.y, trackingPosition.y + _offset.y, Time.deltaTime / _lerpSpeed.y);
            cameraPosition.z = Mathf.Lerp(gameCamera.transform.position.z, trackingPosition.z + _offset.z, Time.deltaTime / _lerpSpeed.z);

            gameCamera.transform.position = cameraPosition;

            transform.LookAt(cameraPosition - new Vector3(0f, _offset.y, 0f) + _lookAtOffset);
        }

        public void Pan(Vector2 inputDirection)
        {
            _panningOffset = inputDirection * _panLimit;
        }
    }
}