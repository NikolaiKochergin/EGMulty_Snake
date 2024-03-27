using System.Collections.Generic;
using Source.Scripts.Multiplayer;
using UnityEngine;

namespace Source.Scripts
{
    public class LocalInput : MonoBehaviour
    {
        [SerializeField] private Transform _cursor;

        private MultiplayerManager _multiplayerManager;
        private Snake _snake;
        private Camera _camera;
        private Plane _plane;

        public void Init(Snake snake)
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _snake = snake;
            _camera = Camera.main;
            _plane = new Plane(Vector3.up,Vector3.zero);
        }

        private void Update()
        {
            if (!Input.GetMouseButton(0)) 
                return;
            
            MoveCursor();
            _snake.LookAt(_cursor.position);

            SendMove();
        }

        private void SendMove()
        {
            _snake.GetMoveInfo(out Vector3 position);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "x", position.x },
                { "z", position.z },
            };
            
            _multiplayerManager.SendMessage(MessageNames.move, data);
        }

        private void MoveCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out float distance)) 
                _cursor.position = ray.GetPoint(distance);
        }
    }
}
