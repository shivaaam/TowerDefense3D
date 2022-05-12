using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewEnemyWave", menuName = "Enemy Wave")]
    public class EnemyWaveData : ScriptableObject
    {
        public EnemyCountDictionary enemiesDictionary = new EnemyCountDictionary();
        public float spawnIntervalSameEnemies;
        public float spawnIntervalDifferentEnemies;
    }

    [System.Serializable]
    public class EnemyCountDictionary : SerializableDictionary<EnemyCategory, int> { }
}
