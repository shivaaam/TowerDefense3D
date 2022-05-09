using UnityEngine;

namespace TowerDefense3D
{
    public class EnemyAttributes : ScriptableObject
    {
        [Header("Path Follow settings")]
        public float movementSpeed;
        public float steerSpeed;
        public float lookAheadTime;
        [Header("Other settings")]
        public int maxHealth;
        public float attackRate;
        public float attackDamage;
    }
}