using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class GameState : MonoBehaviour
    {
        public static GameData GetGameData => gameData;
        private static GameData gameData;

        public static GameData GetSaveableData()
        {
            return gameData;
        }

        public static void LoadSavedData(GameData l_data)
        {
            gameData = l_data;
        }
    }

    [System.Serializable]
    public class GameData
    {
        public PlayerData playerData;
        public int maxLevelsCleared;
    }
}
