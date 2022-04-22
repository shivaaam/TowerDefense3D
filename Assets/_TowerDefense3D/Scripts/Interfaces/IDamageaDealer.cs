using UnityEngine;

namespace TowerDefense3D
{
    public interface IDamageDealer
    {
        public void Attack(IDamageDealer attacker, IDamageable defender);

        public void DealDamage(IDamageable defender, float damage);

        public Transform GetDamageDealerTransform();
    }
}
