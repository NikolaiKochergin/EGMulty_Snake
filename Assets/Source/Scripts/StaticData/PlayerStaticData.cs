using System;
using UnityEngine;

namespace Source.Scripts.StaticData
{
    [Serializable]
    public class PlayerStaticData
    {
        [SerializeField, Min(0)] private float _speed = 6f;
        [SerializeField, Min(0)] private float _rotateSpeed = 90f;
        
        public float Speed => _speed;
        public float RotateSpeed => _rotateSpeed;
    }
}