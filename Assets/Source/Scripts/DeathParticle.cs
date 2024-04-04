using UnityEngine;

namespace Source.Scripts
{
    public class DeathParticle : MonoBehaviour
    {
        [SerializeField] private GameObject _deathParticlePrefab;
        
        private void OnDisable() => 
            Instantiate(_deathParticlePrefab, transform.position, transform.rotation);
    }
}