using System.Collections.Generic;
using Colyseus.Schema;
using Source.Scripts.Multiplayer;
using UnityEngine;

namespace Source.Scripts
{
    public class RemoteInput : MonoBehaviour
    {
        private Player _player;
        private Snake _snake;
        private string _clientID;

        public void Init(string clientID, Player player, Snake snake)
        {
            _clientID = clientID;
            _player = player;
            _snake = snake;
            player.OnChange += OnPlayerChange;
        }

        private void OnDestroy() => 
            _player.OnChange -= OnPlayerChange;

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
                    case "score":
                        MultiplayerManager.Instance.UpdateScore(_clientID, (ushort)change.Value);
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