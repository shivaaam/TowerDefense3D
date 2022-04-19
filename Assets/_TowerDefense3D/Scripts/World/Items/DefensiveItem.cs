using UnityEngine;

namespace TowerDefense3D
{
    public class DefensiveItem : BaseItem, IDamageable
    {
        public DefensiveItemAttributes itemAttributes;

        public override PlaceableItemAttributes GetItemAttributes()
        {
            return itemAttributes;
        }

        public override PlaceableItemType GetPlaceableItemType()
        {
            return itemAttributes.type;
        }

        public virtual void TakeDamage(float damage)
        {
            
        }
    }
}
