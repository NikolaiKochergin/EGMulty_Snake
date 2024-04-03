using UnityEngine;

namespace Source.Scripts
{
    public class DeathParticle : MonoBehaviour
    {
        [SerializeField] private GameObject _deathParticlePrefab;
        
        private void OnDestroy() => 
            Instantiate(_deathParticlePrefab, transform.position, transform.rotation);
    }
}