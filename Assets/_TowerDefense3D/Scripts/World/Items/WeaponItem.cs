using UnityEngine;

namespace TowerDefense3D
{
    public abstract class WeaponItem : BaseItem, IDamageable
    {
        protected IDamageable target;
        protected int health;

        public virtual Transform GetDamageableTransform()
        {
            return transform;
        }

        public virtual void TakeDamage(int damage)
        {
            
        }

        public virtual void Attack(IDamageable l_target)
        {

        }
    }
}
