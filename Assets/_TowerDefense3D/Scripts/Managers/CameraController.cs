using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class CameraController : MonoBehaviour
    {
        public CameraControlSettings settings;
        public CinemachineVirtualCamera inGameCamera;
        public Transform cameraFollowTarget;

        private CinemachineTransposer inGameCameraTransposer;
        private CinemachineOrbitalTransposer inGameCameraOrbitalTransposer;
        private float currentCameraYawFactor;
        private float currentCameraPitchFactor;
        private float currentCameraZoomFactor;

        private Vector3 minFollowTargetPosition;
        private Vector3 maxFollowTargetPosition;

        private Vector2 lastMiddleMouseHoldPosition;

        private static UnityEvent<InitialCameraSetupSettings> SetInitialCameraSettingsEvent = new UnityEvent<InitialCameraSetupSettings>();

        private void Awake()
        {
            inGameCameraTransposer = inGameCamera.GetCinemachineComponent<CinemachineTransposer>();
            inGameCameraTransposer.m_XDamping = settings.movementDamping;
            inGameCameraTransposer.m_YDamping = settings.movementDamping;
            inGameCameraTransposer.m_ZDamping = settings.movementDamping;

            inGameCameraOrbitalTransposer = inGameCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }

        private void OnEnable()
        {
            SetInitialCameraSettingsEvent.AddListener(SetupInitialCamera);
        }

        private void OnDisable()
        {
            SetInitialCameraSettingsEvent.RemoveListener(SetupInitialCamera);
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

            if (IsFollowTargetInPanRange())
            {
                cameraFollowTarget.Translate(moveDirection.x * settings.moveSpeed * Time.deltaTime, 0, moveDirection.z * settings.moveSpeed * Time.deltaTime);
                float clampedX = Mathf.Clamp(cameraFollowTarget.position.x, minFollowTargetPosition.x, maxFollowTargetPosition.x);
                float clampedZ = Mathf.Clamp(cameraFollowTarget.position.z, minFollowTargetPosition.z, maxFollowTargetPosition.z);
                cameraFollowTarget.position = new Vector3(clampedX, cameraFollowTarget.position.y, clampedZ);
            }

        }

        public void RotateCamera(Vector2 rotateInput)
        {
            float rotationYaw = rotateInput.x * settings.rotateSpeed * Time.deltaTime;
            float rotationPitch = -rotateInput.y * settings.rotateSpeed * Time.deltaTime;

            currentCameraYawFactor = Mathf.Lerp(currentCameraYawFactor, rotationYaw, settings.rotationDamping);
            currentCameraPitchFactor = Mathf.Lerp(currentCameraPitchFactor, rotationPitch, settings.rotationDamping);

            Vector3 directionTowardsCamera = inGameCameraTransposer.m_FollowOffset;
            Vector3 directionTowardsCameraOnPlane = Vector3.ProjectOnPlane(directionTowardsCamera, Vector3.up); ;

            // Pitch
            float angle = Vector3.Angle(directionTowardsCameraOnPlane, directionTowardsCamera);
            angle += currentCameraPitchFactor;
            angle = Mathf.Clamp(angle, settings.minPitchAngle, settings.maxPitchAngle);
            inGameCameraTransposer.m_FollowOffset = new Vector3(0,
                directionTowardsCamera.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad),
                -directionTowardsCamera.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad));

            // Yaw
            if(inGameCameraOrbitalTransposer != null)
                inGameCameraOrbitalTransposer.m_Heading.m_Bias += (currentCameraYawFactor % 180f);
        }

        public void UpdateCameraZoom(float zoomInput)
        {
            float zoomFactor = zoomInput * settings.zoomSpeed * Time.deltaTime;
            currentCameraZoomFactor = Mathf.Lerp(currentCameraZoomFactor, zoomFactor, settings.zoomDamping);

            float finalZoom = Mathf.Clamp(inGameCameraTransposer.m_FollowOffset.z + currentCameraZoomFactor, settings.minZoom, settings.maxZoom);
            inGameCameraTransposer.m_FollowOffset = new Vector3(inGameCameraTransposer.m_FollowOffset.x, inGameCameraTransposer.m_FollowOffset.y, finalZoom);
        }

        public static void SetInitialCameraSettings(InitialCameraSetupSettings initialSetup)
        {
            SetInitialCameraSettingsEvent?.Invoke(initialSetup);
        }

        public void SetupInitialCamera(InitialCameraSetupSettings initialSetup)
        {
            cameraFollowTarget.position = initialSetup.targetPosition;
            inGameCameraTransposer.m_FollowOffset = initialSetup.cameraFollowOffset;
            inGameCameraOrbitalTransposer.m_Heading.m_Bias = initialSetup.cameraYawBias;

            minFollowTargetPosition = new Vector3(initialSetup.minCameraPanPosition.x, cameraFollowTarget.position.y, initialSetup.minCameraPanPosition.y);
            maxFollowTargetPosition = new Vector3(initialSetup.maxCameraPanPosition.x, cameraFollowTarget.position.y, initialSetup.maxCameraPanPosition.y);
        }

        private bool IsFollowTargetInPanRange()
        {
            return cameraFollowTarget.position.x >= minFollowTargetPosition.x && cameraFollowTarget.position.x <= maxFollowTargetPosition.x &&
                cameraFollowTarget.position.z >= minFollowTargetPosition.z && cameraFollowTarget.position.z <= maxFollowTargetPosition.z;
        }
    }
}
