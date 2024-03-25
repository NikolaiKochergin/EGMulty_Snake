using System;
using UnityEngine;

namespace Source.Scripts
{
    public class LocalInput : MonoBehaviour
    {
        [SerializeField] private Snake _snake;
        [SerializeField] private Transform _cursor;
        
        private Camera _camera;
        private Plane _plane;


        private void Awake()
        {
            _camera = Camera.main;
            _plane = new Plane(Vector3.up,Vector3.zero);
        }

        private void Update()
        {
            if (!Input.GetMouseButton(0)) return;
            MoveCursor();
            _snake.LookAt(_cursor.position);
        }

        private void MoveCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out float distance)) 
                _cursor.position = ray.GetPoint(distance);
        }
    }
}
