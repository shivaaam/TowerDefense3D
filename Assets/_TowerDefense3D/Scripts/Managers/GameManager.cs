using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense3D
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject levelObject;

        private void OnEnable()
        {
            GameEvents.OnGameSceneLoaded.AddListener(OnLevelLoaded);
        }

        private void OnDisable()
        {
            GameEvents.OnGameSceneLoaded.RemoveListener(OnLevelLoaded);
        }

        private void OnLevelLoaded(LevelData l_data)
        {

        }
    }
}
