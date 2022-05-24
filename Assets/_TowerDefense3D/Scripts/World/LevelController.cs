using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class LevelController : MonoBehaviour
    {
        public Grid worldGrid;
        private BoxCollider boxCollider;
        private LevelData levelData;
        private int levelEnemiesKilled;
        private int totalEnemiesInLevel;

        public InitialCameraSetupSettings initialCameraSetup;

        private void OnEnable()
        {
            GameEvents.OnGameSceneLoaded.AddListener(OnLevelLoaded);
            GameEvents.OnDamageableDie.AddListener(OnEnemyKilled);
            GameEvents.OnPLayerLIfeReachesZero.AddListener(OnPlayerLifeFinish);
        }

        private void OnDisable()
        {
            GameEvents.OnGameSceneLoaded.RemoveListener(OnLevelLoaded);
            GameEvents.OnDamageableDie.RemoveListener(OnEnemyKilled);
            GameEvents.OnPLayerLIfeReachesZero.RemoveListener(OnPlayerLifeFinish);
        }

        private void OnLevelLoaded(LevelData data)
        {
            levelData = data;
            initialCameraSetup = data.initialCameraSettings;
            if(initialCameraSetup != null)
                SetCameraInitialView();
        }

        void Start()
        {
            worldGrid = new Grid(worldGrid.rows, worldGrid.columns, worldGrid.cellSize, worldGrid.origin);

            boxCollider = GetComponent<BoxCollider>();
            UpdateBoxColliderSize();
        }

        public GridCell GetGridCellAtWorldPosition(Vector3 l_pos)
        {
            return worldGrid.GetGridCellAtWorldPoint(l_pos, transform);
        }

        public Vector3 GetGridCellWorldPosition(GridCell cell)
        {
            Vector3 worldPos = worldGrid.GetGridCellWorldPosition(cell, new Vector2(cell.size / 2f, cell.size / 2f), transform);
            worldPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);
            return worldPos;
        }

        private void UpdateBoxColliderSize()
        {
            if (GetComponent<MeshCombiner2>() == null)
            {
                boxCollider.enabled = false;
                return;
            }

            if (boxCollider == null)
                return;

            float x = worldGrid.columns * worldGrid.cellSize;
            float z = worldGrid.rows * worldGrid.cellSize;

            boxCollider.size = new Vector3(x, 0.2f, z);
            boxCollider.center = boxCollider.size / 2f;
        }

        private void SetCameraInitialView()
        {
            CameraController.SetInitialCameraSettings(initialCameraSetup);
        }

        public void RecenterCamera()
        {
            SetCameraInitialView();
        }

        private int GetTotalLevelEnemiesCount()
        {
            int total = totalEnemiesInLevel;
            if (total == 0)
            {
                foreach (var enemyWave in levelData.levelWaves.waves)
                {
                    foreach (var entry in enemyWave.enemiesDictionary)
                    {
                        total += entry.Value;
                    }
                }
                totalEnemiesInLevel = total;
            }
            return total;
        }

        private void OnEnemyKilled(IDamageable damageable)
        {
            if (damageable is BaseEnemy enemy)
            {
                levelEnemiesKilled++;
            }

            if (levelEnemiesKilled >= GetTotalLevelEnemiesCount())
            {
                Debug.Log($"Level cleared");
                GameEvents.OnLevelCleared?.Invoke(levelData);
            }
        }

        private void OnPlayerLifeFinish()
        {
            Debug.Log($"Level lost");
            GameEvents.OnLevelLost?.Invoke(levelData);
        }

        private void OnDrawGizmosSelected()
        {
            worldGrid.DrawGrid(transform);
        }
        
    }
}
