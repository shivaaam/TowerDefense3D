using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class RangeWeapon : WeaponItem
    {
        public RangeWeaponAttributes itemAttributes;
        public AmmoAttributes ammo;

        private void Update()
        {
            if (target == null)
                return;

            // chase target and fire with given fire rate
        }

        public override PlaceableItemAttributes GetItemAttributes()
        {
            return itemAttributes;
        }

        public override PlaceableItemType GetPlaceableItemType()
        {
            return itemAttributes.type;
        }

        public override void TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, itemAttributes.maxHealth);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, 0.25f);
            Gizmos.DrawSphere(transform.position, itemAttributes.attackRadius);
        }
    }
}
