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
                if (collider.TryGetComponent(out Apple apple))
                    apple.Collect();
                else
                    GameOver();
        }

        private void GameOver() => 
            GameOverHappened?.Invoke();
    }
}