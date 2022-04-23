using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeEnemy : BaseEnemy
    {
        public RangeEnemyAttributes enemyAttributes;

        public override void TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, enemyAttributes.maxHealth);
        }
    }
}
