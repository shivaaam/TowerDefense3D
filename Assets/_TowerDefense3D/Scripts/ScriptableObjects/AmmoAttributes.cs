using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewAmmoAttributes", menuName = "Weapons/Ammo Attributes")]
    public class AmmoAttributes : ScriptableObject
    {
        public AssetReferenceGameObject prefab;
        public int damage;
        public float damageRadius;
        public AnimationCurve damageDistanceCurve;
    }
}
