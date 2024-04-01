using UnityEngine;

namespace Source.Scripts
{
    public class CollisionChecker
    {
        private readonly Transform _snakeHead;
        private readonly float _overlapRadius; 

        public CollisionChecker(Transform snakeHead, float overlapRadius)
        {
            _snakeHead = snakeHead;
            _overlapRadius = overlapRadius;
        }

        public void CheckCollision()
        {
            Collider[] others = Physics.OverlapSphere(_snakeHead.position, _overlapRadius);

            foreach (Collider collider in others)
                if (collider.TryGetComponent(out Apple apple))
                    apple.Collect();
        }
    }
}