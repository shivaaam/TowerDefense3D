using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class GameState : MonoBehaviour
    {
        public static string SaveGamePath;
        public static GameData GetGameData => gameData;
        private static GameData gameData;

        public static GameData GetSaveableData()
        {
            return gameData;
        }

        public static void LoadData(GameData l_data)
        {
            gameData = l_data;
        }

        public static void SaveData()
        {
            SaveData(gameData);
        }

        public static void SaveData(GameData l_data)
        {
            string dataToSave = JsonUtility.ToJson(l_data);
            System.IO.File.WriteAllText(SaveGamePath, dataToSave);
            gameData = l_data;
        }

        public static void LoadSavedData()
        {
            GameData saveData = new GameData();
            if (System.IO.File.Exists(SaveGamePath))
            {
                saveData = JsonUtility.FromJson<GameData>(System.IO.File.ReadAllText(SaveGamePath));
            }
            LoadData(saveData);
        }

        public static void UpdateLevelsCleared(int l_level)
        {
            gameData.maxLevelsCleared = l_level;
        }
        public static void UpdatePlayerMoney(int l_amount)
        {
            gameData.playerData.money = l_amount;
        }
    }

    [System.Serializable]
    public class GameData
    {
        public PlayerData playerData;
        public int maxLevelsCleared;

        public GameData()
        {
            playerData = new PlayerData();
            maxLevelsCleared = 0;
        }
    }
}
