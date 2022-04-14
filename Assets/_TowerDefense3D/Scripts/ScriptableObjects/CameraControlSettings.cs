using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewCameraControlSettings", menuName = "Game Settings/Camera Control Settings")]
    public class CameraControlSettings : ScriptableObject
    {
        public float moveSpeed;
        public float rotateSpeed;
        public float movementDamping;
        public float rotationDamping;
        [Header("Zoom")]
        public float zoomSpeed;
        public float zoomDamping;
        public float minZoom;
        public float maxZoom;
    }
}
