using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TowerDefense3D
{
    public class GameFlow : MonoBehaviour
    {
        public string saveFileName = "TDSaveData";
        public LevelData[] levels;
        public int mainMenuSceneIndex;

        [Header("UI")] 
        public GameObject mainMenuPanel;
        public GameObject levelSelectionPanel;
        public GameObject loadingPanel;
        public GameObject levelSelectionButtonObject;
        public Transform levelSelectionButtonParent;

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
        }

        private void OnDisable()
        {
            GameEvents.OnClickLevelButton.RemoveListener(OnClickLevelButton);
        }

        private void Start()
        {
            HideAllUI();
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
            GameEvents.OnGameSceneLoaded?.Invoke(lastClickedLevel);
            HideLoading();
            SceneManager.UnloadSceneAsync(mainMenuSceneIndex);
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

        public void ShowMainMenu()
        {
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

        private void HideAllUI()
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
                GameObject levelButton = Instantiate(levelSelectionButtonObject);
                levelButton.transform.SetParent(levelSelectionButtonParent);

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
            lastClickedLevel = data;
            HideLevelSelection();
            ShowLoading();
            StartLevel(data.scene);
        }

    }
}
