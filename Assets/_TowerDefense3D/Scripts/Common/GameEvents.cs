using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class GameEvents
    {
        public static UnityEvent<PlaceableItemAttributes> OnSelectPlaceableItem = new UnityEvent<PlaceableItemAttributes>();
        public static UnityEvent OnDeselectCurrentItem = new UnityEvent();
        public static UnityEvent<BaseItem> OnPlaceSelectedItem = new UnityEvent<BaseItem>();
        public static UnityEvent<IDamageable> OnDamageableDie = new UnityEvent<IDamageable>();

        public static UnityEvent<IDamageable, BaseEnemy> OnItemEnterEnemyRadius = new UnityEvent<IDamageable, BaseEnemy>();
        public static UnityEvent<IDamageable, BaseEnemy> OnItemExitEnemyRadius = new UnityEvent<IDamageable, BaseEnemy>();
    }
}
