using System.Collections;
using System.Collections.Generic;
using nStation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense3D
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerData playerData;
        private IPlaceable currentSelectedItem;

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

        private void OnItemSelectedToPlace(PlaceableItemAttributes attributes)
        {
            GameObject obj = AddressableLoader.InstantiateAddressable(attributes.prefab); 
            obj.transform.SetParent(transform);
            currentSelectedItem = obj.GetComponent<IPlaceable>();
        }

        public void PlaceSelectedItem()
        {
            if (currentSelectedItem != null)
            {
                currentSelectedItem.Place(new Vector2(0, 0)); // spawn at current selected grid cell
                GameEvents.OnPlaceSelectedItem?.Invoke(currentSelectedItem.GetItemAttributes());
            }
        }

        private void OnCancelSelectionInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                currentSelectedItem = null;
                GameEvents.OnDeselectCurrentItem?.Invoke();
            }
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
