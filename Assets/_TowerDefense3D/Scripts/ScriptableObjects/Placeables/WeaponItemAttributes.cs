using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewWeaponItem", menuName = "Weapons/Weapon Item")]
    public class WeaponItemAttributes : PlaceableItemAttributes
    {
        public float attackRadius;
        public int maxHealth;
    }
}
