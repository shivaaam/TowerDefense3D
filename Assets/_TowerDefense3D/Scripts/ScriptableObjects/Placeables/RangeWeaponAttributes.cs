using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewRangeWeapon", menuName = "Weapons/Range Weapon")]
    public class RangeWeaponAttributes : WeaponItemAttributes
    {
        public int fireRate;
    }
}
