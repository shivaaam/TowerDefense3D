using System.Collections;
using UnityEngine;
using TMPro;

namespace TowerDefense3D
{
    public class MoneyAddedAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private AnimationCurve animationEase = AnimationCurve.Linear(0, 0, 1, 1);

        private PlayerController player;
        private Coroutine animationCoroutine;
        private float yOffset = 142f;

        public void SetData(int l_amount)
        {
            gameObject.SetActive(true);
            amountText.text = l_amount > 0 ? $"+{Mathf.Abs(l_amount)}" : $"-{Mathf.Abs(l_amount)}";
            StartAnimation();
        }

        private void StartAnimation()
        {
            if(animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimationCoroutine(animationTime));
        }

        private IEnumerator AnimationCoroutine(float l_animationTime)
        {
            float timeElapsed = 0;
            Vector3 initialPosition = transform.position;
            Vector3 finalPosition = new Vector3(initialPosition.x, initialPosition.y + yOffset, initialPosition.z);
            float initialAlpha = 1f;
            while (timeElapsed < l_animationTime)
            {
                Vector3 currentPosition = Vector3.Lerp(initialPosition, finalPosition, animationEase.Evaluate(timeElapsed / animationTime));
                float currentAlpha = Mathf.Lerp(initialAlpha, 0, animationEase.Evaluate(timeElapsed / animationTime));
                transform.position = currentPosition;
                canvasGroup.alpha = currentAlpha;

                timeElapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
