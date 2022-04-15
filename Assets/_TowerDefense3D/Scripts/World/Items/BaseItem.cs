using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseItem : MonoBehaviour, IPlaceable
    {
        public virtual void Place(Vector2 coordinate)
        {

        }
    }
}
