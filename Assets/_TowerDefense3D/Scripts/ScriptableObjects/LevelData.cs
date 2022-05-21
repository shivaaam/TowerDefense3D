using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Levels/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string id;
        public string name;
        public Grid levelGrid;
        public InitialCameraSetupSettings initialCameraSettings;
        public LevelWavesData levelWaves;
        public Sprite thumbnail;
    }
}
