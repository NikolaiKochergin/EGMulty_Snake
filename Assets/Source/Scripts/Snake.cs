using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts
{
    public class Snake : MonoBehaviour
    {
        [SerializeField] private MaterialSetter _headModel;
        [SerializeField] private Tail _tailPrefab;

        private Tail _tail;
        private float _speed;
        
        public Transform Head { get; private set; }
        
        private void Update() => 
            Move();

        public void Init(int detailCount, float moveSpeed, MaterialSetup materialSetup)
        {
            Head = _headModel.transform;
            _speed = moveSpeed;
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
            Head.LookAt(pointToLook);

        private void Move() => 
            transform.position += Head.forward * Time.deltaTime * _speed;
    }
}