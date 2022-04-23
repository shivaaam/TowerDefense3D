using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewRangeEnemy", menuName = "Enemies/Range Enemy")]
    public class RangeEnemyAttributes : EnemyAttributes
    {
        public float attackRadius;
    }
}