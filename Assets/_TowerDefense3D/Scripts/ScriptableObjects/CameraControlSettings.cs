using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewCameraControlSettings", menuName = "Game Settings/Camera Control Settings")]
    public class CameraControlSettings : ScriptableObject
    {
        [Header("Movement")]
        public float primaryMovementMultiplier;
        public float moveSpeed;
        public float movementDamping;

        [Header("Rotation")]
        public float primaryRotationMultiplier;
        public float rotateSpeed;
        public float rotationDamping;

        [Header("Zoom")]
        public float zoomSpeed;
        public float zoomDamping;
        public float minZoom;
        public float maxZoom;
    }
}
