using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class PlayerLivesUI : MonoBehaviour
    {
        public PlayerController player;

        [Header("UI references")] 
        public Transform iconsParent;
        public GameObject heartIconPrefab;

        private int currentLives;
        private List<GameObject> spawnedLlifeIcons = new List<GameObject>();

        public void OnEnable()
        {
            GameEvents.OnGameSceneLoaded.AddListener(OnGameLevelLoaded);
            if (player != null)
            {
                player.OnUpdatePlayerLives.AddListener(OnUpdatePlayerLives);
            }
        }

        public void OnDisable()
        {
            GameEvents.OnGameSceneLoaded.RemoveListener(OnGameLevelLoaded);
            if (player != null)
            {
                player.OnUpdatePlayerLives.RemoveListener(OnUpdatePlayerLives);
            }
        }

        private void OnUpdatePlayerLives(int l_newAmount)
        {
            currentLives = l_newAmount;

            // update number of lives in UI
            foreach (var iconObject in spawnedLlifeIcons)
            {
                Destroy(iconObject);
            }
            spawnedLlifeIcons.Clear();
            SpawnLifeIcons(l_newAmount);
        }

        private void SpawnLifeIcons(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject iconObj = Instantiate(heartIconPrefab, iconsParent);
                if (!spawnedLlifeIcons.Contains(iconObj))
                    spawnedLlifeIcons.Add(iconObj);
            }
        }

        private void OnGameLevelLoaded(LevelData l_data)
        {
            OnUpdatePlayerLives(Constants.startPlayerLives);
        }
    }
}
