using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewInitialCameraSetup", menuName = "Initial Camera Setup")]
    public class InitialCameraSetupSettings : ScriptableObject
    {
        public Vector3 targetPosition;
        public Vector3 cameraFollowOffset;
        public float cameraYawBias;
        public Vector2 minCameraPanPosition;
        public Vector2 maxCameraPanPosition;
    }
}
