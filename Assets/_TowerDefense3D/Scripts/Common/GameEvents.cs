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

        public static UnityEvent<LevelData> OnClickLevelButton = new UnityEvent<LevelData>();
        
        /// <summary>
        /// int -> index of level
        /// LevelData -> Data of loaded level
        /// </summary>
        public static UnityEvent<int, LevelData> OnGameSceneLoaded = new UnityEvent<int, LevelData>();

        public static UnityEvent<LevelData> OnLevelCleared = new UnityEvent<LevelData>();
        public static UnityEvent<LevelData> OnLevelLost = new UnityEvent<LevelData>();

        public static UnityEvent<BaseEnemy> OnEnemyReachedPathEnd = new UnityEvent<BaseEnemy>();
        public static UnityEvent OnPLayerLIfeReachesZero = new UnityEvent();

        public static UnityEvent<Collectable> OnClickCollectable = new UnityEvent<Collectable>();
    }
}
