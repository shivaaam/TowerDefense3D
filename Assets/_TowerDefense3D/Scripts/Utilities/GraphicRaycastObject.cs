using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TowerDefense3D
{
    public class GraphicRaycastObject
    {
        public static bool IsMouseOverGraphics => IsMouseOnGraphics();

        private static bool IsMouseOnGraphics()
        {
            bool isOnGraphics = false;

            if (EventSystem.current == null)
            {
                return false;
            }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            isOnGraphics = EventSystem.current.IsPointerOverGameObject();
#elif UNITY_IOS || UNITY_ANDROID
            if (Touchscreen.current.touches.Count > 0)
        {
            isOnGraphics = EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue());
            //Debug.Log($"is mouse over graphics: {isOnGraphics}");
        }
        else
            isOnGraphics = false;
#else
            isOnGraphics = false;
#endif
            return isOnGraphics;
        }
    }
}