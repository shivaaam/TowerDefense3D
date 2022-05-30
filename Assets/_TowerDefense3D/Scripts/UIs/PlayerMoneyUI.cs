using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerDefense3D
{
    public class PlayerMoneyUI : MonoBehaviour
    {
        public PlayerController player;
        public TextMeshProUGUI moneyText;
        public GameObject moneyAddedPrefab;

        public void OnEnable()
        {
            GameEvents.OnGameSceneLoaded.AddListener(OnGameLevelLoaded);
            if (player != null)
            {
                player.OnUpdateCollectedMoney.AddListener(OnUpdatePlayerMoney);
            }
        }

        public void OnDisable()
        {
            GameEvents.OnGameSceneLoaded.RemoveListener(OnGameLevelLoaded);
            if (player != null)
            {
                player.OnUpdateCollectedMoney.RemoveListener(OnUpdatePlayerMoney);
            }
        }

        private void OnUpdatePlayerMoney(int l_newAmount)
        {
            int prevValue = int.Parse(moneyText.text);
            moneyText.text = l_newAmount.ToString();
            SpawnMoneyAddedObject(l_newAmount - prevValue);
        }

        private void SpawnMoneyAddedObject(int l_amount)
        {
            GameObject obj = Instantiate(moneyAddedPrefab, transform);
            obj.transform.position = moneyAddedPrefab.transform.position;
            MoneyAddedAnimation animationScript = obj.GetComponent<MoneyAddedAnimation>();
            animationScript.SetData(l_amount);
        }

        private void OnGameLevelLoaded(int index, LevelData l_data)
        {
            OnUpdatePlayerMoney(GameState.GetGameData.playerData.money);
        }
    }
}
