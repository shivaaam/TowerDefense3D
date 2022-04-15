using UnityEngine;

namespace TowerDefense3D
{
    public class WeaponItem : BaseItem, IDamageable, IDamageDealer
    {
        public WeaponItemAttributes itemAttributes;

        public virtual void TakeDamage(float damage)
        {
            
        }

        public virtual void DealDamage(IDamageable defender, float damage)
        {
            
        }
    }
}
