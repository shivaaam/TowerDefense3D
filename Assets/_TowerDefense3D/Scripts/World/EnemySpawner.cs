using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    public class EnemySpawner : MonoBehaviour
    {
        public BoxCollider spawnArea;
        public LevelWavesData levelWaves;
        public EnemyTypePrefabDictionary enemyPrefabsDictionary = new EnemyTypePrefabDictionary();

        [Header("Enemy Paths")]
        public Path[] groundPaths;
        public Path[] aerialPaths;

        private Coroutine enemyWaveCoroutine;
        private Coroutine levelWavesCoroutine;

        public Transform SpawnParent 
        {
            get
            {
                if (spawnParentObj == null)
                {
                    GameObject obj = transform.Find("Enemies").gameObject;
                    if (obj == null)
                    {
                        obj = new GameObject("Enemies");
                        obj.transform.SetParent(transform);
                    }
                    spawnParentObj = obj;
                }
                return spawnParentObj.transform;
            }
        }
        private GameObject spawnParentObj;

        private void OnEnable()
        {
            GameEvents.OnGameSceneLoaded.AddListener(OnLevelLoaded);
        }

        private void OnDisable()
        {
            GameEvents.OnGameSceneLoaded.RemoveListener(OnLevelLoaded);
        }

        public void SpawnEnemy(EnemyCategory enemy, float pathFollowRandomFactor = 0.2f)
        {
            if (!enemyPrefabsDictionary.ContainsKey(enemy))
                return;

            var selectedAttributes = enemyPrefabsDictionary[enemy];
            GameObject enemyObj = AddressableLoader.InstantiateAddressable(selectedAttributes.prefab);
            enemyObj.transform.SetParent(SpawnParent);
            
            float spawnX = Random.Range(spawnArea.transform.position.x - spawnArea.size.x / 2, spawnArea.transform.position.x + spawnArea.size.x / 2);
            float spawnZ = Random.Range(spawnArea.transform.position.z - spawnArea.size.z / 2, spawnArea.transform.position.z + spawnArea.size.z / 2);
            Vector3 spawnPosition = new Vector3(spawnX, selectedAttributes.spawnHeight, spawnZ);

            enemyObj.transform.position = spawnPosition;

            BaseEnemy enemyComponent = enemyObj.GetComponent<BaseEnemy>();
            pathFollowRandomFactor = Mathf.Clamp(pathFollowRandomFactor, 0, 1);

            // get random path with enemy type
            Path[] selectedPathsList = selectedAttributes.type == EnemyType.Ground ? groundPaths : aerialPaths;
            int rngPathIndex = Random.Range(0, selectedPathsList.Length);

            // set path settings
            var followSettings = GetRandomPathFollowingSettings(selectedAttributes.pathFollowSettings, pathFollowRandomFactor);
            enemyComponent.SetPathFollowSettings(followSettings);
            enemyComponent.SetFollowPath(selectedPathsList[rngPathIndex]);

            // set evade settings
            enemyComponent.SetEvadeSettings(selectedAttributes.evadeSettings);
            enemyComponent.StartPathFollow(selectedPathsList[rngPathIndex], followSettings, selectedAttributes.evadeSettings);

            enemyComponent.SetHealth(selectedAttributes.maxHealth);
        }

        private PathFollowSettings GetRandomPathFollowingSettings(PathFollowSettings l_pathFollowSettings, float l_randomFactor)
        {
            PathFollowSettings newPathFollowSettings = new PathFollowSettings();

            float steerSpeedOffset = l_randomFactor * (l_pathFollowSettings.maxMoveSpeed - l_pathFollowSettings.minSteerSpeed);
            float minSteer = Mathf.Clamp(Random.Range(l_pathFollowSettings.minSteerSpeed - steerSpeedOffset, l_pathFollowSettings.minSteerSpeed + steerSpeedOffset), Constants.minEnemySteerSpeed, l_pathFollowSettings.maxSteerSpeed);
            newPathFollowSettings.minSteerSpeed = minSteer;

            float maxSteer = Mathf.Clamp(Random.Range(l_pathFollowSettings.maxSteerSpeed - steerSpeedOffset, l_pathFollowSettings.maxSteerSpeed + steerSpeedOffset), Constants.minEnemySteerSpeed, l_pathFollowSettings.maxSteerSpeed + 10f);
            newPathFollowSettings.maxSteerSpeed = maxSteer;

            float moveSpeedOffset = l_randomFactor * l_pathFollowSettings.maxMoveSpeed / 4f;
            float moveSpeed = Mathf.Clamp(Random.Range(l_pathFollowSettings.maxMoveSpeed - moveSpeedOffset, l_pathFollowSettings.maxMoveSpeed + moveSpeedOffset), Constants.minEnemyMoveSpeed, Constants.maxEnemyMoveSpeed);
            newPathFollowSettings.maxMoveSpeed = moveSpeed;

            float lookAheadOffset = l_randomFactor * l_pathFollowSettings.lookAheadTime / 2f;
            newPathFollowSettings.lookAheadTime = Mathf.Clamp(Random.Range(l_pathFollowSettings.lookAheadTime - lookAheadOffset, l_pathFollowSettings.lookAheadTime + lookAheadOffset), 0, Constants.maxEnemyLookAheadOffset);
            
            newPathFollowSettings.characterRadius = l_pathFollowSettings.characterRadius;

            return newPathFollowSettings;
        }

        public void SpawnEnemyWave(EnemyWaveData waveData)
        {
            if (enemyWaveCoroutine != null)
                StopCoroutine(enemyWaveCoroutine);
            enemyWaveCoroutine = StartCoroutine(EnemyWaveCoroutine(waveData));
        }

        private IEnumerator EnemyWaveCoroutine(EnemyWaveData waveData)
        {
            foreach (var entry in waveData.enemiesDictionary)
            {
                for (int i = 0; i < entry.Value; i++)
                {
                    SpawnEnemy(entry.Key);
                    yield return new WaitForSeconds(waveData.spawnIntervalSameEnemies);
                }
                yield return new WaitForSeconds(waveData.spawnIntervalDifferentEnemies);
            }
            //Debug.Log("enemy wave done spawning");
        }

        public void StartLevelWaves(LevelWavesData levelWavesData)
        {
            if(levelWavesCoroutine != null)
                StopCoroutine(levelWavesCoroutine);
            levelWavesCoroutine = StartCoroutine(LevelWavesCoroutine(levelWavesData));
        }

        private IEnumerator LevelWavesCoroutine(LevelWavesData wavesData)
        {
            // wait for a few seconds before starting to spawn enemies
            yield return new WaitForSeconds(wavesData.spawnStartTime);

            foreach (var wave in wavesData.waves)
            {
                SpawnEnemyWave(wave);
                float allSpawnTime = 0;
                foreach (var entry in wave.enemiesDictionary)
                {
                    allSpawnTime += (float)entry.Value * wave.spawnIntervalSameEnemies;
                }
                allSpawnTime += (wave.enemiesDictionary.Count - 1) * wave.spawnIntervalDifferentEnemies;
                allSpawnTime += wavesData.spawnIntervalBetweenWaves;
                yield return new WaitForSeconds(allSpawnTime);
            }
        }

        private void OnLevelLoaded(int index, LevelData l_data)
        {
            levelWaves = l_data.levelWaves;
            if (levelWaves != null)
            {
                StartLevelWaves(levelWaves);
            }

        }

#if UNITY_EDITOR
        [ContextMenu("Spawn Mummy")]
        public void SpawnKaroMummy()
        {
            SpawnEnemy(EnemyCategory.GroundMummy);
        }

        [ContextMenu("Spawn UFO")]
        public void SpawnKaroUFO()
        {
            SpawnEnemy(EnemyCategory.AerialUfoWithoutWeapons);
        }

        [ContextMenu("Spawn Spider")]
        public void SpawnKaroSpider()
        {
            SpawnEnemy(EnemyCategory.GroundSpider);
        }

        [ContextMenu("Spawn UFO Missile")]
        public void SpawnKaroUfoWithMissile()
        {
            SpawnEnemy(EnemyCategory.AerialUfoWithMissile);
        }
#endif
    }

    public enum EnemyCategory
    {
        GroundMummy,
        GroundSpider,
        AerialUfoWithoutWeapons,
        AerialUfoWithLaser,
        AerialUfoWithMissile
    }

    public enum EnemyType
    {
        Ground, 
        Aerial
    }

    [System.Serializable] public class EnemyTypePrefabDictionary : SerializableDictionary<EnemyCategory, EnemyAttributes> { }
}
