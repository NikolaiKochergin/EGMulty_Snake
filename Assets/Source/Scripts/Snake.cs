using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts
{
    public class Snake : MonoBehaviour
    {
        [SerializeField] private MaterialSetter _headModel;
        [SerializeField] private Tail _tailPrefab;

        private Transform _head;
        private Tail _tail;
        private float _speed;
        
        private void Update() => 
            Move();

        public void Init(int detailCount, float moveSpeed, MaterialSetup materialSetup)
        {
            _speed = moveSpeed;
            _head = _headModel.transform;
            _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
            _tail.Init(_headModel, detailCount, materialSetup);
        }

        public void Destroy()
        {
            _tail.Destroy();
            Destroy(gameObject);
        }

        public void SetDetailCount(int value) => 
            _tail.SetDetailCount(value);

        public void SetRotation(Vector3 pointToLook) => 
            _head.LookAt(pointToLook);

        private void Move() => 
            transform.position += _head.forward * Time.deltaTime * _speed;
    }
}