using UnityEngine;

namespace Source.Scripts
{
    public class PlayerAim : MonoBehaviour
    {
        private Vector3 _targetDirection = Vector3.forward;
        private float _speed;
        private float _rotateSpeed;

        public void Init(float speed, float rotateSpeed)
        {
            _speed = speed;
            _rotateSpeed = rotateSpeed;
        }

        public void Destroy() => 
            Destroy(gameObject);

        public void SetTarget(Vector3 pointToLook) => 
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