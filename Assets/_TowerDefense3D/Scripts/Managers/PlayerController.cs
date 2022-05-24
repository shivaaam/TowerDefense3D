using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TowerDefense3D
{
    public class PlayerController : MonoBehaviour
    {
        public PlacementMarker placementMarker;

        private PlayerData playerData;
        private PlaceableItemAttributes currentSelectedItemAttributes;
        private BaseItem currentSelectedItem;
        private int playerLives;

        private List<BaseItem> placedItems = new List<BaseItem>();
        private float lastSelectedItemPlacedTime;

        [HideInInspector] public UnityEvent<int> OnUpdateCollectedMoney = new UnityEvent<int>();
        [HideInInspector] public UnityEvent<int> OnUpdatePlayerLives = new UnityEvent<int>();

        private void OnEnable()
        {
            GameEvents.OnSelectPlaceableItem.AddListener(OnItemSelectedToPlace);
            GameEvents.OnGameSceneLoaded.AddListener(OnGameLevelLoaded);
            UserInputs.OnCancelSelectionInputEvent.AddListener(OnCancelSelectionInput);
            UserInputs.OnPerformActionInputEvent.AddListener(OnPerformActionInput);
        }

        private void OnDisable()
        {
            GameEvents.OnSelectPlaceableItem.RemoveListener(OnItemSelectedToPlace);
            GameEvents.OnGameSceneLoaded.RemoveListener(OnGameLevelLoaded);
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
            if (currentSelectedItem == null || placementMarker == null || !placementMarker.IsCurrentPositionValid ||
                (Time.time - lastSelectedItemPlacedTime < currentSelectedItemAttributes.cooldownTime) ||
                playerData.money < currentSelectedItemAttributes.cost)
                return;

            UpdateMoneyAmount(-currentSelectedItemAttributes.cost);
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

        /// <summary>
        /// Takes -ve for subtracting and +ve for adding
        /// </summary>
        private void UpdateMoneyAmount(int amountToChange)
        {
            playerData.money = Mathf.Clamp(playerData.money + amountToChange, 0, Constants.maxPlayerMoney);
            OnUpdateCollectedMoney?.Invoke(playerData.money);
        }

        private void UpdatePlayerLives(int amountToChange)
        {
            playerLives = Mathf.Clamp(playerLives + amountToChange, 0, Constants.maxPlayerLives);
            OnUpdatePlayerLives?.Invoke(playerLives);

            if(playerLives <= 0)
                GameEvents.OnPLayerLIfeReachesZero?.Invoke();
        }

        private void OnGameLevelLoaded(LevelData l_data)
        {
            playerLives = Constants.startPlayerLives;
            playerData = GameState.GetGameData.playerData;
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public int money;

        public PlayerData()
        {
            money = Constants.startPlayerMoney;
        }
    }
}
