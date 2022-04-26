using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class CannonAmmo : BaseAmmo
    {
        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            Debug.Log("Move bullet towards target");
            base.Attack(attacker, defender);
        }

        public override void DealDamage(IDamageDealer damageDealer, IDamageable defender, float damage)
        {
            base.DealDamage(damageDealer, defender, damage);
        }
    }
}
