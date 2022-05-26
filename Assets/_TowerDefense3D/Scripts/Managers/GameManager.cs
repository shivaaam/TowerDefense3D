using UnityEngine;

namespace TowerDefense3D
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelController levelController;
        [SerializeField] private PlayerController player;

        private void OnEnable()
        {
            GameEvents.OnLevelCleared.AddListener(OnLevelCleared);
        }

        private void OnDisable()
        {
            GameEvents.OnLevelCleared.RemoveListener(OnLevelCleared);
        }

        private void OnLevelCleared(LevelData l_data)
        {
            //levelController.Update
        }
    }
}
