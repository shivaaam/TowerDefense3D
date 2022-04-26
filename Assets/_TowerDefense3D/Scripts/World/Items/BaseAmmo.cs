using System.Collections;
using nStation;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseAmmo : MonoBehaviour, IDamageDealer
    {
        protected IDamageable target;
        protected AmmoAttributes attributes;
        private Coroutine selfDestructionTimerCoroutine;

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {
            // TODO: Play fire sound
            target = defender;

            // start self destruct timer
            if (selfDestructionTimerCoroutine != null)
                StopCoroutine(selfDestructionTimerCoroutine);
            selfDestructionTimerCoroutine = StartCoroutine(SelfDestructionTimer(attributes.maxLifetime));
        }

        public virtual void DealDamage(IDamageDealer damageDealer, IDamageable defender, float damage)
        {
            defender.TakeDamage(Mathf.FloorToInt(damage));
        }

        public virtual Transform GetDamageDealerTransform()
        {
            return transform;
        }

        public void SetAttributes(AmmoAttributes l_att)
        {
            attributes = l_att;
        }

        private void OnTriggerEnter(Collider l_coll)
        {
            if ((1 << l_coll.gameObject.layer | attributes.damageLayer) == attributes.damageLayer)
            {
                IDamageable damageable = l_coll.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    // deal full damage to the collided object
                    DealDamage(this, damageable, attributes.damage);
                    DealDamageInRadius(l_coll, attributes.damageRadius);
                    SpawnCollisionParticles(transform.position);
                    AddressableLoader.DestroyAndReleaseAddressable(gameObject);
                }

            }
        }

        private void DealDamageInRadius(Collider l_coll, float dmgRadius)
        {
            if (attributes.damageRadius > 0)
            {
                Collider[] colls = Physics.OverlapSphere(GetDamageDealerTransform().position, dmgRadius, attributes.damageLayer);
                if (colls.Length > 0)
                {
                    foreach (Collider coll in colls)
                    {
                        if (coll != l_coll)
                        {
                            IDamageable dmgObj = coll.GetComponent<IDamageable>();
                            if (dmgObj != null)
                            {
                                float distanceToCurrentDamageable = Vector3.Distance(GetDamageDealerTransform().position, dmgObj.GetDamageableTransform().position);
                                DealDamage(this, dmgObj, attributes.damage * attributes.damageDistanceCurve.Evaluate(distanceToCurrentDamageable / dmgRadius));
                            }
                        }
                    }
                }
            }
        }

        private void SpawnCollisionParticles(Vector3 position)
        {
            // TODO: Spawn collision particles
            // TODO: Play collision sound on the particle object
        }

        private IEnumerator SelfDestructionTimer(float time)
        {
            yield return new WaitForSeconds(time); 
            DealDamageInRadius(null, attributes.damageRadius);
            SpawnCollisionParticles(transform.position);
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);

        }
    }
}
