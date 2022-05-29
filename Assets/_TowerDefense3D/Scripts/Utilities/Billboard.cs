using UnityEngine;

namespace TowerDefense3D
{
    public class Billboard : MonoBehaviour
    {
        private Transform camTransform;
        private Quaternion originalRotation;

        private void Start()
        {
            camTransform = Camera.main.transform;
            originalRotation = transform.rotation;
        }

        private void Update()
        {
            transform.rotation = camTransform.rotation * originalRotation;
        }
    }
}
