using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewWeaponItem", menuName = "Placeable Items/Weapon Item")]
    public class WeaponItemAttributes : PlaceableItemAttributes
    {
        public float attackRadius;
        public int fireRate;
        public int damage;
        public int health;
        public float damageRadius;
    }
}
