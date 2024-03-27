using System.Collections.Generic;
using Colyseus;
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

        private async void Connection()
        {
            _room = await client.JoinOrCreate<State>(GameRoomName);
            _room.OnStateChange += OnChange;
        }

        private void OnChange(State state, bool isfirststate)
        {
            if(isfirststate == false)
                return;
            
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

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            LeaveRoom();
        }

        public void LeaveRoom() => 
            _room?.Leave();

        public void SendMessage(string key, Dictionary<string, object> data) => 
            _room.Send(key, data);

        #endregion

        #region Player
        
        [SerializeField] private LocalInput _localInputPrefab;
        [SerializeField] private Snake _snakePrefab;
        
        private void CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
            snake.Init(player.d);
            LocalInput localInput = Instantiate(_localInputPrefab);
            localInput.Init(snake);
        }
        
        #endregion

        #region Enemy

        private Dictionary<string, RemoteInput> _enemies = new Dictionary<string, RemoteInput>();
        
        private void CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
            snake.Init(player.d);
            RemoteInput enemy = snake.gameObject.AddComponent<RemoteInput>();
            enemy.Init(player, snake);
            
            _enemies.Add(key, enemy);
        }

        private void RemoveEnemy(string key, Player value)
        {
            if (_enemies.ContainsKey(key) == false)
            {
                Debug.LogWarning($"Attempt to delete a non-existent enemy by key {key}");
                return;
            }
            RemoteInput enemy = _enemies[key];
            _enemies.Remove(key);
            enemy.Destroy();
        }
        
        #endregion
    }
}
