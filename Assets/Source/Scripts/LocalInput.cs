using System.Collections.Generic;
using Colyseus.Schema;
using Source.Scripts.Multiplayer;
using UnityEngine;

namespace Source.Scripts
{
    public class LocalInput : MonoBehaviour
    {
        [SerializeField] private Transform _cursor;
        [SerializeField] private float _cameraOffsetY;

        private MultiplayerManager _multiplayerManager;
        private PlayerAim _playerAim;
        private Player _player;
        private Snake _snake;
        private Camera _camera;
        private Plane _plane;

        public void Init(PlayerAim aim, Player player, Snake snake)
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _playerAim = aim;
            _player = player;
            _snake = snake;
            _camera = Camera.main;
            _plane = new Plane(Vector3.up,Vector3.zero);
            _snake.gameObject.AddComponent<CameraTracker>().Init(_camera.transform, _cameraOffsetY);
            _player.OnChange += OnPlayerChange;
        }

        public void Destroy()
        {
            _player.OnChange -= OnPlayerChange;
            _snake.Destroy();
        }

        private void Update()
        {
            if (!Input.GetMouseButton(0)) 
                return;
            
            MoveCursor();
            _playerAim.SetTargetDirection(_cursor.position);

            SendMove();
        }

        private void SendMove()
        {
            _playerAim.GetMoveInfo(out Vector3 position);

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

        private void OnPlayerChange(List<DataChange> changes)
        {
            Vector3 position = _snake.transform.position;
            
            foreach (DataChange change in changes)
            {
                switch (change.Field)
                {
                    case "x":
                        position.x = (float) change.Value;
                        break;
                    case "z":
                        position.z = (float) change.Value;
                        break;
                    case "d":
                        _snake.SetDetailCount((byte)change.Value);
                        break;
                    default:
                        Debug.LogWarning($"The {change.Field} field change is not being processed");
                        break;
                }
            }
            
            _snake.SetRotation(position);
        }
    }
}
