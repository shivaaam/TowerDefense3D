using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class LoadingAnimator : MonoBehaviour
    {
        [SerializeField] private float frameTime = 0.25f;
        private UnityEngine.UI.Image loadingIcon;
        private Coroutine animtionCoroutine;

        void Start()
        {
            loadingIcon = GetComponent<UnityEngine.UI.Image>();
            StartCoroutine(AnimationCoroutine(30f, frameTime));
        }

        private IEnumerator AnimationCoroutine(float anglePerSpin, float frameTime = 0.25f)
        {
            while (true)
            {
                loadingIcon.transform.Rotate(0, 0, anglePerSpin);
                yield return new WaitForSeconds(frameTime);
            }
        }
    }
}
