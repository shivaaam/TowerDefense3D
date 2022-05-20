using System.Collections;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseAmmo : MonoBehaviour, IDamageDealer
    {
        protected AudioSource audioSource;
        protected IDamageable target;
        protected AmmoAttributes attributes;
        private Coroutine selfDestructionTimerCoroutine;

        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {
            target = defender;

            // start self destruct timer
            if (selfDestructionTimerCoroutine != null)
                StopCoroutine(selfDestructionTimerCoroutine);
            selfDestructionTimerCoroutine = StartCoroutine(SelfDestructionTimer(attributes.maxLifetime));
        }

        public virtual void DealDamage(IDamageDealer damageDealer, IDamageable defender, int damage, Vector3 hitPoint)
        {
            defender.TakeDamage(Mathf.FloorToInt(damage), hitPoint);
        }

        public virtual Transform GetDamageDealerTransform()
        {
            return transform;
        }

        public float LastAttackTime { get; set; }

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
                    DealDamage(this, damageable, attributes.damage, l_coll.ClosestPoint(transform.position));
                    DealDamageInRadius(l_coll, attributes.damageRadius);
                    SpawnCollisionParticles(transform.position);
                    //Destroy(gameObject);
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
                                DealDamage(this, dmgObj, Mathf.CeilToInt(attributes.damage * attributes.damageDistanceCurve.Evaluate(distanceToCurrentDamageable / dmgRadius)), l_coll != null ? l_coll.ClosestPoint(transform.position) : transform.position);
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
            //Destroy(gameObject);
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }
    }
}
