using UnityEngine;

namespace TowerDefense3D
{
    public interface IDamageDealer
    {
        public void Attack(IDamageDealer attacker, IDamageable defender);

        public void DealDamage(IDamageDealer damageDealer, IDamageable defender, int damage, Vector3 hitPoint);

        public Transform GetDamageDealerTransform();

        public float LastAttackTime { get; set; }
    }
}
