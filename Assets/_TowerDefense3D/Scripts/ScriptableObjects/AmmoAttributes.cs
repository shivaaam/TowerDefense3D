using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewAmmoAttributes", menuName = "Weapons/Ammo Attributes")]
    public class AmmoAttributes : ScriptableObject
    {
        public AssetReferenceGameObject prefab;
        public float moveSpeed;
        public float targetTrackingLookAheadFactor;
        public int damage;
        public float damageRadius;
        public AnimationCurve damageDistanceCurve;
        public LayerMask damageLayer;
        public float maxLifetime;
        public AudioClip fireSound;
        public AudioClip collisionSound;
        public AssetReferenceGameObject collisionParticlesPrefab;
    }
}
