using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class ItemsSelectionUI : MonoBehaviour
    {
        public SelectItemButton sampleButtonObject;
        public Transform buttonsParent;
        [SerializeField] private List<PlaceableItemAttributes> availableItems;

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
    }
}
