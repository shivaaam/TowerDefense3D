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

            Vector3 predictedPos = target.GetDamageableTransform().position + target.GetDamageableVelocity() * (attributes.targetTrackingLookAheadFactor > 0 ? attributes.targetTrackingLookAheadFactor : 1);
            Vector3 desired = (predictedPos - transform.position).normalized;

            transform.Translate(Vector3.forward * attributes.moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, Constants.minMissileHeight, Constants.maxMissileHeight), transform.position.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(desired), maxSteeringForce * Time.deltaTime);
        }

        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            base.Attack(attacker, defender);
            isSpawned = true;
        }

        public override void DealDamage(IDamageDealer damageDealer, IDamageable defender, int damage, Vector3 hitPoint)
        {
            base.DealDamage(damageDealer, defender, damage, hitPoint);
        }
    }
}
