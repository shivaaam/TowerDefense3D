using UnityEngine;

namespace TowerDefense3D
{
    public interface IPlaceable
    {
        public PlaceableItemAttributes GetItemAttributes();

        /// <summary>
        /// Place the item on given grid coordinate
        /// </summary>
        /// <param name="coordinate"></param>
        public void Place(Vector2 coordinate);
    }
}
