using System;
using System.Collections.Generic;
using Source.Scripts.Multiplayer;
using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts
{
    public class LocalInput : MonoBehaviour
    {
        [SerializeField] private Transform _cursor;
        [SerializeField]private PlayerAim _playerAimPrefab;

        private MultiplayerManager _multiplayerManager;
        private Camera _camera;
        private Plane _plane;
        private PlayerAim _playerAim;
        private CollisionChecker _collisionChecker;
        private Transform _snakeHead;

        public event Action GameOverHappened;

        public void Init(Transform snakeHead, PlayerStaticData playerSettings)
        {
            _snakeHead = snakeHead;
            _multiplayerManager = MultiplayerManager.Instance;
            _camera = Camera.main;
            _plane = new Plane(Vector3.up,Vector3.zero);
            _playerAim = Instantiate(_playerAimPrefab, snakeHead.position, snakeHead.rotation);
            _playerAim.Init(playerSettings.Speed, playerSettings.RotateSpeed);
            _collisionChecker = new CollisionChecker(snakeHead, playerSettings.CollisionMask, playerSettings.OverlapRadius);
            _collisionChecker.GameOverHappened += () => GameOverHappened?.Invoke();
        }

        public void Destroy()
        {
            _playerAim.Destroy();
            Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveCursor();
                _playerAim.SetTarget(_cursor.position);
            }
            
            SendMove();
            CheckExit();
        }

        private void FixedUpdate() => 
            _collisionChecker.CheckCollision();

        private void MoveCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out float distance)) 
                _cursor.position = ray.GetPoint(distance);
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

        private void CheckExit()
        {
            if(Math.Abs(_snakeHead.position.x) > 128 || Math.Abs(_snakeHead.position.z) > 128)
                GameOverHappened?.Invoke();
        }
    }
}
