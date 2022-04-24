using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewRangeWeapon", menuName = "Weapons/Range Weapon")]
    public class RangeWeaponAttributes : WeaponItemAttributes
    {
        public AmmoAttributes ammo;
        public int fireRate;
        
        [Header("Rotation")]
        public float trackTargetSpeed;
        public Vector2 rotationAxis;
    }
}
