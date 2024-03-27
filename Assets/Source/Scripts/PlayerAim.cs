using UnityEngine;

namespace Source.Scripts
{
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed = 90f;
        
        private Vector3 _targetDirection = Vector3.forward;
        private float _speed;

        public void Init(float speed) => 
            _speed = speed;

        public void SetTargetDirection(Vector3 pointToLook) => 
            _targetDirection = pointToLook - transform.position;

        public void GetMoveInfo(out Vector3 position) => 
            position = transform.position;

        private void Update()
        {
            Rotate();
            Move();
        }

        private void Rotate() =>
            transform.rotation = Quaternion
                .RotateTowards(
                    transform.rotation, 
                    Quaternion.LookRotation(_targetDirection), 
                    _rotateSpeed * Time.deltaTime);

        private void Move() => 
            transform.position += transform.forward * _speed * Time.deltaTime;
    }
}