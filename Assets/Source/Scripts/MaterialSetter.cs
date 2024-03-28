using UnityEngine;

namespace Source.Scripts
{
    public class MaterialSetter : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] _renderers;

        public void SetMaterial(Material value)
        {
            foreach (MeshRenderer renderer in _renderers) 
                renderer.material = value;
        }
    }
}
