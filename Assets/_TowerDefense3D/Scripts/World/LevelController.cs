using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class LevelController : MonoBehaviour
    {
        public Grid worldGrid;
        public PathEndTrigger pathEndTrigger;
        private BoxCollider boxCollider;
        private int levelIndex;
        private LevelData levelData;
        private int levelEnemiesKilled;
        private int totalEnemiesInLevel;
        private bool isLevelEnd;

        public InitialCameraSetupSettings initialCameraSetup;

        private void OnEnable()
        {
            if(pathEndTrigger != null)
                pathEndTrigger.OnEnterPathEndTrigger.AddListener(OnEnemyReachedPathEnd);
            GameEvents.OnGameSceneLoaded.AddListener(OnLevelLoaded);
            GameEvents.OnDamageableDie.AddListener(OnEnemyKilled);
            GameEvents.OnPLayerLIfeReachesZero.AddListener(OnPlayerLifeFinish);
        }

        private void OnDisable()
        {
            if (pathEndTrigger != null)
                pathEndTrigger.OnEnterPathEndTrigger.RemoveListener(OnEnemyReachedPathEnd);
            GameEvents.OnGameSceneLoaded.RemoveListener(OnLevelLoaded);
            GameEvents.OnDamageableDie.RemoveListener(OnEnemyKilled);
            GameEvents.OnPLayerLIfeReachesZero.RemoveListener(OnPlayerLifeFinish);
        }

        private void OnLevelLoaded(int index, LevelData l_data)
        {
            levelData = l_data;
            initialCameraSetup = l_data.initialCameraSettings;
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
                if (!isLevelEnd)
                {
                    isLevelEnd = true;
                    StartCoroutine(LevelClearedCoroutine(1f));
                }
            }
        }

        private IEnumerator LevelClearedCoroutine(float waitTIme)
        {
            yield return new WaitForSeconds(waitTIme);
            GameEvents.OnLevelCleared?.Invoke(levelData);
        }

        private void OnPlayerLifeFinish()
        {
            Debug.Log($"Level lost");
            if (!isLevelEnd)
            {
                isLevelEnd = true;
                GameEvents.OnLevelLost?.Invoke(levelData);
            }
        }

        private void OnEnemyReachedPathEnd(GameObject enemyObj)
        {
            BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
            if(enemy != null)
                GameEvents.OnEnemyReachedPathEnd?.Invoke(enemy);
        }

        public int GetMaxLevelsCleared()
        {
            return (levelIndex + 1) % Constants.maxLevelsCount;
        }

        private void OnDrawGizmosSelected()
        {
            worldGrid.DrawGrid(transform);
        }
        
    }
}
