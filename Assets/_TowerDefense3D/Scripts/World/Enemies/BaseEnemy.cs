using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseEnemy : MonoBehaviour, IDamageable
    {
        protected int health;

        public virtual void TakeDamage(int damage)
        {

        }

        public Transform GetDamageableTransform()
        {
            return transform;
        }
    }
}
