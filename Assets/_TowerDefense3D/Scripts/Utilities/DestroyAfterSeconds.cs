using System.Collections;
using UnityEngine;

namespace TowerDefense3D
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float secondsToDestroy;

        private void Start()
        {
            StartCoroutine(StartDestructionCountdown(secondsToDestroy));
        }

        private IEnumerator StartDestructionCountdown(float waitTime)
        {
            float timeElapsed = 0;
            while (timeElapsed < waitTime)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }
    }
}
