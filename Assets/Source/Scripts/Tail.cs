using System.Collections.Generic;
using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts
{
    public class Tail : MonoBehaviour
    {
        [SerializeField] private MaterialSetter _selfMaterialSetter;
        [SerializeField] private MaterialSetter _detailPrefab;
        [SerializeField] private float _detailDistance = 1;
        
        private readonly List<Transform> _details = new List<Transform>();
        private readonly List<Vector3> _positionHistory = new List<Vector3>();
        private readonly List<Quaternion> _rotationHistory = new List<Quaternion>();
        private Transform _head;
        private MaterialSetup _materialSetup;
        private int _playerLayer;
        private bool _isPlayer;

        public void Init(MaterialSetter head, int detailCount, MaterialSetup materialSetup, int playerLayer,
            bool isPlayer = false)
        {
            _playerLayer = playerLayer;
            _isPlayer = isPlayer;
            
            if (isPlayer) 
                SetPlayerLayer(gameObject);
            
            _materialSetup = materialSetup;
            _head = head.transform;
            head.SetMaterial(materialSetup.Head);
            _selfMaterialSetter.SetMaterial(materialSetup.Tail);
            _details.Add(transform);
            _positionHistory.Add(_head.position);
            _rotationHistory.Add(_head.rotation);
            _positionHistory.Add(transform.position);
            _rotationHistory.Add(transform.rotation);
            
            SetDetailCount(detailCount);
        }

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

        public void Destroy()
        {
            foreach (Transform detail in _details) 
                Destroy(detail.gameObject);
        }

        public void SetDetailCount(int detailCount)
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
            MaterialSetter detail = Instantiate(_detailPrefab, position, rotation);
            detail.SetMaterial(_materialSetup.Detail);
            _details.Insert(0, detail.transform);
            _positionHistory.Add(position);
            _rotationHistory.Add(rotation);
            
            if(_isPlayer)
                SetPlayerLayer(detail.gameObject);
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

        private void SetPlayerLayer(GameObject gameObject)
        {
            gameObject.layer = _playerLayer;
            Transform[] childrens = GetComponentsInChildren<Transform>();
            foreach (Transform children in childrens) 
                children.gameObject.layer = _playerLayer;
        }
    }
}