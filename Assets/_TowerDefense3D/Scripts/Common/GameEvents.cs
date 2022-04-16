using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class GameEvents
    {
        public static UnityEvent<PlaceableItemAttributes> OnPlaceableItemSelected = new UnityEvent<PlaceableItemAttributes>();
        public static UnityEvent<PlaceableItemAttributes> OnSelectedItemPlaced = new UnityEvent<PlaceableItemAttributes>();
    }
}
