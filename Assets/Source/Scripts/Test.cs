using UnityEngine;

namespace Source.Scripts
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private LocalInput _localInputPrefab;
        [SerializeField] private Snake _snakePrefab;
        [SerializeField] private int _detailCount = 1;

        private Snake _snake;
        private LocalInput _localInput;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_snake) 
                    _snake.Destroy();
                
                if(_localInput)
                    Destroy(_localInput.gameObject);
                
                _snake = Instantiate(_snakePrefab);
                _snake.Init(_detailCount);
                _localInput = Instantiate(_localInputPrefab);
                _localInput.Init(_snake);
            }
        }
    }
}