using UnityEngine;

namespace TowerDefense3D
{
    public class PlacementMarker : MonoBehaviour
    {
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
                    markerImage.transform.position = hit.point;
                }
            }
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
