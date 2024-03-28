using System;
using UnityEngine;

namespace Source.Scripts.StaticData
{
    [Serializable]
    public class MaterialSetup
    {
        [SerializeField] private Material _head;
        [SerializeField] private Material _detail;
        [SerializeField] private Material _tail;

        public Material Head => _head;
        public Material Detail => _detail;
        public Material Tail => _tail;
    }
}