using System.Collections.Generic;
using UnityEngine;

namespace Source.Scripts
{
    public class Tail : MonoBehaviour
    {
        [SerializeField] private Transform _detailPrefab;
        [SerializeField] private float _detailDistance = 1;
        
        private readonly List<Vector3> _positionHistory = new List<Vector3>();
        private readonly List<Transform> _details = new List<Transform>();
        private Transform _head;
        private float _snakeSpeed = 2f;

        public void Init(Transform head, float speed, int detailCount)
        {
            _head = head;
            _snakeSpeed = speed;
            _details.Add(transform);
            _positionHistory.Add(_head.position);
            _positionHistory.Add(transform.position);
            
            SetDetailCount(detailCount);
        }

        public void Destroy()
        {
            foreach (Transform detail in _details) 
                Destroy(detail.gameObject);
        }

        private void Update()
        {
            float distance = (_head.position - _positionHistory[0]).magnitude;

            while(distance > _detailDistance)
            {
                Vector3 direction = (_head.position - _positionHistory[0]).normalized;
                
                _positionHistory.Insert(0, _positionHistory[0] + direction * _detailDistance);
                _positionHistory.RemoveAt(_positionHistory.Count - 1);

                distance -= _detailDistance;
            }

            for (int i = 0; i < _details.Count; i++)
            {
                _details[i].position =
                    Vector3.Lerp(_positionHistory[i + 1], _positionHistory[i], distance / _detailDistance);

                // Vector3 direction = (_positionHistory[i] - _positionHistory[i + 1]).normalized;
                // _details[i].position += direction * _snakeSpeed * Time.deltaTime;
            }
        }

        private void SetDetailCount(int detailCount)
        {
            if(detailCount == _details.Count - 1)
                return;

            int diff = _details.Count - 1 - detailCount;

            if (diff < 1)
            {
                for (int i = 0; i < -diff; i++)
                {
                    AddDetail();
                }
            }
            else
            {
                for (int i = 0; i < diff; i++)
                {
                    RemoveDetail();
                }
            }
        }

        private void AddDetail()
        {
            Vector3 position = _details[^1].position;
            Transform detail = Instantiate(_detailPrefab, position, Quaternion.identity);
            _details.Insert(0, detail);
            _positionHistory.Add(position);
        }

        private void RemoveDetail()
        {
            if (_details.Count <= 1)
            {
                Debug.Log("Пытаемся удалить деталь которой нет.");
                return;
            }

            Transform detail = _details[0];
            _details.Remove(detail);
            Destroy(detail.gameObject);
            _positionHistory.RemoveAt(_positionHistory.Count - 1);
        }
    }
}