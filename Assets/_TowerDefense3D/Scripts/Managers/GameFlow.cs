using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TowerDefense3D
{
    public class GameFlow : MonoBehaviour
    {
        public string saveFileName = "TDSaveData";
        public LevelData[] levels;
        public int mainMenuSceneIndex;

        [Header("Main Menu UI")]
        public GameObject mainMenuParentUIObject;
        public GameObject mainMenuPanel;
        public GameObject levelSelectionPanel;
        public GameObject loadingPanel;
        public GameObject levelSelectionButtonObject;
        public Transform levelSelectionButtonParent;

        [Header("Level UI")] 
        public GameObject levelClearedLostParentUIObject;
        public GameObject levelClearedLostPanel;
        public UnityEngine.UI.Button nextLevelButton;
        public UnityEngine.UI.Button replayLevelButton;
        public UnityEngine.UI.Button selectLevelButton;
        public GameObject levelClearedTextObj;
        public GameObject levelLostTextObj;

        private CanvasGroup mainMenuCanvasGroup;
        private CanvasGroup levelSelectionCanvasGroup;
        private CanvasGroup loadingCanvasGroup;

        private Coroutine mainMenuAlphaCoroutine;
        private Coroutine levelSelectionAlphaCoroutine;
        private Coroutine loadingAlphaCoroutine;

        private LevelSelectionButton[] levelButtons;
        private LevelData lastClickedLevel;

        [SerializeField] private float uiTransitionTime = 1f;

        private void Awake()
        {
            levelButtons = new LevelSelectionButton[levels.Length];
            GameState.SaveGamePath = $"{Application.persistentDataPath}/{saveFileName}.json";
            GameState.LoadSavedData();

            mainMenuCanvasGroup = mainMenuPanel.GetComponent<CanvasGroup>();
            levelSelectionCanvasGroup = levelSelectionPanel.GetComponent<CanvasGroup>();
            loadingCanvasGroup = loadingPanel.GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            GameEvents.OnClickLevelButton.AddListener(OnClickLevelButton);
            GameEvents.OnLevelLost.AddListener(OnLevelLost);
            GameEvents.OnLevelCleared.AddListener(OnLevelCleared);
        }

        private void OnDisable()
        {
            GameEvents.OnClickLevelButton.RemoveListener(OnClickLevelButton);
            GameEvents.OnLevelLost.RemoveListener(OnLevelLost);
            GameEvents.OnLevelCleared.RemoveListener(OnLevelCleared);
        }

        private void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 60;
#endif
            HideAllMainMenuUIPanels();
            ShowMainMenu();
            InstantiateLevelButtons(levels);
        }

        public void StartLevel(AssetReference l_level)
        {
            StartCoroutine(LoadLevelCoroutine(l_level));
        }

        private IEnumerator LoadLevelCoroutine(AssetReference l_level)
        {
            var asyncOperation = Addressables.LoadSceneAsync(l_level, LoadSceneMode.Additive);
            SceneInstance scene = asyncOperation.WaitForCompletion();
            yield return scene.ActivateAsync();
            yield return new WaitForSeconds(0.35f); // just an offset

            Debug.Log("Level activated");
            GameEvents.OnGameSceneLoaded?.Invoke(GetLevelIndex(lastClickedLevel), lastClickedLevel);
            HideLoading();
            //SceneManager.UnloadSceneAsync(mainMenuSceneIndex);
            ToggleMainMenuParentUI(false);
        }

        public void UnloadLevel(Scene l_level, System.Action onDone = null)
        {
            StartCoroutine(UnloadLevelCoroutine(l_level, onDone));
        }

        private IEnumerator UnloadLevelCoroutine(Scene l_level, System.Action onDone = null)
        {
            var op = SceneManager.UnloadSceneAsync(l_level);
            yield return new WaitUntil (() => op.isDone);
            yield return new WaitForSeconds(0.35f);

            Debug.Log("level unloaded");
            lastClickedLevel = null;
            onDone?.Invoke();
        }

        public void StartNewGame()
        {
            lastClickedLevel = levels[0];
            HideMainMenu();
            ShowLoading();
            StartLevel(lastClickedLevel.scene);
        }

        private IEnumerator UITransitionCoroutine(CanvasGroup canvasGroup, bool isActivating, float duration, System.Action onDone = null)
        {
            float timeElapsed = 0;

            float start = isActivating ? 0 : 1;
            float end = isActivating ? 1 : 0;

            while (timeElapsed < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(start, end, timeElapsed/duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            onDone?.Invoke();
        }

        public void ToggleMainMenuParentUI(bool isActive)
        {
            mainMenuParentUIObject.SetActive(isActive);
        }

        public void ShowMainMenu()
        {
            if(!mainMenuParentUIObject.activeInHierarchy)
                ToggleMainMenuParentUI(true);

            mainMenuPanel.SetActive(true);
            mainMenuCanvasGroup.interactable = true;

            if(mainMenuAlphaCoroutine != null)
                StopCoroutine(mainMenuAlphaCoroutine);
            mainMenuAlphaCoroutine = StartCoroutine(UITransitionCoroutine(mainMenuCanvasGroup, true, uiTransitionTime));
        }

        public void HideMainMenu()
        {
            mainMenuCanvasGroup.interactable = false;
            if (mainMenuAlphaCoroutine != null)
                StopCoroutine(mainMenuAlphaCoroutine);
            mainMenuAlphaCoroutine = StartCoroutine(UITransitionCoroutine(mainMenuCanvasGroup, false, uiTransitionTime,
                () =>
                {
                    mainMenuPanel.SetActive(false);
                }));
        }

        public void ShowLevelSelection()
        {
            if (!mainMenuParentUIObject.activeInHierarchy)
                ToggleMainMenuParentUI(true);

            levelSelectionPanel.SetActive(true);
            levelSelectionCanvasGroup.interactable = true;

            if (levelSelectionAlphaCoroutine != null)
                StopCoroutine(levelSelectionAlphaCoroutine);
            levelSelectionAlphaCoroutine = StartCoroutine(UITransitionCoroutine(levelSelectionCanvasGroup, true, uiTransitionTime));

            UpdateLevelSelectionScreen();
        }

        public void HideLevelSelection()
        {
            levelSelectionCanvasGroup.interactable = false;
            if (levelSelectionAlphaCoroutine != null)
                StopCoroutine(levelSelectionAlphaCoroutine);
            levelSelectionAlphaCoroutine = StartCoroutine(UITransitionCoroutine(levelSelectionCanvasGroup, false, uiTransitionTime,
                () =>
                {
                    levelSelectionPanel.SetActive(false);
                }));
        }

        public void ShowLoading()
        {
            if (!mainMenuParentUIObject.activeInHierarchy)
                ToggleMainMenuParentUI(true);

            loadingPanel.SetActive(true);
            loadingCanvasGroup.interactable = true;

            if (loadingAlphaCoroutine != null)
                StopCoroutine(loadingAlphaCoroutine);
            loadingAlphaCoroutine = StartCoroutine(UITransitionCoroutine(loadingCanvasGroup, true, uiTransitionTime));

            UpdateLevelSelectionScreen();
        }

        public void HideLoading()
        {
            loadingCanvasGroup.interactable = false;
            if (loadingAlphaCoroutine != null)
                StopCoroutine(loadingAlphaCoroutine);
            loadingAlphaCoroutine = StartCoroutine(UITransitionCoroutine(loadingCanvasGroup, false, uiTransitionTime,
                () =>
                {
                    loadingPanel.SetActive(false);
                }));
        }

        public void ShowLevelClearedPanel()
        {
            levelClearedLostParentUIObject.SetActive(true);
            levelClearedLostPanel.SetActive(true);

            levelClearedTextObj.SetActive(true);
            levelLostTextObj.SetActive(false);

            replayLevelButton.interactable = true;
            nextLevelButton.interactable = GetLevelIndex(lastClickedLevel) != levels.Length-1;
        }

        public void ShowLevelLostPanel()
        {
            levelClearedLostParentUIObject.SetActive(true);
            levelClearedLostPanel.SetActive(true);

            levelClearedTextObj.SetActive(false);
            levelLostTextObj.SetActive(true);

            replayLevelButton.interactable = true;
            nextLevelButton.interactable = false;
        }

        public void HideLevelClearedLostPanel()
        {
            levelClearedLostParentUIObject.SetActive(false);
            levelClearedLostPanel.SetActive(false);
        }

        private void HideAllMainMenuUIPanels()
        {
            mainMenuCanvasGroup.alpha = 0;
            levelSelectionCanvasGroup.alpha = 0;
            loadingCanvasGroup.alpha = 0;
        }

        private void UpdateLevelSelectionScreen()
        {
            var gameData = GameState.GetGameData;

            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].SetLockState(gameData.maxLevelsCleared < i);
            }
        }

        private void InstantiateLevelButtons(LevelData[] l_levels)
        {
            var gameData = GameState.GetGameData;

            for (int i = 0; i < l_levels.Length; i++)
            {
                GameObject levelButton = Instantiate(levelSelectionButtonObject, levelSelectionButtonParent);

                LevelSelectionButton lsb = levelButton.GetComponent<LevelSelectionButton>();
                if (lsb != null)
                {
                    levelButtons[i] = lsb;
                    lsb.SetButtonData(new LevelSelectionButtonUIData { levelData = l_levels[i], levelName = l_levels[i].name, thumbnailSprite = l_levels[i].thumbnail, isLocked = gameData.maxLevelsCleared < i});
                }
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void OnClickLevelButton(LevelData data)
        {
            HideLevelSelection();
            ShowLoading();

            if (lastClickedLevel == null)
            {
                lastClickedLevel = data;
                StartLevel(data.scene);
            }
            else
            {
                UnloadLevel(SceneManager.GetSceneAt(GetLevelIndex(lastClickedLevel) + 1), () => { lastClickedLevel = data; StartLevel(data.scene); });
            }
        }

        public void GotoNextLevel()
        {
            HideLevelClearedLostPanel();
            int currentLevelIndex = GetLevelIndex(lastClickedLevel);
            int nextLevelIndex = (currentLevelIndex + 1) % levels.Length;

            UnloadLevel(SceneManager.GetSceneAt(GetLevelIndex(lastClickedLevel) + 1), () => { OnClickLevelButton(levels[nextLevelIndex]); }); 
        }

        public void ReplayLevel()
        {
            HideLevelClearedLostPanel();
            int currentIndex = GetLevelIndex(lastClickedLevel);
            UnloadLevel(SceneManager.GetSceneAt(currentIndex), () => { OnClickLevelButton(levels[currentIndex]); });
        }

        private void OnLevelCleared(LevelData l_data)
        {
            HideLevelClearedLostPanel();
            ShowLevelClearedPanel();
        }

        private void OnLevelLost(LevelData l_data)
        {
            ShowLevelLostPanel();
        }

        private int GetLevelIndex(LevelData l_data)
        {
            return Array.FindIndex(levels, t => t.id == l_data.id);
        }
    }
}
