using System.Collections.Generic;
using Colyseus.Schema;
using Source.Scripts.Multiplayer;
using UnityEngine;

namespace Source.Scripts
{
    public class Apple : MonoBehaviour
    {
        private Vector2Float _apple;

        public void Init(Vector2Float apple)
        {
            _apple = apple;
            _apple.OnChange += OnChange;
        }

        public void Destroy()
        {
            if(_apple != null)
                _apple.OnChange -= OnChange;
            Destroy(gameObject);
        }

        public void Collect()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"id", _apple.id}
            };
            
            MultiplayerManager.Instance.SendMessage(MessageNames.collect, data);
            
            gameObject.SetActive(false);
        }

        private void OnChange(List<DataChange> changes)
        {
            Vector3 position = transform.position;
            foreach (DataChange change in changes)
            {
                switch (change.Field)
                {
                    case "x":
                        position.x = (float)change.Value;
                        break;
                    case "z":
                        position.z = (float) change.Value;
                        break;
                    default:
                        Debug.LogWarning("The apple does not respond to the field change" + change.Field);
                        break;
                }
            }
            transform.position = position;
            gameObject.SetActive(true);
        }
    }
}
