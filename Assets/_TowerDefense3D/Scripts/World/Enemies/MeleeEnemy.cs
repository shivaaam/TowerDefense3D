using UnityEngine;

namespace TowerDefense3D
{
    public abstract class MeleeEnemy : BaseEnemy, IDamageDealer
    {
        public MeleeEnemyAttributes Attributes => enemyAttributes as MeleeEnemyAttributes;

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {

        }

        public virtual void DealDamage(IDamageDealer damageDealer, IDamageable defender, int damage, Vector3 hitPoint)
        {

        }

        public virtual Transform GetDamageDealerTransform()
        {
            return transform;
        }

        public float LastAttackTime { get; set; }
    }
}
