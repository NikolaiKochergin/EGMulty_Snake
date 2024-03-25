using System.Collections.Generic;
using UnityEngine;

namespace Source.Scripts
{
    public class Tail : MonoBehaviour
    {
        [SerializeField] private Transform _head;
        [SerializeField] private List<Transform> _details;
        [SerializeField] private float _detailDistance = 1;
        [SerializeField] private float _snakeSpeed = 2;
        
        private List<Vector3> _positionHistory = new List<Vector3>();

        private void Awake()
        {
            _positionHistory.Add(_head.position);

            foreach (Transform detail in _details) 
                _positionHistory.Add(detail.position);
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

                Vector3 direction = (_positionHistory[i] - _positionHistory[i + 1]).normalized;
                _details[i].position += direction * _snakeSpeed * Time.deltaTime;
            }
        }
    }
}