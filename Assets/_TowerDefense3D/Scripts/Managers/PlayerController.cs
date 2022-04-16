using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerData playerData;
        private IPlaceable currentSelectedItem;

        private void OnEnable()
        {
            GameEvents.OnPlaceableItemSelected.AddListener(OnItemSelectedToPlace);
        }

        private void OnDisable()
        {
            GameEvents.OnPlaceableItemSelected.RemoveListener(OnItemSelectedToPlace);
        }

        private void OnItemSelectedToPlace(PlaceableItemAttributes attributes)
        {
            // instantiate the item with matching attributes
            // cache item in currentSelectedItem
        }

        public void PlaceSelectedItem()
        {
            if (currentSelectedItem != null)
            {
                currentSelectedItem.Place(new Vector2(0, 0)); // spawn at current selected grid cell
                GameEvents.OnSelectedItemPlaced?.Invoke(currentSelectedItem.GetItemAttributes());
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public int money;
    }
}
