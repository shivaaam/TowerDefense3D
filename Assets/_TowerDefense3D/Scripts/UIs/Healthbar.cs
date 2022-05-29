using UnityEngine;

namespace TowerDefense3D
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image fillImage;
        [SerializeField] private Gradient healthGradient;

        private Transform camTransform;
        private CanvasGroup canvasGroup;

        private bool isDead;

        private void Start()
        {
            InitializeHealthbar();
        }

        private void Update()
        {
            if (camTransform == null || canvasGroup == null && !isDead)
                return;
            
            float distanceFromCam = Mathf.Clamp(Vector3.Distance(transform.position, camTransform.position), Constants.minHealthBarDistance, Constants.maxHealthBarDistance);
            canvasGroup.alpha = !isDead ? Mathf.Clamp(1 - ((distanceFromCam - Constants.minHealthBarDistance )/ (Constants.maxHealthBarDistance - Constants.minHealthBarDistance)), 0, 1) : 0f;
        }

        public void InitializeHealthbar()
        {
            camTransform = Camera.main.transform;
            canvasGroup = GetComponent<CanvasGroup>();
            UpdateHealth(1, 1);
        }

        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            float percent = (float)currentHealth / (float)maxHealth;
            fillImage.color = healthGradient.Evaluate(percent);
            fillImage.fillAmount = percent;

            isDead = percent <= 0;
        }
    }
}