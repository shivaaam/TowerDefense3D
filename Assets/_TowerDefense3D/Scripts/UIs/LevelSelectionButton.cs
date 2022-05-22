using TMPro;
using UnityEngine;

namespace TowerDefense3D
{
    public class LevelSelectionButton : MonoBehaviour
    {
        private LevelData data;
        public TextMeshProUGUI levelName;
        public UnityEngine.UI.Image thumbnail;
        public GameObject levelLockedPanel;

        public void SetButtonData(LevelSelectionButtonUIData l_data)
        {
            data = l_data.levelData;
            levelName.text = l_data.levelName;
            thumbnail.sprite = l_data.thumbnailSprite;
            SetLockState(l_data.isLocked);
        }

        public void SetLockState(bool isLocked)
        {
            levelLockedPanel.SetActive(isLocked);
        }

        public void OnClick()
        {
            GameEvents.OnClickLevelButton?.Invoke(data);
        }
    }

    [System.Serializable] 
    public struct LevelSelectionButtonUIData
    {
        public LevelData levelData;
        public string levelName;
        public Sprite thumbnailSprite;
        public bool isLocked;
    }
}
