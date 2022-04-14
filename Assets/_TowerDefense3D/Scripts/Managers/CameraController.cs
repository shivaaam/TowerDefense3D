using Cinemachine;
using UnityEngine;

namespace TowerDefense3D
{
    public class CameraController : MonoBehaviour
    {
        public CameraControlSettings settings;
        public CinemachineVirtualCamera inGameCamera;
        public Transform cameraFollowTarget;

        private CinemachineTransposer inGameCameraTransposer;
        private CinemachineOrbitalTransposer inGameCameraOrbitalTransposer;
        private float currentCameraRotationFactor;

        private void Awake()
        {
            inGameCameraTransposer = inGameCamera.GetCinemachineComponent<CinemachineTransposer>();
            inGameCameraTransposer.m_XDamping = settings.movementDamping;
            inGameCameraTransposer.m_YDamping = settings.movementDamping;
            inGameCameraTransposer.m_ZDamping = settings.movementDamping;

            inGameCameraOrbitalTransposer = inGameCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }

        private void Update()
        {
            if (UserInputs.inputData.middleMouseButtonHold)
            {
                // see if mouse position is changing
            }
            if(UserInputs.inputData.moveCameraSecondary != Vector2.zero)
                MoveCamera(UserInputs.inputData.moveCameraSecondary);
            //if (UserInputs.inputData.rotateCameraSecondary != Vector2.zero)
                RotateCamera(UserInputs.inputData.rotateCameraSecondary);
        }

        public void MoveCamera(Vector2 moveInput)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            Vector3 moveDirection = inGameCamera.transform.TransformDirection(inputDirection).normalized;
            cameraFollowTarget.Translate(moveDirection.x * settings.moveSpeed * Time.deltaTime, 0, moveDirection.z * settings.moveSpeed * Time.deltaTime);
        }

        public void RotateCamera(Vector2 rotateInput)
        {
            Vector3 inputDirection = new Vector3(rotateInput.x, 0, rotateInput.y);
            float rotationYaw = inputDirection.x * settings.rotateSpeed * Time.deltaTime;
            currentCameraRotationFactor = Mathf.Lerp(currentCameraRotationFactor, rotationYaw, settings.rotationDamping);
            float rotationPitch = 0;

            if(inGameCameraOrbitalTransposer != null)
                inGameCameraOrbitalTransposer.m_Heading.m_Bias += (currentCameraRotationFactor % 180f);
        }
    }
    
}
