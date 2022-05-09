using UnityEngine;

namespace TowerDefense3D
{
    public interface IDamageable
    {
        public int GetCurrentDamageableHealth();

        public void TakeDamage(int damage);

        public Transform GetDamageableTransform();
    }
}
