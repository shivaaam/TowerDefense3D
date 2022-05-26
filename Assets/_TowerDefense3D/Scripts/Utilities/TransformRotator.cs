using System.Collections;
using UnityEngine;

namespace TowerDefense3D
{
    public class TransformRotator : MonoBehaviour
    {
        public float rotationSpeed = 10f;
        private Coroutine rotationCoroutine;

        private void Start()
        {
            StartRotation();
        }

        public void StartRotation()
        {
            StopRotation();
            rotationCoroutine = StartCoroutine(RotationCoroutine(rotationSpeed));
        }

        private IEnumerator RotationCoroutine(float l_rotationSpeed)
        {
            while (true)
            {
                transform.Rotate(Vector3.up, l_rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        public void StopRotation()
        {
            if (rotationCoroutine != null)
                StopCoroutine(rotationCoroutine);
        }
    }
}
