using UnityEngine;

namespace Source.Scripts.StaticData
{
    [CreateAssetMenu(menuName = nameof(GameSettings), fileName = nameof(GameSettings))]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private PlayerStaticData _playerSettings;
        [SerializeField, Min(0)] private float _cameraOffsetY = 16f;

        public PlayerStaticData PlayerSettings => _playerSettings;
        public float CameraOffsetY => _cameraOffsetY;
    }
}