using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class PlayerController : MonoBehaviour
    {
        private PlaceableItemAttributes currentSelectedItemAttributes;

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
            currentSelectedItemAttributes = attributes;
        }
    }
}
