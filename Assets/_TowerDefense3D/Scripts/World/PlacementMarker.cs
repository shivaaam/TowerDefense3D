using UnityEngine;

namespace TowerDefense3D
{
    public class PlacementMarker : MonoBehaviour
    {
        public LevelController levelController;
        [SerializeField] private Camera currentCamera;
        public UnityEngine.UI.Image markerImage;
        public LayerMask groundLayer;

        private PlaceableItemAttributes selectedItem;

        private void OnEnable()
        {
            GameEvents.OnSelectPlaceableItem.AddListener(OnSelectItem);
            GameEvents.OnDeselectCurrentItem.AddListener(OnCancelSelection);
        }

        private void OnDisable()
        {
            GameEvents.OnSelectPlaceableItem.RemoveListener(OnSelectItem);
            GameEvents.OnDeselectCurrentItem.RemoveListener(OnCancelSelection);
        }

        private void Start()
        {
            ToggleMarkerImage(false);
        }

        private void Update()
        {
            if (selectedItem != null)
            {
                Ray ray = currentCamera.ScreenPointToRay(new Vector3(UserInputs.inputData.mousePosition.x, UserInputs.inputData.mousePosition.y, 0));
                Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer);

                if (hit.collider != null)
                {
                    //markerImage.transform.position = hit.point;
                    GridCell cell = levelController.GetGridCellAtWorldPosition(hit.point);
                    Vector3 estimatedPos = levelController.GetGridCellWorldPosition(cell);
                    float x = selectedItem.size.x % 2 == 0 ? estimatedPos.x + (cell.size / 2f) : estimatedPos.x;
                    float z = selectedItem.size.y % 2 == 0 ? estimatedPos.z + (cell.size / 2f) : estimatedPos.z;

                    markerImage.transform.position = new Vector3(x, estimatedPos.y, z);
                }
            }
        }

        private bool IsPositionValidForPlacement(Vector3 l_worldPosition)
        {
            return true;
        }

        private void OnCancelSelection()
        {
            selectedItem = null;
            ToggleMarkerImage(false);
        }

        private void OnSelectItem(PlaceableItemAttributes l_att)
        {
            selectedItem = l_att;
            markerImage.rectTransform.sizeDelta = l_att.size;
            ToggleMarkerImage(true);
        }

        private void ToggleMarkerImage(bool isActive)
        {
            markerImage.enabled = isActive;
        }
    }
}
