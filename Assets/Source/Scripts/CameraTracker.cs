using UnityEngine;

namespace Source.Scripts
{
    public class CameraTracker : MonoBehaviour
    {
        private Transform _mainCamera;
        
        public void Init(Transform cameraTransform, float offsetY)
        {
            _mainCamera = cameraTransform;
            _mainCamera.parent = transform;
            _mainCamera.localPosition = Vector3.up * offsetY;
        }

        private void OnDestroy()
        {
            if(_mainCamera)
                _mainCamera.parent = null;
        }
    }
}