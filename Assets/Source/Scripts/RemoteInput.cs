using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

namespace Source.Scripts
{
    public class RemoteInput : MonoBehaviour
    {
        private Player _player;
        private Snake _snake;

        public void Init(Player player, Snake snake)
        {
            _player = player;
            _snake = snake;
            player.OnChange += OnPlayerChange;
        }

        public void Destroy()
        {
            _player.OnChange -= OnPlayerChange;
            _snake.Destroy();
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