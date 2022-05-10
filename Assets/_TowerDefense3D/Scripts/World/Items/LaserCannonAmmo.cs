using UnityEngine;

namespace TowerDefense3D
{
    public class LaserCannonAmmo : BaseAmmo
    {
        private bool isSpawned = false;

        private void Update()
        {
            if (!isSpawned)
                return;
            transform.Translate(Vector3.forward * attributes.moveSpeed * Time.deltaTime);
        }

        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            base.Attack(attacker, defender);
            isSpawned = true;
        }

        public override void DealDamage(IDamageDealer damageDealer, IDamageable defender, float damage, Vector3 hitPoint)
        {
            base.DealDamage(damageDealer, defender, damage, hitPoint);
        }
    }
}
