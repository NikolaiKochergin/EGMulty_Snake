using UnityEngine;

namespace Source.Scripts
{
    public class CameraTracker : MonoBehaviour
    {
        public void Init(Transform trackedTransform, float offsetY)
        {
            transform.parent = trackedTransform;
            transform.localPosition = Vector3.up * offsetY;
        }
    }
}