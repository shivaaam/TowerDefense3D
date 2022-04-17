using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class GameEvents
    {
        public static UnityEvent<PlaceableItemAttributes> OnSelectPlaceableItem = new UnityEvent<PlaceableItemAttributes>();
        public static UnityEvent OnDeselectCurrentItem = new UnityEvent();

        public static UnityEvent<PlaceableItemAttributes> OnPlaceSelectedItem = new UnityEvent<PlaceableItemAttributes>();
    }
}
