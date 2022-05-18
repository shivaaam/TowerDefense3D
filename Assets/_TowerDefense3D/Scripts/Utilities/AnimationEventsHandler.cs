using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class AnimationEventsHandler : MonoBehaviour
    {
        public UnityEvent<string> animationUnityEvent = new UnityEvent<string>();

        public void OnAnimationEvent(string eventName)
        {
            animationUnityEvent?.Invoke(eventName);
        }
    }
}
