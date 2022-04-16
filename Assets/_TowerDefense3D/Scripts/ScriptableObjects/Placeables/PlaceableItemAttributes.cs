using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    public class PlaceableItemAttributes : ScriptableObject
    {
        public int id;
        public AssetReferenceGameObject prefab;
        public Vector2 size;
        public int cost;
        public int cooldownTime;
        public Sprite icon;
        public float constructTime;
    }
}
