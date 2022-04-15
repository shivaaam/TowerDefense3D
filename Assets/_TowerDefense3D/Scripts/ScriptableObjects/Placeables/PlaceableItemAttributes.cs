using UnityEngine;

namespace TowerDefense3D
{
    public class PlaceableItemAttributes : ScriptableObject
    {
        public int cost;
        public int cooldownTime;
        public Sprite icon;
        public float constructTime;
    }
}
