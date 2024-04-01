using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Scripts.StaticData
{
    [Serializable]
    public class PlayerStaticData
    {
        [SerializeField, Min(0)] private float _speed = 6f;
        [SerializeField, Min(0)] private float _rotateSpeed = 90f;
        [SerializeField,Min(0)] private float _overlapRadius = 0.5f;
        [SerializeField] private List<MaterialSetup> _materialSetups;

        public float Speed => _speed;
        public float RotateSpeed => _rotateSpeed;
        public IReadOnlyList<MaterialSetup> MaterialSetups => _materialSetups;
        public float OverlapRadius => _overlapRadius;
    }
}