using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public class SceneAIController : MonoBehaviour
    {
        [SerializeField] private SceneAISettings settings;
        private Dictionary<IDamageable, List<BaseEnemy>> enemiesOnItemDictionary = new Dictionary<IDamageable, List<BaseEnemy>>();

        private void OnEnable()
        {
            GameEvents.OnDamageableDie.AddListener(OnDamageableHealthZero);
            GameEvents.OnItemEnterEnemyRadius.AddListener(OnItemEnterEnemyRadius);
            GameEvents.OnItemExitEnemyRadius.AddListener(OnItemExitEnemyRadius);
            GameEvents.OnPlaceSelectedItem.AddListener(OnItemPlaced);
        }

        private void OnDisable()
        {
            GameEvents.OnDamageableDie.RemoveListener(OnDamageableHealthZero);
            GameEvents.OnItemEnterEnemyRadius.RemoveListener(OnItemEnterEnemyRadius);
            GameEvents.OnItemExitEnemyRadius.RemoveListener(OnItemExitEnemyRadius);
            GameEvents.OnPlaceSelectedItem.RemoveListener(OnItemPlaced);
        }

        private void OnDamageableHealthZero(IDamageable damageable)
        {
            if (damageable is BaseItem item)
            {
                if (enemiesOnItemDictionary.ContainsKey(damageable))
                {
                    // change state of all the enemies to 'moving'
                    foreach (var enemy in enemiesOnItemDictionary[damageable])
                    {
                        enemy.SetTarget(null);
                    }

                    // remove key from the dictionary
                    enemiesOnItemDictionary.Remove(damageable);
                }
            }
            else if (damageable is BaseEnemy diedEnemy) // if enemy has died, remove it from the building, let others attack
            {
                var dictionaryEntry = enemiesOnItemDictionary.FirstOrDefault(t => t.Value.Exists(y => y == diedEnemy));
                //Debug.Log($"dictionaryEntry: {dictionaryEntry}");
                if (dictionaryEntry.Key != null)
                {
                    enemiesOnItemDictionary[dictionaryEntry.Key].Remove(diedEnemy);
                }
            }
        }

        private void OnItemEnterEnemyRadius(IDamageable damageable, BaseEnemy enemy)
        {
            if (enemiesOnItemDictionary.ContainsKey(damageable))
            {
                if (enemiesOnItemDictionary[damageable].Count < settings.sameEnemiesPerBuilding)
                {
                    if (!enemiesOnItemDictionary[damageable].Contains(enemy))
                    {
                        enemiesOnItemDictionary[damageable].Add(enemy);
                        enemy.SetTarget(damageable);
                        enemy.SetState(EnemyStates.Attacking);
                    }
                }
            }
        }

        private void OnItemExitEnemyRadius(IDamageable damageable, BaseEnemy enemy)
        {
            if (enemiesOnItemDictionary.ContainsKey(damageable))
            {
                if (enemiesOnItemDictionary[damageable].Contains(enemy))
                {
                    enemiesOnItemDictionary[damageable].Remove(enemy);
                    enemy.SetTarget(null);
                }
            }
        }

        private void OnItemPlaced(BaseItem item)
        {
            if (item is IDamageable damageable)
            {
                if(!enemiesOnItemDictionary.ContainsKey(damageable))
                    enemiesOnItemDictionary.Add(damageable, new List<BaseEnemy>());
            }
        }
    }
}
