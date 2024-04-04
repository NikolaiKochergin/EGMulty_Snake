using System.Collections.Generic;
using System.Linq;
using Colyseus;
using Source.Scripts.StaticData;
using UnityEngine;
using UnityEngine.UI;

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
        
        public void SendMessage(string key, string data) => 
            _room.Send(key, data);

        private async void Connection()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "login", PlayerSettings.Instance.Login },
            };
            
            _room = await client.JoinOrCreate<State>(GameRoomName, data);
            _room.OnStateChange += OnChange;
        }

        private void OnChange(State state, bool isFirstState)
        {
            _room.OnStateChange -= OnChange;
            
            state.players.ForEach(CreateSnake);
            
            CreatePlayer();
            
            _room.State.players.OnAdd += CreateSnake;
            _room.State.players.OnRemove += RemoveSnake;

            _room.State.apples.ForEach(CreateApple);
            _room.State.apples.OnAdd += OnAddApple;
            _room.State.apples.OnRemove += RemoveApple;
        }

        private void OnAddApple(int key, Vector2Float apple) => CreateApple(apple);

        private void LeaveRoom()
        {
            _room.State.players.OnAdd -= CreateSnake;
            _room.State.players.OnRemove -= RemoveSnake;
            _room.State.apples.OnAdd -= OnAddApple;
            _room.State.apples.OnRemove -= RemoveApple;
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
        
        private void CreatePlayer()
        {
            _playerSnake = _snakes[_room.SessionId];
            
            _playerLocalInput = Instantiate(_localInputPrefab);
            _playerLocalInput.Init(_playerSnake.Head.transform, _gameSettings.PlayerSettings);
            
            _mainCamera.Init(_playerSnake.transform, _gameSettings.CameraOffsetY);

            _playerLocalInput.GameOverHappened += OnGameOverHappened;
            
        }

        private void OnGameOverHappened()
        {
            _playerLocalInput.GameOverHappened -= OnGameOverHappened;
            RemovePlayer();
        }

        private void RemovePlayer()
        {
            RemoveLeader(_room.SessionId);
            _mainCamera.transform.parent = null;
            if(_playerLocalInput)
                _playerLocalInput.Destroy();
            if(_playerSnake)
                _playerSnake.Destroy();
        }

        #endregion

        #region Snake
        
        private readonly Dictionary<string, Snake> _snakes = new Dictionary<string, Snake>();

        private void CreateSnake(string key, Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
            int materialIndex = player.c % _gameSettings.PlayerSettings.MaterialSetups.Count;
            snake.Init(key, player.login, player.d, _gameSettings.PlayerSettings.Speed, _gameSettings.PlayerSettings.MaterialSetups[materialIndex], key == _room.SessionId);
            
            RemoteInput remoteInput = snake.gameObject.AddComponent<RemoteInput>();
            remoteInput.Init(key, player, snake);
            
            _snakes.Add(key, snake);
            AddLeader(key, player);
        }

        private void RemoveSnake(string key, Player value)
        {
            if(key == _room.SessionId)
                return;
            
            RemoveLeader(key);
            
            if (_snakes.ContainsKey(key) == false)
            {
                Debug.LogWarning($"Attempt to delete a non-existent enemy by key {key}");
                return;
            }
            Snake snake = _snakes[key];
            _snakes.Remove(key);
            snake.Destroy();
        }
        
        #endregion

        #region Apple

        [SerializeField] private Apple _applePrefab;

        private Dictionary<Vector2Float, Apple> _apples = new Dictionary<Vector2Float, Apple>();
        
        private void CreateApple(Vector2Float vector2Float)
        {
            Vector3 position = new Vector3(vector2Float.x, 0, vector2Float.z);
            Apple apple = Instantiate(_applePrefab, position, Quaternion.identity);
            apple.Init(vector2Float);
            _apples.Add(vector2Float, apple);
        }

        private void RemoveApple(int key, Vector2Float vector2Float)
        {
            if(_apples.ContainsKey(vector2Float) == false)
                return;

            Apple apple = _apples[vector2Float];
            _apples.Remove(vector2Float);
            apple.Destroy();
        }

        #endregion

        #region Leaderboard

        private class LoginScorePair
        {
            public string Login;
            public float Score;
        }
        
        [SerializeField] private Text _text;

        private Dictionary<string, LoginScorePair> _leaders = new Dictionary<string, LoginScorePair>();

        private void AddLeader(string sessionID, Player player)
        {
            if(_leaders.ContainsKey(sessionID))
                return;
            
            _leaders.Add(sessionID, new LoginScorePair
            {
                Login = player.login, 
                Score = player.score
            });
            
            UpdateBoard();
        }

        private void RemoveLeader(string sessionID)
        {
            if(_leaders.ContainsKey(sessionID) == false)
                return;
            _leaders.Remove(sessionID);
            
            UpdateBoard();
        }

        public void UpdateScore(string sessionID, int score)
        {
            if(_leaders.ContainsKey(sessionID) == false)
                return;

            _leaders[sessionID].Score = score;
            UpdateBoard();
        }

        private void UpdateBoard()
        {
            int topCount = Mathf.Clamp(_leaders.Count, 0, 8);
            IEnumerable<KeyValuePair<string, LoginScorePair>> top8 = _leaders.OrderByDescending(pair => pair.Value.Score).Take(topCount);

            string text = "";
            int i = 1;
            foreach (KeyValuePair<string,LoginScorePair> item in top8)
            {
                text += $"{i}. {item.Value.Login}: {item.Value.Score}\n";
                i++;
            }
            
            _text.text = text;
        }

        #endregion
    }
}
