using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    public class EnemyAttributes : ScriptableObject
    {
        public EnemyCategory category;
        public EnemyType type;
        public AssetReferenceGameObject prefab;
        public AssetReferenceGameObject particlesOnDeadPrefab;
        public float spawnHeight;
        public PathFollowSettings pathFollowSettings;
        public EvadeSettings evadeSettings;
        [Header("Other settings")]
        public int maxHealth;

        public float perceptionRadius;
        public float attackRate;
        public int attackDamage;
        public float attackDistance;
        public int killReward;
        public int specialReward;
        public float specialRewardProbability;
        public AssetReferenceGameObject specialRewardPrefab;
    }
}