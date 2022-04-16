using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense3D
{
    public class SelectItemButton : MonoBehaviour
    {
        public Button button;
        public Image iconImage;
        public Image cooldownTimerImage;
        public TextMeshProUGUI costText;

        private PlaceableItemAttributes attributes;
        private Coroutine timerCoroutine;

        private void OnEnable()
        {
            GameEvents.OnSelectedItemPlaced.AddListener(OnSelectedItemPlaced);
        }

        private void OnDisable()
        {
            GameEvents.OnSelectedItemPlaced.RemoveListener(OnSelectedItemPlaced);
        }

        public void SetButtonData(PlaceableItemAttributes l_att)
        {
            iconImage.sprite = l_att.icon;
            cooldownTimerImage.fillAmount = 1;
            costText.text = l_att.cost.ToString();
            attributes = l_att;
        }

        public void SelectItem()
        {
            GameEvents.OnPlaceableItemSelected?.Invoke(attributes);
        }

        private void ToggleButtonInteractable(bool isActive)
        {
            button.interactable = isActive;
        }

        private void SetCooldownTimer(float value)
        {
            cooldownTimerImage.fillAmount = value;
        }

        private void StartTimer()
        {
            if(timerCoroutine != null)
                StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(ButtonTimerCoroutine(attributes.cooldownTime));
        }

        private IEnumerator ButtonTimerCoroutine(float time)
        {
            float timeElapsed = 0;
            while (timeElapsed < time)
            {
                timeElapsed += Time.deltaTime;
                cooldownTimerImage.fillAmount = timeElapsed / time;
                yield return null;
            }
            ToggleButtonInteractable(true);
        }

        private void OnSelectedItemPlaced(PlaceableItemAttributes l_att)
        {
            if (l_att.id == attributes.id)
            {
                ToggleButtonInteractable(false);
                StartTimer();
            }
        }
    }
}
