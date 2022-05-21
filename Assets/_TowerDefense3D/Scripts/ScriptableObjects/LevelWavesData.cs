using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewLevelWaves", menuName = "Level Waves")]
    public class LevelWavesData : ScriptableObject
    {
        public EnemyWaveData[] waves;
        public float spawnIntervalBetweenWaves;
    }
}
