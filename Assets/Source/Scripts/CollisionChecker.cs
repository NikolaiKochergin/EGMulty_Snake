using System;
using UnityEngine;

namespace Source.Scripts
{
    public class CollisionChecker
    {
        private readonly Transform _snakeHead;
        private readonly LayerMask _mask;
        private readonly float _overlapRadius;

        public CollisionChecker(Transform snakeHead, LayerMask mask, float overlapRadius)
        {
            _snakeHead = snakeHead;
            _mask = mask;
            _overlapRadius = overlapRadius;
        }

        public event Action GameOverHappened;

        public void CheckCollision()
        {
            Collider[] others = Physics.OverlapSphere(_snakeHead.position, _overlapRadius, _mask);

            foreach (Collider collider in others)
            {
                if (collider.TryGetComponent(out Apple apple))
                {
                    apple.Collect();
                }
                else
                {
                    if (collider.GetComponentInParent<Snake>())
                    {
                        Transform enemy = collider.transform;
                        Vector3 enemyPosition = enemy.position;
                        Vector3 playerPosition = _snakeHead.position;
                        float playerAngle = Vector3.Angle(enemyPosition - playerPosition, _snakeHead.forward); 
                        float enemyAngle = Vector3.Angle(playerPosition - enemyPosition, enemy.forward);
                        
                        if(playerAngle < enemyAngle + 5)
                            GameOver();
                    }
                    else
                    {
                        GameOver();
                    }
                }
            }
        }

        private void GameOver() => 
            GameOverHappened?.Invoke();
    }
}