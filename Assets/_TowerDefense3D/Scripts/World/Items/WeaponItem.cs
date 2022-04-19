using UnityEngine;

namespace TowerDefense3D
{
    public class WeaponItem : BaseItem, IDamageable, IDamageDealer
    {
        public WeaponItemAttributes itemAttributes;

        public override PlaceableItemAttributes GetItemAttributes()
        {
            return itemAttributes;
        }

        public override PlaceableItemType GetPlaceableItemType()
        {
            return itemAttributes.type;
        }

        public virtual void TakeDamage(float damage)
        {
            
        }

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {
        }

        public virtual void DealDamage(IDamageable defender, float damage)
        {
            
        }
    }
}
