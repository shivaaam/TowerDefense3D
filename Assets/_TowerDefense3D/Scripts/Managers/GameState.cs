using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class GameState : MonoBehaviour
    {
        public static GameData GetGameData => gameData;
        private static GameData gameData;

        public GameData GetSaveableData()
        {
            return gameData;
        }

        public void LoadSavedData(GameData l_data)
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
