using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseAmmo : MonoBehaviour, IDamageDealer
    {
        private AmmoAttributes attributes;

        public virtual void Attack(IDamageDealer attacker, IDamageable defender)
        {
        }

        public virtual void DealDamage(IDamageable defender, float damage)
        {
        }

        public virtual Transform GetDamageDealerTransform()
        {
            return transform;
        }

        public void SetAttributes(AmmoAttributes l_att)
        {
            attributes = l_att;
        }
    }
}
