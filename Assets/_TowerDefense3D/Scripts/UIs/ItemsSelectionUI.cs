using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class ItemsSelectionUI : MonoBehaviour
    {
        public SelectItemButton sampleButtonObject;
        public Transform buttonsParent;

        [Header("Selected item")] 
        public GameObject selectedItemPanel;
        public UnityEngine.UI.Image selectedItemImage;

        [SerializeField] private List<PlaceableItemAttributes> availableItems;

        private void OnEnable()
        {
            GameEvents.OnSelectPlaceableItem.AddListener(OnItemSelect);
            GameEvents.OnDeselectCurrentItem.AddListener(OnItemDeselect);
        }

        private void OnDisable()
        {
            GameEvents.OnSelectPlaceableItem.RemoveListener(OnItemSelect);
            GameEvents.OnDeselectCurrentItem.RemoveListener(OnItemDeselect);
        }

        private void Start()
        {
            PopulateItemsUI(availableItems);
        }

        private void PopulateItemsUI(List<PlaceableItemAttributes> items)
        {
            if (items.Count <= 0)
                return;

            foreach (var item in items)
            {
                GameObject obj = Instantiate(sampleButtonObject.gameObject, buttonsParent);
                SelectItemButton buttonComponent = obj.GetComponent<SelectItemButton>();
                buttonComponent.SetButtonData(item);
                obj.SetActive(true);
            }
        }

        private void OnItemSelect(PlaceableItemAttributes item)
        {
            selectedItemImage.sprite = item.icon;
            ToggleSelectedItemPanel(true);
        }

        private void ToggleSelectedItemPanel(bool isActive)
        {
            selectedItemPanel.SetActive(isActive);
        }

        private void OnItemDeselect()
        {
            ToggleSelectedItemPanel(false);
        }
    }
}
