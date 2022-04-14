using System;
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
        private float currentCameraZoomFactor;

        private Vector2 lastMiddleMouseHoldPosition;
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
            // move using primary controls
            Vector2 mousePositionDifference = lastMiddleMouseHoldPosition - UserInputs.inputData.mousePosition;
            lastMiddleMouseHoldPosition = UserInputs.inputData.mousePosition;

            if (UserInputs.inputData.middleMouseButtonHold)
            {
                MoveCamera(mousePositionDifference * settings.primaryMovementMultiplier);
            }

            // move using secondary controls
            if (UserInputs.inputData.moveCameraSecondary != Vector2.zero)
                MoveCamera(UserInputs.inputData.moveCameraSecondary);

            // rotate using primary controls
            if (UserInputs.inputData.mouseRightClickHold)
            {
                RotateCamera(mousePositionDifference * settings.primaryRotationMultiplier);
            }
            
            // rotate using secondary controls
            RotateCamera(UserInputs.inputData.rotateCameraSecondary);
            
            UpdateCameraZoom(UserInputs.inputData.zoomCamera);
        }

        public void MoveCamera(Vector2 moveInput)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            Vector3 moveDirection = inGameCamera.transform.TransformDirection(inputDirection);
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

        public void UpdateCameraZoom(float zoomInput)
        {
            float zoomFactor = zoomInput * settings.zoomSpeed * Time.deltaTime;
            currentCameraZoomFactor = Mathf.Lerp(currentCameraZoomFactor, zoomFactor, settings.zoomDamping);

            float finalZoom = Mathf.Clamp(inGameCameraTransposer.m_FollowOffset.z + currentCameraZoomFactor, settings.minZoom, settings.maxZoom);
            inGameCameraTransposer.m_FollowOffset = new Vector3(inGameCameraTransposer.m_FollowOffset.x, inGameCameraTransposer.m_FollowOffset.y, finalZoom);
        }
    }
}
