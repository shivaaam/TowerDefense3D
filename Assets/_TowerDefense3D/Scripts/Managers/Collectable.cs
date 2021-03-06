using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense3D
{
    public class Collectable : MonoBehaviour
    {
        public float selfDestructHeight = -200f;

        public int collectableAmount;
        private Camera raycastCam;
        private Camera RaycastCam
        {
            get
            {
                if (raycastCam == null)
                    raycastCam = Camera.main;
                return raycastCam;
            }
        }

        private void Awake()
        {
            raycastCam = Camera.main;
        }

        private void OnEnable()
        {
            UserInputs.OnPerformActionInputEvent.AddListener(OnPerformActionInput);
#if UNITY_ANDROID || UNITY_IOS
            UserInputs.OnPrimaryTouchInputEvent.AddListener(OnPerformActionInput);
#endif
        }

        private void OnDisable()
        {
            UserInputs.OnPerformActionInputEvent.RemoveListener(OnPerformActionInput);
#if UNITY_ANDROID || UNITY_IOS
            UserInputs.OnPrimaryTouchInputEvent.RemoveListener(OnPerformActionInput);
#endif
        }

        private void Update()
        {
            if(transform.position.y < selfDestructHeight)
                AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }

        private void OnPerformActionInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
#if UNITY_ANDROID || UNITY_IOS
                Ray ray = RaycastCam.ScreenPointToRay(new Vector3(UserInputs.inputData.primaryTouchPosition.x, UserInputs.inputData.primaryTouchPosition.y, 0));
#else
                Ray ray = RaycastCam.ScreenPointToRay(new Vector3(UserInputs.inputData.mousePosition.x, UserInputs.inputData.mousePosition.y, 0));
#endif
                Physics.Raycast(ray, out RaycastHit hit, 200, 1 << gameObject.layer);
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    GameEvents.OnClickCollectable?.Invoke(this);
                    AddressableLoader.DestroyAndReleaseAddressable(gameObject);
                }
            }
        }
    }
}
