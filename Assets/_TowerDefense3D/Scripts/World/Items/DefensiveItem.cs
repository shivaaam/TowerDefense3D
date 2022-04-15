using UnityEngine;

namespace TowerDefense3D
{
    public class DefensiveItem : BaseItem, IDamageable
    {
        public DefensiveItemAttributes itemAttributes;

        public virtual void TakeDamage(float damage)
        {
            
        }
    }
}
