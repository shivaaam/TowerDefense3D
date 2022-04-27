using UnityEngine;

namespace TowerDefense3D
{
    public class CannonAmmo : BaseAmmo
    {
        private bool isSpawned;
        [SerializeField] private float maxSteeringForce = 10f;


        private void Update()
        {
            if (!isSpawned)
                return;

            Vector3 desired = (target.GetDamageableTransform().position - transform.position).normalized;

            transform.Translate(Vector3.forward * attributes.moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(desired), maxSteeringForce * Time.deltaTime);
        }

        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            base.Attack(attacker, defender);
            isSpawned = true;
        }

        public override void DealDamage(IDamageDealer damageDealer, IDamageable defender, float damage)
        {
            base.DealDamage(damageDealer, defender, damage);
        }
    }
}
