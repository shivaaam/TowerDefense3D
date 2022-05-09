using UnityEngine;

namespace TowerDefense3D
{
    public abstract class WeaponItem : BaseItem, IDamageable
    {
        protected IDamageable target;
        protected int health;

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEvents.OnDamageableDie.AddListener(OnDamageableHealthZero);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvents.OnDamageableDie.AddListener(OnDamageableHealthZero);
        }

        protected virtual void OnDamageableHealthZero(IDamageable l_damageable)
        {
            if (target == l_damageable)
                target = null;
        }

        public virtual Transform GetDamageableTransform()
        {
            return transform;
        }

        public int GetCurrentDamageableHealth()
        {
            return health;
        }

        public virtual void TakeDamage(int damage)
        {
            
        }

        public virtual void Attack(IDamageable l_target)
        {

        }
    }
}
