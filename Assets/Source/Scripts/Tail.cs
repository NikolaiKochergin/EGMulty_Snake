using System.Collections.Generic;
using UnityEngine;

namespace Source.Scripts
{
    public class Tail : MonoBehaviour
    {
        [SerializeField] private Transform _detailPrefab;
        [SerializeField] private float _detailDistance = 1;
        
        private readonly List<Transform> _details = new List<Transform>();
        private readonly List<Vector3> _positionHistory = new List<Vector3>();
        private readonly List<Quaternion> _rotationHistory = new List<Quaternion>();
        private Transform _head;

        private void Update()
        {
            float distance = (_head.position - _positionHistory[0]).magnitude;

            while(distance > _detailDistance)
            {
                Vector3 direction = (_head.position - _positionHistory[0]).normalized;
                
                _positionHistory.Insert(0, _positionHistory[0] + direction * _detailDistance);
                _positionHistory.RemoveAt(_positionHistory.Count - 1);
                
                _rotationHistory.Insert(0, _head.rotation);
                _rotationHistory.RemoveAt(_rotationHistory.Count - 1);

                distance -= _detailDistance;
            }

            for (int i = 0; i < _details.Count; i++)
            {
                float percent = distance / _detailDistance;
                _details[i].position = Vector3.Lerp(_positionHistory[i + 1], _positionHistory[i], percent);
                _details[i].rotation = Quaternion.Lerp(_rotationHistory[i + 1], _rotationHistory[i], percent);
            }
        }

        public void Init(Transform head, int detailCount)
        {
            _head = head;
            _details.Add(transform);
            _positionHistory.Add(_head.position);
            _rotationHistory.Add(_head.rotation);
            _positionHistory.Add(transform.position);
            _rotationHistory.Add(transform.rotation);
            
            SetDetailCount(detailCount);
        }

        public void Destroy()
        {
            foreach (Transform detail in _details) 
                Destroy(detail.gameObject);
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
            Quaternion rotation = _details[^1].rotation;
            Transform detail = Instantiate(_detailPrefab, position, rotation);
            _details.Insert(0, detail);
            _positionHistory.Add(position);
            _rotationHistory.Add(rotation);
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
            _rotationHistory.RemoveAt(_positionHistory.Count - 1);
        }
    }
}