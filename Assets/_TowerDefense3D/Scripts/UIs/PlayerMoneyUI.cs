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
            moneyText.text = l_newAmount.ToString();
        }

        private void OnGameLevelLoaded(int index, LevelData l_data)
        {
            OnUpdatePlayerMoney(GameState.GetGameData.playerData.money);
        }
    }
}
