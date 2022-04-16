using UnityEngine;
using UnityEngine.EventSystems;

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
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                isOnGraphics = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                //Debug.Log("IsMouseOverGraphics"+ IsMouseOverGraphics);
            }
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