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
            Pose seekOrientation  = AutonomousAgent.Seek(predictedPos, transform.position, transform.rotation, attributes.moveSpeed, maxSteeringForce);
            transform.rotation = seekOrientation.rotation;
            transform.Translate(transform.TransformDirection(seekOrientation.position) * Time.deltaTime);
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
