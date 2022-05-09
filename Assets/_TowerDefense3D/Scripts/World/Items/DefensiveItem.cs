using UnityEngine;

namespace TowerDefense3D
{
    public class DefensiveItem : BaseItem, IDamageable
    {
        public DefensiveItemAttributes itemAttributes;

        private int health;

        public override PlaceableItemAttributes GetItemAttributes()
        {
            return itemAttributes;
        }

        public override PlaceableItemType GetPlaceableItemType()
        {
            return itemAttributes.type;
        }

        public int GetCurrentDamageableHealth()
        {
            return health;
        }

        public virtual void TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, itemAttributes.maxHealth);
        }

        public virtual Transform GetDamageableTransform()
        {
            return transform;
        }
    }
}
