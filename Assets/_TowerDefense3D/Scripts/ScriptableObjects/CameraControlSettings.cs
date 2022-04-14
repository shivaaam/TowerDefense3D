using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewCameraControlSettings", menuName = "Game Settings/Camera Control Settings")]
    public class CameraControlSettings : ScriptableObject
    {
        public float moveSpeed;
        public float rotateSpeed;
        public float zoomSpeed;
        public float movementDamping;
        public float rotationDamping;
    }
}
