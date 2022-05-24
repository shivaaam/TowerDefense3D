using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewLevelWaves", menuName = "Levels/Level Waves")]
    public class LevelWavesData : ScriptableObject
    {
        public float spawnStartTime;
        public EnemyWaveData[] waves;
        public float spawnIntervalBetweenWaves;
    }
}
