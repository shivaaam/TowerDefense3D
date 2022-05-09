using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeEnemy : BaseEnemy
    {
        public RangeEnemyAttributes Attributes => enemyAttributes as RangeEnemyAttributes;
    }
}
