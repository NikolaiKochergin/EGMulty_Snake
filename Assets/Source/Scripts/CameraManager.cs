using System;
using UnityEngine;

namespace Source.Scripts
{
    public class CameraManager : MonoBehaviour
    {
        private Transform _mainCamera;
        
        private void Start()
        {
            if (Camera.main != null)
                _mainCamera = Camera.main.transform;
            else
                throw new ArgumentException("Main camera does not exist.");

            _mainCamera.parent = transform;
            _mainCamera.localPosition = Vector3.zero;
        }

        private void OnDestroy()
        {
            if(_mainCamera)
                _mainCamera.parent = null;
        }
    }
}