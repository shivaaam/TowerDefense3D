using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace TowerDefense3D
{
    public class PlayerController : MonoBehaviour
    {
        public PlacementMarker placementMarker;

        private PlayerData playerData;
        private PlaceableItemAttributes currentSelectedItemAttributes;
        private BaseItem currentSelectedItem;

        private List<BaseItem> placedItems = new List<BaseItem>();
        private float lastSelectedItemPlacedTime;

        private void OnEnable()
        {
            GameEvents.OnSelectPlaceableItem.AddListener(OnItemSelectedToPlace);
            UserInputs.OnCancelSelectionInputEvent.AddListener(OnCancelSelectionInput);
            UserInputs.OnPerformActionInputEvent.AddListener(OnPerformActionInput);
        }

        private void OnDisable()
        {
            GameEvents.OnSelectPlaceableItem.RemoveListener(OnItemSelectedToPlace);
            UserInputs.OnCancelSelectionInputEvent.RemoveListener(OnCancelSelectionInput);
            UserInputs.OnPerformActionInputEvent.RemoveListener(OnPerformActionInput);
        }

        private void Start()
        {
            lastSelectedItemPlacedTime = -50;
        }

        private void Update()
        {
            if (currentSelectedItem != null)
                currentSelectedItem.transform.position = placementMarker.Marker.position;
        }

        private void OnItemSelectedToPlace(PlaceableItemAttributes attributes)
        {
            if (currentSelectedItemAttributes == null || attributes.id != currentSelectedItemAttributes.id)
            {
                currentSelectedItemAttributes = attributes;
                if(placedItems.Count > 0)
                    lastSelectedItemPlacedTime = 0;
            }

            if(currentSelectedItem != null)
                RemoveCurrentSelectedItem();

            GameObject obj = AddressableLoader.InstantiateAddressable(attributes.prefab); 
            obj.transform.SetParent(transform);
            currentSelectedItem = obj.GetComponent<BaseItem>();
        }

        public void PlaceSelectedItem()
        {
            if (currentSelectedItem == null || placementMarker == null || !placementMarker.IsCurrentPositionValid || (Time.time - lastSelectedItemPlacedTime < currentSelectedItemAttributes.cooldownTime))
                return;

            lastSelectedItemPlacedTime = Time.time;
            currentSelectedItem.Place(placementMarker.Marker.position);
            AddToPlacedItems(currentSelectedItem);
            GameEvents.OnPlaceSelectedItem?.Invoke(currentSelectedItem);
            currentSelectedItem = null;
        
            // get another item instance to place
            OnItemSelectedToPlace(currentSelectedItemAttributes);
        }

        private void AddToPlacedItems(BaseItem item)
        {
            if(!placedItems.Contains(item))
                placedItems.Add(item);
        }

        private void OnCancelSelectionInput(InputAction.CallbackContext context)
        {
            if (currentSelectedItem == null)
                return;
            if (context.phase == InputActionPhase.Started)
            {
                RemoveCurrentSelectedItem();
                GameEvents.OnDeselectCurrentItem?.Invoke();
            }
        }

        private void RemoveCurrentSelectedItem()
        {
            if (currentSelectedItem == null)
                return;
            AddressableLoader.DestroyAndReleaseAddressable(currentSelectedItem.gameObject);
            currentSelectedItem = null;
        }

        private void OnPerformActionInput(InputAction.CallbackContext context)
        {
            if (GraphicRaycastObject.IsMouseOverGraphics)
                return;

            if (context.phase == InputActionPhase.Started)
            {
                PlaceSelectedItem();
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public int money;
    }
}
