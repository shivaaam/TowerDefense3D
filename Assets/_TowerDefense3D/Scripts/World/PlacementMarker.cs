using UnityEngine;

namespace TowerDefense3D
{
    public class PlacementMarker : MonoBehaviour
    {
        [SerializeField] private Camera currentCamera;
        public LayerMask groundLayer;

        private void Update()
        {
            Ray ray = currentCamera.ScreenPointToRay(new Vector3(UserInputs.inputData.mousePosition.x, UserInputs.inputData.mousePosition.y, 0));
            Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer);

            if (hit.collider != null)
            {
                Debug.Log($"Hitting {hit.collider.name}");
            }
        }
    }
}
