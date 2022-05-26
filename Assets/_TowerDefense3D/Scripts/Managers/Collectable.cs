using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense3D
{
    public class Collectable : MonoBehaviour
    {
        public float selfDestructTime = 5f;
        public float selfDestructHeight = -200f;

        public int collectableAmount;
        private Camera raycastCam;

        private void Awake()
        {
            raycastCam = Camera.main;
        }

        private void OnEnable()
        {
            UserInputs.OnPerformActionInputEvent.AddListener(OnPerformActionInput);
        }

        private void OnDisable()
        {
            UserInputs.OnPerformActionInputEvent.RemoveListener(OnPerformActionInput);
        }

        private void Start()
        {
            StartSelfDestructTimer();
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
                Ray ray = raycastCam.ScreenPointToRay(new Vector3(UserInputs.inputData.mousePosition.x, UserInputs.inputData.mousePosition.y, 0));
                Physics.Raycast(ray, out RaycastHit hit, 200, 1 << gameObject.layer);
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    GameEvents.OnClickCollectable?.Invoke(this);
                    AddressableLoader.DestroyAndReleaseAddressable(gameObject);
                }
            }
        }

        private void StartSelfDestructTimer()
        {
            StartCoroutine(SelfDestructTimer(selfDestructTime));
        }

        private IEnumerator SelfDestructTimer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }
    }
}
