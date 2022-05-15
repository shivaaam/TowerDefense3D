using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewSceneAISettings", menuName = "Enemies/Scene AI Settings")]
    public class SceneAISettings : ScriptableObject
    {
        public int sameEnemiesPerBuilding;
    }
}