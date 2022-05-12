using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewLevelWaves", menuName = "Level Waves")]
    public class LevelWaves : ScriptableObject
    {
        public EnemyWaveData[] waves;
        public float spawnIntervalBetweenWaves;
    }
}
