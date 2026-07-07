using System.Collections.Generic;
using SRPG.Audio;
using SRPG.Battle;
using SRPG.Debugging;
using SRPG.UI;
using UnityEngine;

namespace SRPG.Stage
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private int currentStageIndex;

        private readonly List<StageData> stages = new List<StageData>();
        private bool allClear;
        private bool titleScreenOpen;
        private bool stageSelectOpen;
        private bool optionsOpen;
        private int selectedTitleMenuIndex;
        private int selectedOptionsIndex;
        private int selectedStageIndex;
        private int selectedResolutionIndex = 1;
        private bool hasLoadedStage;
        private bool wasBattleEnded;
        private bool waitForResultConfirmRelease;

        private static readonly Vector2Int[] ResolutionOptions =
        {
            new Vector2Int(1280, 720),
            new Vector2Int(1600, 900),
            new Vector2Int(1920, 1080)
        };

        public static StageManager Instance { get; private set; }

        public int CurrentStageNumber => currentStageIndex + 1;
        public int TotalStages => stages.Count;
        public bool IsTitleScreenOpen => titleScreenOpen;
        public bool IsStageSelectOpen => stageSelectOpen;
        public bool IsMenuOpen => titleScreenOpen || stageSelectOpen || optionsOpen;

        public void LoadCurrentStage()
        {
            EnsureStages();

            if (stages.Count == 0)
            {
                Debug.LogWarning("No stages available.");
                return;
            }

            var loader = EnsureStageLoader();
            if (loader == null)
            {
                Debug.LogWarning("StageLoader not found.");
                return;
            }

            var stageData = stages[currentStageIndex];
            allClear = false;
            titleScreenOpen = false;
            stageSelectOpen = false;
            optionsOpen = false;
            hasLoadedStage = true;
            wasBattleEnded = false;
            waitForResultConfirmRelease = false;
            DamagePopup.ClearAll();
            PlayerController.Instance?.ResetControllerState();
            TurnManager.Instance?.SetStageData(stageData);
            TurnManager.Instance?.ResetBattleState();
            BattleUI.Instance?.SetStageInfo(CurrentStageNumber, TotalStages);
            BattleUI.Instance?.SetObjectiveInfo(stageData);
            BattleUI.Instance?.ClearBattleLog();
            BattleUI.Instance?.ClearAttackPreview();
            BattleUI.Instance?.ClearResult();
            BattleUI.Instance?.HideTitleScreen();
            BattleUI.Instance?.HideOptionsScreen();
            BattleUI.Instance?.HideStageSelect();

            loader.LoadStage(stageData);

            TurnManager.Instance?.SetStageData(stageData);
            TurnManager.Instance?.ResetBattleState();
            BattleUI.Instance?.SetStageInfo(CurrentStageNumber, TotalStages);
            BattleUI.Instance?.SetObjectiveInfo(stageData);
            BattleUI.Instance?.ShowStageIntro(CurrentStageNumber, TotalStages, stageData);
            AudioManager.Instance?.PlayBattleBgm();
            DevLogger.Log($"Stage {CurrentStageNumber}/{TotalStages} started.");
        }

        public void LoadStageAt(int stageIndex)
        {
            EnsureStages();
            if (stages.Count == 0)
            {
                Debug.LogWarning("No stages available.");
                return;
            }

            currentStageIndex = ClampStageIndex(stageIndex);
            selectedStageIndex = currentStageIndex;
            LoadCurrentStage();
        }

        public void LoadNextStage()
        {
            if (!HasNextStage())
            {
                allClear = true;
                AudioManager.Instance?.StopBgm();
                AudioManager.Instance?.PlayAllClearSe();
                BattleUI.Instance?.ShowResult("ALL CLEAR\nThanks for playing prototype");
                BattleUI.Instance?.AddBattleLog("ALL CLEAR");
                Debug.Log("ALL CLEAR");
                return;
            }

            currentStageIndex++;
            LoadCurrentStage();
        }

        public void RetryCurrentStage()
        {
            AudioManager.Instance?.PlayRestartSe();
            LoadCurrentStage();
            BattleUI.Instance?.AddBattleLog($"Stage {CurrentStageNumber} restarted");
            DevLogger.Log($"Stage {CurrentStageNumber} restarted.");
        }

        public bool HasNextStage()
        {
            EnsureStages();
            return currentStageIndex + 1 < stages.Count;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureStages();
        }

        private void Start()
        {
            ShowTitleScreen();
        }

        private void Update()
        {
            if (titleScreenOpen)
            {
                HandleTitleScreenInput();
                return;
            }

            if (optionsOpen)
            {
                HandleOptionsInput();
                return;
            }

            if (stageSelectOpen)
            {
                HandleStageSelectInput();
                return;
            }

            if (PlayerController.Instance != null && PlayerController.Instance.IsAnimating)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.S))
            {
                AudioManager.Instance?.PlayCancelSe();
                ShowStageSelect();
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RetryCurrentStage();
                return;
            }

            var battleEnded = TurnManager.Instance != null && TurnManager.Instance.IsBattleEnded;
            if (battleEnded && !wasBattleEnded)
            {
                waitForResultConfirmRelease = Input.GetKey(KeyCode.Return);
            }
            else if (!battleEnded)
            {
                waitForResultConfirmRelease = false;
            }

            wasBattleEnded = battleEnded;

            if (allClear || TurnManager.Instance == null || !battleEnded)
            {
                return;
            }

            if (waitForResultConfirmRelease)
            {
                if (!Input.GetKey(KeyCode.Return))
                {
                    waitForResultConfirmRelease = false;
                }

                return;
            }

            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }

            if (TurnManager.Instance.Result == BattleResult.Victory)
            {
                AudioManager.Instance?.PlayConfirmSe();
                LoadNextStage();
                return;
            }

            if (TurnManager.Instance.Result == BattleResult.Defeat)
            {
                AudioManager.Instance?.PlayConfirmSe();
                LoadCurrentStage();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void EnsureStages()
        {
            if (stages.Count > 0)
            {
                return;
            }

            stages.AddRange(StageData.CreateDefaultStages());
        }

        private void ShowStageSelect()
        {
            EnsureStages();
            titleScreenOpen = false;
            stageSelectOpen = true;
            optionsOpen = false;
            selectedStageIndex = ClampStageIndex(hasLoadedStage ? currentStageIndex : selectedStageIndex);
            TurnManager.Instance?.StopActiveEnemyTurn();
            PlayerController.Instance?.ResetControllerState();
            BattleUI.Instance?.ClearAttackPreview();
            BattleUI.Instance?.SetSelectedUnit(null);
            BattleUI.Instance?.SetEnemyThreatVisible(false);
            BattleUI.Instance?.HideOptionsScreen();
            BattleUI.Instance?.ShowStageSelect(stages, selectedStageIndex);
            AudioManager.Instance?.PlayStageSelectBgm();
            DevLogger.Log("Stage select opened.");
        }

        private void ShowTitleScreen(int selectedMenuIndex = 0)
        {
            titleScreenOpen = true;
            stageSelectOpen = false;
            optionsOpen = false;
            selectedTitleMenuIndex = ClampTitleMenuIndex(selectedMenuIndex);
            PlayerController.Instance?.ResetControllerState();
            BattleUI.Instance?.SetSelectedUnit(null);
            BattleUI.Instance?.SetEnemyThreatVisible(false);
            BattleUI.Instance?.HideOptionsScreen();
            BattleUI.Instance?.ShowTitleScreen(selectedTitleMenuIndex);
            AudioManager.Instance?.PlayTitleBgm();
            DevLogger.Log("Title screen opened.");
        }

        private void HandleTitleScreenInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedTitleMenuIndex = ClampTitleMenuIndex(selectedTitleMenuIndex - 1);
                BattleUI.Instance?.SetTitleMenuSelection(selectedTitleMenuIndex);
                AudioManager.Instance?.PlayCursorSe();
                return;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedTitleMenuIndex = ClampTitleMenuIndex(selectedTitleMenuIndex + 1);
                BattleUI.Instance?.SetTitleMenuSelection(selectedTitleMenuIndex);
                AudioManager.Instance?.PlayCursorSe();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmTitleMenuSelection();
            }
        }

        private void ConfirmTitleMenuSelection()
        {
            switch (selectedTitleMenuIndex)
            {
                case 0:
                    AudioManager.Instance?.PlayConfirmSe();
                    LoadStageAt(0);
                    break;
                case 1:
                    AudioManager.Instance?.PlayConfirmSe();
                    ShowStageSelect();
                    break;
                case 2:
                    AudioManager.Instance?.PlayConfirmSe();
                    ShowOptionsScreen();
                    break;
                case 3:
                    AudioManager.Instance?.PlayConfirmSe();
                    DevLogger.Log("EXIT selected.");
                    Application.Quit();
                    break;
            }
        }

        private void ShowOptionsScreen()
        {
            titleScreenOpen = false;
            stageSelectOpen = false;
            optionsOpen = true;
            selectedOptionsIndex = 0;
            selectedResolutionIndex = GetClosestResolutionIndex(Screen.width, Screen.height);
            BattleUI.Instance?.ShowOptionsScreen(AudioManager.Instance, selectedOptionsIndex);
            AudioManager.Instance?.PlayTitleBgm();
            DevLogger.Log("Options screen opened.");
        }

        private void HandleOptionsInput()
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance?.PlayCancelSe();
                ShowTitleScreen(2);
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedOptionsIndex = ClampOptionsIndex(selectedOptionsIndex - 1);
                BattleUI.Instance?.ShowOptionsScreen(AudioManager.Instance, selectedOptionsIndex);
                AudioManager.Instance?.PlayCursorSe();
                return;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedOptionsIndex = ClampOptionsIndex(selectedOptionsIndex + 1);
                BattleUI.Instance?.ShowOptionsScreen(AudioManager.Instance, selectedOptionsIndex);
                AudioManager.Instance?.PlayCursorSe();
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AdjustOption(-0.05f);
                return;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                AdjustOption(0.05f);
                return;
            }

            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }

            if (selectedOptionsIndex == 3)
            {
                AudioManager.Instance?.PlayConfirmSe();
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.MuteAll = !AudioManager.Instance.MuteAll;
                }

                BattleUI.Instance?.ShowOptionsScreen(AudioManager.Instance, selectedOptionsIndex);
                return;
            }

            if (selectedOptionsIndex == 5)
            {
                ToggleFullscreen();
                return;
            }

            if (selectedOptionsIndex == 6)
            {
                AudioManager.Instance?.PlayCancelSe();
                ShowTitleScreen(2);
            }
        }

        private void AdjustOption(float delta)
        {
            if (selectedOptionsIndex == 4)
            {
                CycleResolution(delta < 0f ? -1 : 1);
                return;
            }

            if (selectedOptionsIndex == 5)
            {
                ToggleFullscreen();
                return;
            }

            var audio = AudioManager.Instance;
            switch (selectedOptionsIndex)
            {
                case 0:
                    if (audio == null)
                    {
                        return;
                    }
                    audio.MasterVolume += delta;
                    break;
                case 1:
                    if (audio == null)
                    {
                        return;
                    }
                    audio.BgmVolume += delta;
                    break;
                case 2:
                    if (audio == null)
                    {
                        return;
                    }
                    audio.SeVolume += delta;
                    break;
                default:
                    return;
            }

            AudioManager.Instance?.PlayCursorSe();
            BattleUI.Instance?.ShowOptionsScreen(audio, selectedOptionsIndex);
        }

        private void CycleResolution(int direction)
        {
            selectedResolutionIndex = GetClosestResolutionIndex(Screen.width, Screen.height);
            selectedResolutionIndex += direction;
            if (selectedResolutionIndex < 0)
            {
                selectedResolutionIndex = ResolutionOptions.Length - 1;
            }
            else if (selectedResolutionIndex >= ResolutionOptions.Length)
            {
                selectedResolutionIndex = 0;
            }

            var resolution = ResolutionOptions[selectedResolutionIndex];
            Screen.SetResolution(resolution.x, resolution.y, GetCurrentFullScreenMode());
            AudioManager.Instance?.PlayCursorSe();
            BattleUI.Instance?.ShowOptionsScreen(AudioManager.Instance, selectedOptionsIndex, FormatResolution(resolution), FormatDisplayMode(GetCurrentFullScreenMode()));
            Debug.Log($"Resolution changed: {resolution.x}x{resolution.y}, Display: {(Screen.fullScreen ? "Full Screen" : "Window")}");
        }

        private void ToggleFullscreen()
        {
            var resolution = ResolutionOptions[GetClosestResolutionIndex(Screen.width, Screen.height)];
            var targetMode = Screen.fullScreen ? FullScreenMode.Windowed : FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(resolution.x, resolution.y, targetMode);
            AudioManager.Instance?.PlayConfirmSe();
            BattleUI.Instance?.ShowOptionsScreen(AudioManager.Instance, selectedOptionsIndex, FormatResolution(resolution), FormatDisplayMode(targetMode));
            Debug.Log($"Display mode changed: {(Screen.fullScreen ? "Full Screen" : "Window")} {resolution.x}x{resolution.y}");
        }

        private string FormatResolution(Vector2Int resolution)
        {
            return $"{resolution.x} x {resolution.y}";
        }

        private string FormatDisplayMode(FullScreenMode mode)
        {
            return mode == FullScreenMode.Windowed ? "Window" : "Full Screen";
        }

        private FullScreenMode GetCurrentFullScreenMode()
        {
            return Screen.fullScreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        }

        private int GetClosestResolutionIndex(int width, int height)
        {
            var bestIndex = 0;
            var bestDistance = int.MaxValue;
            for (var i = 0; i < ResolutionOptions.Length; i++)
            {
                var resolution = ResolutionOptions[i];
                var distance = Mathf.Abs(resolution.x - width) + Mathf.Abs(resolution.y - height);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private void HandleStageSelectInput()
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance?.PlayCancelSe();
                ShowTitleScreen();
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedStageIndex = ClampStageIndex(selectedStageIndex - 1);
                BattleUI.Instance?.ShowStageSelect(stages, selectedStageIndex);
                AudioManager.Instance?.PlayCursorSe();
                return;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedStageIndex = ClampStageIndex(selectedStageIndex + 1);
                BattleUI.Instance?.ShowStageSelect(stages, selectedStageIndex);
                AudioManager.Instance?.PlayCursorSe();
                return;
            }

            var quickStartIndex = GetQuickStartStageIndex();
            if (quickStartIndex >= 0)
            {
                AudioManager.Instance?.PlayConfirmSe();
                LoadStageAt(quickStartIndex);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                AudioManager.Instance?.PlayConfirmSe();
                LoadStageAt(selectedStageIndex);
            }
        }

        private int GetQuickStartStageIndex()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                return 0;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                return 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                return 2;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                return 3;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                return 4;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                return 5;
            }

            return -1;
        }

        private int ClampStageIndex(int stageIndex)
        {
            EnsureStages();
            if (stages.Count == 0)
            {
                return 0;
            }

            if (stageIndex < 0)
            {
                return stages.Count - 1;
            }

            if (stageIndex >= stages.Count)
            {
                return 0;
            }

            return stageIndex;
        }

        private int ClampTitleMenuIndex(int menuIndex)
        {
            if (menuIndex < 0)
            {
                return 3;
            }

            if (menuIndex > 3)
            {
                return 0;
            }

            return menuIndex;
        }

        private int ClampOptionsIndex(int optionIndex)
        {
            if (optionIndex < 0)
            {
                return 6;
            }

            if (optionIndex > 6)
            {
                return 0;
            }

            return optionIndex;
        }

        private StageLoader EnsureStageLoader()
        {
            var loader = StageLoader.Instance != null ? StageLoader.Instance : FindAnyObjectByType<StageLoader>();
            if (loader != null)
            {
                return loader;
            }

            var loaderObject = new GameObject("StageLoader");
            return loaderObject.AddComponent<StageLoader>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapStageManager()
        {
            if (FindAnyObjectByType<StageManager>() == null)
            {
                var managerObject = new GameObject("StageManager");
                managerObject.AddComponent<StageManager>();
            }
        }
    }
}
