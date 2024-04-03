using Source.Scripts.Multiplayer;
using Source.Scripts.StaticData;
using UnityEngine;

namespace Source.Scripts
{
    public class Snake : MonoBehaviour
    {
        private const string PlayerLayer = "Player";
        
        [SerializeField] private MaterialSetter _headModel;
        [SerializeField] private Tail _tailPrefab;

        private Tail _tail;
        private float _speed;
        private string _clientID;

        public Transform Head { get; private set; }
        
        private void Update() => 
            Move();

        public void Init(string clientID, int detailCount, float moveSpeed, MaterialSetup materialSetup, bool isPlayer = false)
        {
            _clientID = clientID;
            int layerIndex = LayerMask.NameToLayer(PlayerLayer);
            
            if (isPlayer)
            {
                gameObject.layer = layerIndex;
                Transform[] childrens = GetComponentsInChildren<Transform>();
                foreach (Transform children in childrens) 
                    children.gameObject.layer = layerIndex;
            }
            
            Head = _headModel.transform;
            _speed = moveSpeed;
            _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
            _tail.Init(_headModel, detailCount, materialSetup, layerIndex, isPlayer);
        }

        public void Destroy()
        {
            DetailPositions detailPositions = _tail.GetDetailPositions().AsDetailPositionsData();
            detailPositions.id = _clientID;
            string json = JsonUtility.ToJson(detailPositions);
            MultiplayerManager.Instance.SendMessage(MessageNames.gameOver, json);
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