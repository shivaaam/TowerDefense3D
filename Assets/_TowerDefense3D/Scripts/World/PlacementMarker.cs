using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public class PlacementMarker : MonoBehaviour
    {
        public LevelController levelController;
        [SerializeField] private Camera currentCamera;
        [SerializeField] private UnityEngine.UI.Image markerImage;

        [Header("Colors")] 
        public Color validColor;
        public Color invalidColor;
        
        [Header("Layers")]
        public LayerMask groundLayer;
        public LayerMask itemsLayer;
        public LayerMask placementValidityCheckLayer;

        private Vector3 previousValidTouchPosition;

        private PlaceableItemAttributes selectedItem;
        public bool IsCurrentPositionValid { get; private set; }
        public Transform Marker => markerImage.transform;

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
            previousValidTouchPosition = Vector3.zero;
        }

        private void Update()
        {
            if (selectedItem != null)
            {
                if (GraphicRaycastObject.IsMouseOverGraphics)
                {
                    return;
                }
#if UNITY_ANDROID || UNITY_IOS
                //Vector3 touchPos = GraphicRaycastObject.IsMouseOverGraphics ? previousValidTouchPosition : new Vector3(UserInputs.inputData.primaryTouchPosition.x, UserInputs.inputData.primaryTouchPosition.y, 0);
                //previousValidTouchPosition = touchPos;
                Ray ray = currentCamera.ScreenPointToRay(new Vector3(UserInputs.inputData.primaryTouchPosition.x, UserInputs.inputData.primaryTouchPosition.y, 0));
#else
                Ray ray = currentCamera.ScreenPointToRay(new Vector3(UserInputs.inputData.mousePosition.x, UserInputs.inputData.mousePosition.y, 0));
#endif
                Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer);

                if (hit.collider != null)
                {
                    //markerImage.transform.position = hit.point;
                    GridCell cell = levelController.GetGridCellAtWorldPosition(hit.point);
                    Vector3 estimatedPos = levelController.GetGridCellWorldPosition(cell);
                    float x = selectedItem.size.x % 2 == 0 ? estimatedPos.x + (cell.size / 2f) : estimatedPos.x;
                    float z = selectedItem.size.y % 2 == 0 ? estimatedPos.z + (cell.size / 2f) : estimatedPos.z;
                    estimatedPos = new Vector3(x, estimatedPos.y, z);

                    if (markerImage.transform.position != estimatedPos)
                    {
                        markerImage.transform.position = estimatedPos;
                        OnMarkerPositionChange(markerImage.transform.position);
                    }
                }
            }
        }

        private void OnMarkerPositionChange(Vector3 position)
        {
            // check for placement validity
            IsCurrentPositionValid = IsPositionValidForPlacement(position);
            markerImage.color = IsCurrentPositionValid ? validColor : invalidColor;
        }

        private bool IsPositionValidForPlacement(Vector3 position)
        {
            bool isValid = true;
            Vector3 boxExtents = new Vector3(selectedItem.size.x / 2.25f, 5f, selectedItem.size.y / 2.25f);
            Collider[] colls = Physics.OverlapBox(position, boxExtents, Quaternion.identity, placementValidityCheckLayer);

            bool isOccupied = colls.Any(t => (1 << t.gameObject.layer | itemsLayer) == itemsLayer);
            if (isOccupied)
                return false;

            foreach (var coll in colls)
            {
                WorldTile tile = coll.GetComponent<WorldTile>();
                if (tile != null)
                {
                    isValid = isValid && tile.validForPlacement.HasFlag(selectedItem.type);
                    if (!isValid)
                        break;
                }
            }
            return isValid;
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
