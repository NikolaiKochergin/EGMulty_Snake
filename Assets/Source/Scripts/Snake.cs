using UnityEngine;

namespace Source.Scripts
{
    public class Snake : MonoBehaviour
    {
        [SerializeField] private Transform _head;
        [SerializeField] private Tail _tailPrefab;
        [SerializeField] private float _speed = 2;
        [SerializeField] private float _rotateSpeed = 90f;

        private Tail _tail;
        private Vector3 _targetDirection = Vector3.forward;
        
        private void Update()
        {
            Rotate();
            Move();
        }

        public void Init(int detailCount)
        {
            _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
            _tail.Init(_head, detailCount);
        }

        public void Destroy()
        {
            _tail.Destroy();
            Destroy(gameObject);
        }

        private void Rotate() =>
            _head.rotation = Quaternion
                .RotateTowards(
                    _head.rotation,
                    Quaternion.LookRotation(_targetDirection),
                    _rotateSpeed * Time.deltaTime);

        private void Move() => 
            transform.position += _head.forward * Time.deltaTime * _speed;

        public void LookAt(Vector3 cursorPosition) =>
            _targetDirection = cursorPosition - _head.position;

        public void GetMoveInfo(out Vector3 position) => 
            position = transform.position;
    }
}