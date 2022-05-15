using UnityEngine;

namespace TowerDefense3D
{
    public abstract class WeaponItem : BaseItem, IDamageable
    {
        private WeaponItemAttributes Attributes => itemAttributes as WeaponItemAttributes;
        protected IDamageable target;
        protected int health;
        [SerializeField] protected Healthbar healthBar;

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

        public Vector3 GetDamageableVelocity()
        {
            return Vector3.zero;
        }

        public int GetCurrentDamageableHealth()
        {
            return health;
        }

        public virtual void TakeDamage(int damage, Vector3 hitPoint)
        {
            health = Mathf.Clamp(health - damage, 0, Attributes.maxHealth);
            if (healthBar != null)
                healthBar.UpdateHealth(health, Attributes.maxHealth);
        }

        public virtual void Attack(IDamageable l_target)
        {

        }
    }
}
