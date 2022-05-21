using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeEnemy : BaseEnemy, IDamageDealer
    {
        public RangeEnemyAttributes Attributes => enemyAttributes as RangeEnemyAttributes;

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {
            LastAttackTime = Time.time;
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
