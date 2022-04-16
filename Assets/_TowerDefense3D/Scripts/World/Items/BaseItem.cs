using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseItem : MonoBehaviour, IPlaceable
    {
        public virtual PlaceableItemAttributes GetItemAttributes()
        {
            return null;
        }

        public virtual void Place(Vector2 coordinate)
        {

        }
    }
}
