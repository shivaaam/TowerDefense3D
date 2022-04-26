using UnityEngine;

namespace TowerDefense3D
{
    public abstract class MeleeEnemy : BaseEnemy, IDamageDealer
    {
        public MeleeEnemyAttributes enemyAttributes;

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {

        }

        public virtual void DealDamage(IDamageDealer damageDealer, IDamageable defender, float damage)
        {

        }

        public override void TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, enemyAttributes.maxHealth);
        }

        public virtual Transform GetDamageDealerTransform()
        {
            return transform;
        }
    }
}
