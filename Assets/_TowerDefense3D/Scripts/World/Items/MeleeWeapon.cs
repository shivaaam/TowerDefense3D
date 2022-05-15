using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class MeleeWeapon : WeaponItem
    {
        public MeleeWeaponAttributes Attributes => itemAttributes as MeleeWeaponAttributes; 
    }
}
