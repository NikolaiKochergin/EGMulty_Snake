using UnityEngine;

namespace Source.Scripts
{
    public class Snake : MonoBehaviour
    {
        [SerializeField] private Transform _head;
        [SerializeField] private float _speed = 2;
        [SerializeField] private float _rotateSpeed = 90f;
        
        private Vector3 _targetDirection = Vector3.forward;
        
        private void Update()
        {
            Rotate();
            Move();
        }

        private void Rotate()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
            
            _head.rotation = Quaternion
                .RotateTowards(
                    _head.rotation,
                    targetRotation,
                    _rotateSpeed * Time.deltaTime);
        }

        private void Move() => 
            transform.position += _head.forward * Time.deltaTime * _speed;

        public void LookAt(Vector3 cursorPosition) =>
            _targetDirection = cursorPosition - _head.position;
    }
}