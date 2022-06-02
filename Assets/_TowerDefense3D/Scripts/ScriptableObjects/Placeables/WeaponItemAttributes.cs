using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    public class WeaponItemAttributes : PlaceableItemAttributes
    {
        public AssetReferenceGameObject particlesOnDestroyPrefab;
        public float lookAheadFactor;
        public float attackRadius;
        public int maxHealth;
    }
}
