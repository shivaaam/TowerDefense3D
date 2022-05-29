using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject touchControlsPanel;

        private void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            touchControlsPanel.SetActive(true);
#else
            touchControlsPanel.SetActive(false);
#endif
        }
    }
}
