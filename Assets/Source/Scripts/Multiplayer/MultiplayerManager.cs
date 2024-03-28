using System.Collections.Generic;
using Colyseus;
using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts.Multiplayer
{
    public class MultiplayerManager : ColyseusManager<MultiplayerManager>
    {
        #region Server
        
        private const string GameRoomName = "state_handler";

        private ColyseusRoom<State> _room;
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            InitializeClient();
            Connection();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            LeaveRoom();
        }

        public void SendMessage(string key, Dictionary<string, object> data) => 
            _room.Send(key, data);

        private async void Connection()
        {
            _room = await client.JoinOrCreate<State>(GameRoomName);
            _room.OnStateChange += OnChange;
        }

        private void OnChange(State state, bool isFirstState)
        {
            _room.OnStateChange -= OnChange;
            
            state.players.ForEach((key, player) =>
            {
                if (key == _room.SessionId)
                    CreatePlayer(player);
                else
                    CreateEnemy(key, player);
            });
            
            _room.State.players.OnAdd += CreateEnemy;
            _room.State.players.OnRemove += RemoveEnemy;
        }

        private void LeaveRoom()
        {
            RemovePlayer();
            _room?.Leave();
        }

        #endregion

        #region Player

        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private CameraTracker _mainCamera;
        [SerializeField] private Snake _snakePrefab;
        [SerializeField] private LocalInput _localInputPrefab;

        private Snake _playerSnake;
        private LocalInput _playerLocalInput;
        
        private void CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            Quaternion rotation = Quaternion.identity;
            
            _playerSnake = Instantiate(_snakePrefab, position, rotation);
            int materialIndex = player.c % _gameSettings.PlayerSettings.MaterialSetups.Count;
            _playerSnake.Init(player.d, _gameSettings.PlayerSettings.Speed, _gameSettings.PlayerSettings.MaterialSetups[materialIndex]);
            
            RemoteInput playerRemoteInput = _playerSnake.gameObject.AddComponent<RemoteInput>();
            playerRemoteInput.Init(player, _playerSnake);
            
            _playerLocalInput = Instantiate(_localInputPrefab);
            _playerLocalInput.Init(_playerSnake.transform, _gameSettings.PlayerSettings);
            
            _mainCamera.Init(_playerSnake.transform, _gameSettings.CameraOffsetY);
        }

        private void RemovePlayer()
        {
            _playerLocalInput.Destroy();
            _playerSnake.Destroy();
        }
        
        #endregion

        #region Enemy

        private readonly Dictionary<string, Snake> _enemies = new Dictionary<string, Snake>();
        
        private void CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            
            Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
            int materialIndex = player.c % _gameSettings.PlayerSettings.MaterialSetups.Count;
            snake.Init(player.d, _gameSettings.PlayerSettings.Speed, _gameSettings.PlayerSettings.MaterialSetups[materialIndex]);
            
            RemoteInput remoteInput = snake.gameObject.AddComponent<RemoteInput>();
            remoteInput.Init(player, snake);
            
            _enemies.Add(key, snake);
        }

        private void RemoveEnemy(string key, Player value)
        {
            if (_enemies.ContainsKey(key) == false)
            {
                Debug.LogWarning($"Attempt to delete a non-existent enemy by key {key}");
                return;
            }
            Snake enemy = _enemies[key];
            _enemies.Remove(key);
            enemy.Destroy();
        }
        
        #endregion
    }
}
