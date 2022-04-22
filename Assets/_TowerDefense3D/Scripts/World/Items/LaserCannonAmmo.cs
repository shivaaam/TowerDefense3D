using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class LaserCannonAmmo : BaseAmmo
    {
        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            base.Attack(attacker, defender);
        }

        public override void DealDamage(IDamageable defender, float damage)
        {
            base.DealDamage(defender, damage);
        }
    }
}
