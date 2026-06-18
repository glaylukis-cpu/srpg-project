using System.Collections.Generic;
using System.Collections;
using System.Text;
using SRPG.Audio;
using SRPG.Battle;
using SRPG.Grid;
using SRPG.Stage;
using SRPG.Units;
using UnityEngine;
using UnityEngine.UI;

namespace SRPG.UI
{
    [RequireComponent(typeof(Canvas))]
    public class BattleUI : MonoBehaviour
    {
        [SerializeField] private Text turnText;
        [SerializeField] private Text stageText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text selectedUnitText;
        [SerializeField] private Text enemyThreatText;
        [SerializeField] private Text controlsText;
        [SerializeField] private Text controlsInputText;
        [SerializeField] private Text controlsActionText;
        [SerializeField] private Text battleLogText;
        [SerializeField] private Text attackPreviewText;
        [SerializeField] private Text resultText;
        [SerializeField] private Text stageIntroText;
        [SerializeField] private Image battleBackdropImage;
        [SerializeField] private Image battleStageFrame;
        [SerializeField] private Image battleStagePanel;
        [SerializeField] private Image battleObjectiveFrame;
        [SerializeField] private Image battleObjectivePanel;
        [SerializeField] private Image battleSelectedFrame;
        [SerializeField] private Image battleSelectedPanel;
        [SerializeField] private Image battleThreatFrame;
        [SerializeField] private Image battleThreatPanel;
        [SerializeField] private Image battleLogFrame;
        [SerializeField] private Image battleLogPanel;
        [SerializeField] private Image battleControlsFrame;
        [SerializeField] private Image battleControlsPanel;
        [SerializeField] private Image battleAttackPreviewFrame;
        [SerializeField] private Image battleAttackPreviewPanel;
        [SerializeField] private Image battleResultFrame;
        [SerializeField] private Image battleResultPanel;
        [SerializeField] private Text stageSelectText;
        [SerializeField] private Text titleText;
        [SerializeField] private Image titleBackgroundImage;
        [SerializeField] private Image titleDimOverlayImage;
        [SerializeField] private Image titleMenuFrame;
        [SerializeField] private Image titleMenuPanel;
        [SerializeField] private Image titleMenuHighlightPanel;
        [SerializeField] private Image titleMenuItemsRootPanel;
        [SerializeField] private RectTransform titleMenuItemsRoot;
        [SerializeField] private Text titleCursorText;
        [SerializeField] private Text titleStartText;
        [SerializeField] private Text titleStageSelectText;
        [SerializeField] private Text titleOptionsText;
        [SerializeField] private Text titleExitText;
        [SerializeField] private Image titleFooterFrame;
        [SerializeField] private Image titleFooterPanel;
        [SerializeField] private Text titlePromptText;
        [SerializeField] private Image optionsFrame;
        [SerializeField] private Image optionsPanel;
        [SerializeField] private Text optionsHeaderText;
        [SerializeField] private Text optionsBodyText;
        [SerializeField] private Text optionsFooterText;
        [SerializeField] private Image stageSelectBackdropImage;
        [SerializeField] private Image stageSelectCenterGlowImage;
        [SerializeField] private Image stageSelectTopInfoFrame;
        [SerializeField] private Image stageSelectTopInfoPanel;
        [SerializeField] private Image stageSelectListFrame;
        [SerializeField] private Image stageSelectListPanel;
        [SerializeField] private Image stageSelectDetailFrame;
        [SerializeField] private Image stageSelectDetailPanel;
        [SerializeField] private Image stageSelectFooterFrame;
        [SerializeField] private Image stageSelectFooterPanel;
        [SerializeField] private Text stageSelectHeaderText;
        [SerializeField] private Text stageSelectTopInfoText;
        [SerializeField] private Text stageSelectListText;
        [SerializeField] private Text stageSelectDetailText;
        [SerializeField] private Text stageSelectDataLabelText;
        [SerializeField] private Text stageSelectDataValueText;
        [SerializeField] private Text stageSelectBestText;
        [SerializeField] private Text stageSelectFooterText;
        [SerializeField] private Image stageSelectMapPreviewFrame;
        [SerializeField] private Image stageSelectMapPreviewPanel;
        [SerializeField] private Text stageSelectMapPreviewText;
        [SerializeField] private Text stageSelectMapPreviewLegendText;

        private const int MaxBattleLogEntries = 5;
        private const int MaxStageSelectRows = 6;
        private const int StagePreviewSize = 8;
        private const float StageIntroDuration = 1.8f;
        private readonly List<string> battleLogEntries = new List<string>();
        private readonly Dictionary<int, StageBestResult> sessionBestResults = new Dictionary<int, StageBestResult>();
        private readonly Image[] stageSelectRowPanels = new Image[MaxStageSelectRows];
        private readonly Text[] stageSelectRowNumberTexts = new Text[MaxStageSelectRows];
        private readonly Text[] stageSelectRowNameTexts = new Text[MaxStageSelectRows];
        private readonly Text[] stageSelectRowDifficultyTexts = new Text[MaxStageSelectRows];
        private readonly Image[] stageSelectPreviewCells = new Image[StagePreviewSize * StagePreviewSize];
        private Font defaultFont;
        private Coroutine stageIntroCoroutine;
        private int selectedStageSelectRow = -1;
        private int selectedTitleMenuRow;
        private int selectedOptionsRow;
        private float stageSelectPulseTimer;
        private float titlePulseTimer;
        private bool titleOverlayInitialized;
        private bool optionsOverlayInitialized;
        private bool stageSelectObjectsInitialized;
        private StageData currentStageData;
        private int currentStageNumber;
        private int currentTotalStages;
        private static bool controlsLogged;

        public static BattleUI Instance { get; private set; }

        private void Update()
        {
            AnimateStageSelectSelection();
            AnimateTitleSelection();
        }

        private Color ThemeBackgroundDark()
        {
            return new Color(0.005f, 0.012f, 0.028f, 0.98f);
        }

        private Color ThemePanelDark()
        {
            return new Color(0.024f, 0.04f, 0.064f, 0.9f);
        }

        private Color ThemePanelBorderGold()
        {
            return new Color(0.7f, 0.5f, 0.22f, 0.82f);
        }

        private Color ThemeTextPrimary()
        {
            return Color.white;
        }

        private Color ThemeTextAccentGold()
        {
            return new Color(1f, 0.82f, 0.45f, 1f);
        }

        private Color ThemeTextMutedGold()
        {
            return new Color(1f, 0.88f, 0.64f, 1f);
        }

        private Color ThemeSelectionBlue()
        {
            return new Color(0.08f, 0.31f, 0.58f, 0.9f);
        }

        private Color ThemeSelectionGlow()
        {
            return new Color(0.18f, 0.52f, 0.88f, 0.95f);
        }

        public void SetTurnInfo(int turnNumber, string phaseName)
        {
            EnsureTextObjects();
            turnText.text = $"Turn {turnNumber}\n{phaseName}";
        }

        public void SetStageInfo(int currentStage, int totalStages)
        {
            EnsureTextObjects();
            currentStageNumber = currentStage;
            currentTotalStages = totalStages;
            RefreshStageText();
        }

        public void SetObjectiveInfo(StageData data)
        {
            EnsureTextObjects();

            if (data == null)
            {
                currentStageData = null;
                RefreshStageText();
                SetObjectiveInfo(string.Empty);
                return;
            }

            currentStageData = data;
            RefreshStageText();
            SetObjectiveInfo(BuildObjectiveText(data, true));
        }

        public void SetObjectiveInfo(string text)
        {
            EnsureTextObjects();
            objectiveText.text = text;
        }

        public void SetSelectedUnit(Unit unit)
        {
            EnsureTextObjects();

            if (unit == null || unit.IsDead)
            {
                selectedUnitText.text =
                    "<color=#FFD98F>Selected</color>\n" +
                    "No Unit Selected\n" +
                    "<color=#AEB7C2>Click an ally or enemy.</color>";
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("<color=#FFD98F>Selected</color>");
            builder.AppendLine(unit.name);
            builder.AppendLine();
            builder.AppendLine($"<color=#FFD98F>Type</color>    {unit.UnitType}");

            if (unit.Faction == Faction.Enemy)
            {
                var prediction = TurnManager.Instance != null ? TurnManager.Instance.GetEnemyActionPrediction(unit) : "Prediction:\nNone";
                builder.AppendLine($"<color=#FFD98F>HP</color>      {unit.CurrentHp} / {unit.MaxHp}");
                builder.AppendLine($"<color=#FFD98F>AI</color>      {unit.EnemyAIType}");
                builder.AppendLine("<color=#FFD98F>Prediction</color>");
                builder.Append(WrapText(CleanPredictionText(prediction), 26));
            }
            else
            {
                builder.AppendLine($"<color=#FFD98F>HP</color>      {unit.CurrentHp} / {unit.MaxHp}");
                builder.AppendLine($"<color=#FFD98F>ATK</color>     {unit.AttackPower}");
                builder.AppendLine($"<color=#FFD98F>MOVE</color>    {unit.MovePower}");
                builder.Append($"<color=#FFD98F>RANGE</color>   {unit.AttackRange}");
            }

            selectedUnitText.text = builder.ToString();
        }

        public void SetEnemyThreatVisible(bool visible)
        {
            EnsureTextObjects();
            enemyThreatText.text = visible ? "Enemy Threat: ON" : "Enemy Threat: OFF";
        }

        public void ShowResult(string result)
        {
            EnsureTextObjects();
            HideTitleScreen();
            HideStageIntro();
            SetResultPanelVisible(true);
            resultText.fontSize = result != null && result.StartsWith("ALL CLEAR") ? 46 : 32;
            resultText.text = BuildResultText(result);
            resultText.enabled = true;
        }

        public void ShowResult(
            string result,
            string rating,
            int turnNumber,
            string turnLimit,
            int playersAlive,
            int playerHpTotal,
            int enemiesAlive,
            int enemyHpTotal)
        {
            EnsureTextObjects();
            HideTitleScreen();
            HideStageIntro();
            UpdateSessionBestResult(result, rating, turnNumber, playersAlive, playerHpTotal);
            SetResultPanelVisible(true);
            resultText.fontSize = 30;
            resultText.text = BuildResultText(result, rating, turnNumber, turnLimit, playersAlive, playerHpTotal, enemiesAlive, enemyHpTotal);
            resultText.enabled = true;
        }

        public void ShowStageIntro(int currentStage, int totalStages, StageData data)
        {
            EnsureTextObjects();
            ClearResult();
            HideTitleScreen();
            HideStageSelect();

            var builder = new StringBuilder();
            builder.AppendLine($"Stage {currentStage} / {totalStages}");
            if (data != null)
            {
                builder.AppendLine(data.DisplayName);
                builder.AppendLine($"Theme: {data.ThemeName}");
                builder.AppendLine($"Difficulty: {data.DifficultyLabel}");
                AppendDescriptionLine(builder, data.Description);
            }

            builder.AppendLine(BuildObjectiveText(data, false));
            builder.Append($"Limit: {BuildLimitText(data)}");

            if (stageIntroCoroutine != null)
            {
                StopCoroutine(stageIntroCoroutine);
                stageIntroCoroutine = null;
            }

            stageIntroText.text = builder.ToString();
            stageIntroText.enabled = true;
            stageIntroCoroutine = StartCoroutine(HideStageIntroAfterDelay());
        }

        public void ShowStageSelect(IReadOnlyList<StageData> stages, int selectedStageIndex)
        {
            EnsureTextObjects();
            HideTitleScreen();
            HideStageIntro();
            ClearResult();

            SetStageSelectVisible(true);
            SetBattleHudVisible(false);
            stageSelectHeaderText.text = "\u25C6 STAGE SELECT \u25C6";

            if (stages == null || stages.Count == 0)
            {
                stageSelectTopInfoText.text = "No stages available.";
                stageSelectListText.text = "STAGES";
                stageSelectDetailText.text = string.Empty;
                stageSelectDataLabelText.text = string.Empty;
                stageSelectDataValueText.text = string.Empty;
                stageSelectBestText.text = string.Empty;
                stageSelectMapPreviewLegendText.text = string.Empty;
                stageSelectFooterText.text = "Esc / Backspace: Title";
                RefreshStageSelectRows(null, -1);
                RefreshStagePreview(null);
            }
            else
            {
                selectedStageIndex = Mathf.Max(0, selectedStageIndex);
                if (selectedStageIndex >= stages.Count)
                {
                    selectedStageIndex = stages.Count - 1;
                }

                var selectedStage = stages[selectedStageIndex];
                stageSelectTopInfoText.text =
                    $"Chapter {selectedStageIndex + 1}\n" +
                    $"{selectedStage.DisplayName}\n" +
                    $"Difficulty: {selectedStage.DifficultyLabel}";

                stageSelectListText.text = "STAGES";
                RefreshStageSelectRows(stages, selectedStageIndex);
                stageSelectDetailText.text = BuildStageSelectDetailText(selectedStage, selectedStageIndex + 1, stages.Count);
                stageSelectDataLabelText.text = BuildStageDataLabelText();
                stageSelectDataValueText.text = BuildStageDataValueText(selectedStage);
                stageSelectBestText.text = BuildStageBestResultText(selectedStageIndex + 1);
                stageSelectMapPreviewLegendText.text =
                    "<color=#3F8BFF>Blue</color> Player  <color=#FF3838>Red</color> Enemy\n" +
                    "<color=#0DB89E>Green</color> Goal  <color=#888888>Dark</color> Obstacle";
                RefreshStagePreview(selectedStage);
                stageSelectFooterText.text =
                    "<color=#FFD98F>Enter</color>: Start      " +
                    "<color=#FFD98F>Esc / Backspace</color>: Title      " +
                    "<color=#FFD98F>Up / Down</color>: Select      " +
                    "<color=#FFD98F>1-6</color>: Quick Start";
            }

            stageSelectText.text = string.Empty;
            stageSelectText.enabled = true;
        }

        public void ShowTitleScreen(int selectedMenuIndex = 0)
        {
            EnsureTextObjects();
            HideStageIntro();
            HideStageSelect();
            HideOptionsScreen();
            ClearResult();
            SetBattleHudVisible(false);
            SetTitleMenuSelection(selectedMenuIndex);
            RefreshTitlePromptText(false);

            if (titleBackgroundImage != null && titleBackgroundImage.sprite != null)
            {
                SetTitleMenuSelection(selectedMenuIndex);
                RefreshTitlePromptText(false);
                BringTitleObjectsToFront();
                titleBackgroundImage.enabled = true;
                SetTitleOverlayVisible(true);
                titleText.text = string.Empty;
                titleText.enabled = false;
                return;
            }

            SetTitleOverlayVisible(false);
            titleText.text =
                "PUZZLE SRPG PROTOTYPE\n" +
                "v0.2 Locked Playable Build\n\n" +
                "Six compact tactical puzzles.\n" +
                "Read enemy ranges. Plan the order. Commit cleanly.\n\n" +
                "Enter: Stage Select";
            titleText.enabled = true;
        }

        public void SetTitleMenuSelection(int selectedMenuIndex)
        {
            EnsureTextObjects();
            if (selectedMenuIndex < 0)
            {
                selectedMenuIndex = 0;
            }
            else if (selectedMenuIndex > 3)
            {
                selectedMenuIndex = 3;
            }

            selectedTitleMenuRow = selectedMenuIndex;
            RefreshTitleMenuText();
            UpdateTitleHighlightPosition();
        }

        public void HideTitleScreen()
        {
            EnsureTextObjects();
            if (titleBackgroundImage != null)
            {
                titleBackgroundImage.enabled = false;
            }

            SetTitleOverlayVisible(false);
            SetOptionsOverlayVisible(false);
            titleText.text = string.Empty;
            titleText.enabled = false;
            SetBattleHudVisible(true);
        }

        public void ShowOptionsScreen(AudioManager audioManager, int selectedOptionIndex)
        {
            EnsureTextObjects();
            HideStageIntro();
            HideStageSelect();
            ClearResult();
            SetBattleHudVisible(false);
            SetTitleOverlayVisible(false);

            selectedOptionsRow = Mathf.Max(0, Mathf.Min(selectedOptionIndex, 6));
            if (titleBackgroundImage != null && titleBackgroundImage.sprite != null)
            {
                titleBackgroundImage.enabled = true;
            }

            RefreshOptionsText(audioManager);
            SetOptionsOverlayVisible(true);
            BringOptionsObjectsToFront();
        }

        public void HideOptionsScreen()
        {
            EnsureTextObjects();
            if (!optionsOverlayInitialized)
            {
                optionsOverlayInitialized = true;
            }

            SetOptionsOverlayVisible(false);
        }

        public void HideStageSelect()
        {
            EnsureTextObjects();
            stageSelectText.text = string.Empty;
            stageSelectText.enabled = false;
            SetStageSelectVisible(false);
        }

        public void AddBattleLog(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            EnsureTextObjects();
            battleLogEntries.Add(message);

            while (battleLogEntries.Count > MaxBattleLogEntries)
            {
                battleLogEntries.RemoveAt(0);
            }

            RefreshBattleLogText();
        }

        public void ClearBattleLog()
        {
            EnsureTextObjects();
            battleLogEntries.Clear();
            RefreshBattleLogText();
        }

        public void ShowAttackPreview(Unit attacker, Unit target, bool inRange)
        {
            EnsureTextObjects();

            if (attacker == null || target == null || attacker.IsDead || target.IsDead)
            {
                ClearAttackPreview();
                return;
            }

            if (!inRange)
            {
                attackPreviewText.text = "<color=#FFD98F>Attack Preview</color>\nOut of range";
                SetAttackPreviewPanelVisible(true);
                attackPreviewText.enabled = true;
                return;
            }

            var damage = attacker.AttackPower;
            var beforeHp = target.CurrentHp;
            var afterHp = Mathf.Max(0, beforeHp - damage);

            var builder = new StringBuilder();
            builder.AppendLine("<color=#FFD98F>Attack Preview</color>");
            builder.AppendLine($"{attacker.name} -> {target.name}");
            builder.AppendLine($"Damage: {damage}");
            builder.AppendLine($"{target.name} HP: {beforeHp} -> {afterHp}");

            if (afterHp <= 0)
            {
                builder.Append("Will defeat");
            }

            attackPreviewText.text = builder.ToString().TrimEnd();
            SetAttackPreviewPanelVisible(true);
            attackPreviewText.enabled = true;
        }

        public void ClearAttackPreview()
        {
            EnsureTextObjects();
            attackPreviewText.text = string.Empty;
            attackPreviewText.enabled = false;
            SetAttackPreviewPanelVisible(false);
        }

        public void ClearResult()
        {
            EnsureTextObjects();
            resultText.text = string.Empty;
            resultText.enabled = false;
            SetResultPanelVisible(false);
        }

        public void HideStageIntro()
        {
            EnsureTextObjects();

            if (stageIntroCoroutine != null)
            {
                StopCoroutine(stageIntroCoroutine);
                stageIntroCoroutine = null;
            }

            stageIntroText.text = string.Empty;
            stageIntroText.enabled = false;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SetupCanvas();
            EnsureTextObjects();
            LogControlsOnce();
        }

        private void Start()
        {
            SetSelectedUnit(null);
            SetEnemyThreatVisible(false);
            SetControlsInfo();

            if (TurnManager.Instance != null)
            {
                SetTurnInfo(TurnManager.Instance.TurnNumber, TurnManager.Instance.CurrentPhase.ToString());
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void SetupCanvas()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void EnsureTextObjects()
        {
            defaultFont = defaultFont == null ? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") : defaultFont;
            EnsureTitleBackgroundImage();
            EnsureTitleOverlayObjects();
            EnsureOptionsObjects();
            EnsureStageSelectObjects();
            EnsureBattleHudObjects();

            if (turnText == null)
            {
                turnText = CreateText("TurnText", new Vector2(34f, -98f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 18, new Vector2(238f, 56f));
            }
            turnText.fontSize = 18;
            turnText.color = ThemeTextPrimary();
            ConfigureRect(turnText.rectTransform, new Vector2(34f, -98f), new Vector2(0f, 1f), new Vector2(238f, 56f));

            if (stageText == null)
            {
                stageText = CreateText("StageText", new Vector2(34f, -32f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 18, new Vector2(238f, 62f));
            }
            stageText.fontSize = 18;
            stageText.color = ThemeTextAccentGold();
            ConfigureRect(stageText.rectTransform, new Vector2(34f, -32f), new Vector2(0f, 1f), new Vector2(238f, 62f));

            if (objectiveText == null)
            {
                objectiveText = CreateText("ObjectiveText", new Vector2(44f, -192f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 11, new Vector2(216f, 146f));
            }
            objectiveText.fontSize = 11;
            objectiveText.color = ThemeTextPrimary();
            ConfigureRect(objectiveText.rectTransform, new Vector2(44f, -192f), new Vector2(0f, 1f), new Vector2(216f, 146f));

            if (selectedUnitText == null)
            {
                selectedUnitText = CreateText("SelectedUnitText", new Vector2(38f, 56f), new Vector2(0f, 0f), TextAnchor.LowerLeft, 15, new Vector2(224f, 136f));
            }
            selectedUnitText.fontSize = 15;
            selectedUnitText.alignment = TextAnchor.LowerLeft;
            selectedUnitText.color = ThemeTextPrimary();
            ConfigureRect(selectedUnitText.rectTransform, new Vector2(38f, 56f), new Vector2(0f, 0f), new Vector2(224f, 136f));

            if (enemyThreatText == null)
            {
                enemyThreatText = CreateText("EnemyThreatText", new Vector2(-34f, -34f), new Vector2(1f, 1f), TextAnchor.UpperRight, 17, new Vector2(208f, 24f));
            }
            enemyThreatText.fontSize = 17;
            enemyThreatText.alignment = TextAnchor.UpperRight;
            enemyThreatText.color = ThemeTextMutedGold();
            ConfigureRect(enemyThreatText.rectTransform, new Vector2(-34f, -34f), new Vector2(1f, 1f), new Vector2(208f, 24f));

            if (controlsText == null)
            {
                controlsText = CreateText("ControlsText", new Vector2(-28f, 152f), new Vector2(1f, 0f), TextAnchor.UpperLeft, 13, new Vector2(220f, 22f));
            }
            controlsText.fontSize = 13;
            controlsText.alignment = TextAnchor.UpperLeft;
            controlsText.color = ThemeTextAccentGold();
            ConfigureRect(controlsText.rectTransform, new Vector2(-28f, 152f), new Vector2(1f, 0f), new Vector2(220f, 22f));

            if (controlsInputText == null)
            {
                controlsInputText = CreateText("ControlsInputText", new Vector2(-142f, 44f), new Vector2(1f, 0f), TextAnchor.UpperLeft, 10, new Vector2(110f, 92f));
            }
            controlsInputText.fontSize = 10;
            controlsInputText.alignment = TextAnchor.UpperLeft;
            controlsInputText.color = ThemeTextAccentGold();
            ConfigureRect(controlsInputText.rectTransform, new Vector2(-142f, 44f), new Vector2(1f, 0f), new Vector2(110f, 92f));

            if (controlsActionText == null)
            {
                controlsActionText = CreateText("ControlsActionText", new Vector2(-28f, 44f), new Vector2(1f, 0f), TextAnchor.UpperLeft, 10, new Vector2(100f, 92f));
            }
            controlsActionText.fontSize = 10;
            controlsActionText.alignment = TextAnchor.UpperLeft;
            controlsActionText.color = ThemeTextPrimary();
            ConfigureRect(controlsActionText.rectTransform, new Vector2(-28f, 44f), new Vector2(1f, 0f), new Vector2(100f, 92f));

            if (battleLogText == null)
            {
                battleLogText = CreateText("BattleLogText", new Vector2(-28f, -42f), new Vector2(1f, 0.5f), TextAnchor.UpperLeft, 13, new Vector2(222f, 114f));
                RefreshBattleLogText();
            }
            battleLogText.fontSize = 13;
            battleLogText.alignment = TextAnchor.UpperLeft;
            battleLogText.color = ThemeTextPrimary();
            ConfigureRect(battleLogText.rectTransform, new Vector2(-28f, -42f), new Vector2(1f, 0.5f), new Vector2(222f, 114f));

            if (attackPreviewText == null)
            {
                attackPreviewText = CreateText("AttackPreviewText", new Vector2(0f, 30f), new Vector2(0.5f, 0f), TextAnchor.MiddleCenter, 16, new Vector2(420f, 78f));
                attackPreviewText.enabled = false;
            }
            attackPreviewText.fontSize = 16;
            attackPreviewText.color = ThemeTextPrimary();
            ConfigureRect(attackPreviewText.rectTransform, new Vector2(0f, 30f), new Vector2(0.5f, 0f), new Vector2(420f, 78f));

            if (resultText == null)
            {
                resultText = CreateText("ResultText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 34, new Vector2(980f, 430f));
                resultText.enabled = false;
            }
            resultText.color = ThemeTextPrimary();
            ConfigureRect(resultText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(700f, 320f));

            if (stageIntroText == null)
            {
                stageIntroText = CreateText("StageIntroText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 30, new Vector2(940f, 330f));
                stageIntroText.enabled = false;
            }
            stageIntroText.color = ThemeTextPrimary();

            if (stageSelectText == null)
            {
                stageSelectText = CreateText("StageSelectText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 25, new Vector2(1040f, 620f));
                stageSelectText.enabled = false;
            }

            if (titleText == null)
            {
                titleText = CreateText("TitleText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 38, new Vector2(980f, 420f));
                titleText.enabled = false;
            }
        }

        private void EnsureBattleHudObjects()
        {
            if (battleBackdropImage == null)
            {
                battleBackdropImage = CreatePanel("BattleBackdrop", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f), new Color(0.005f, 0.012f, 0.028f, 0.34f));
                battleBackdropImage.transform.SetAsFirstSibling();
            }

            EnsurePanelPair(ref battleStageFrame, ref battleStagePanel, "BattleStage", new Vector2(152f, -88f), new Vector2(152f, -88f), new Vector2(0f, 1f), 280f, 142f, 6f);
            EnsurePanelPair(ref battleObjectiveFrame, ref battleObjectivePanel, "BattleObjective", new Vector2(152f, -270f), new Vector2(152f, -270f), new Vector2(0f, 1f), 280f, 196f, 6f);
            EnsurePanelPair(ref battleSelectedFrame, ref battleSelectedPanel, "BattleSelected", new Vector2(152f, 116f), new Vector2(152f, 116f), new Vector2(0f, 0f), 280f, 172f, 6f);
            EnsurePanelPair(ref battleThreatFrame, ref battleThreatPanel, "BattleThreat", new Vector2(-134f, -38f), new Vector2(-134f, -38f), new Vector2(1f, 1f), 244f, 44f, 5f);
            EnsurePanelPair(ref battleLogFrame, ref battleLogPanel, "BattleLog", new Vector2(-134f, -42f), new Vector2(-134f, -42f), new Vector2(1f, 0.5f), 274f, 154f, 6f);
            EnsurePanelPair(ref battleControlsFrame, ref battleControlsPanel, "BattleControls", new Vector2(-134f, 118f), new Vector2(-134f, 118f), new Vector2(1f, 0f), 274f, 178f, 6f);
            EnsurePanelPair(ref battleAttackPreviewFrame, ref battleAttackPreviewPanel, "BattleAttackPreview", new Vector2(0f, 76f), new Vector2(0f, 76f), new Vector2(0.5f, 0f), 462f, 92f, 6f);
            EnsurePanelPair(ref battleResultFrame, ref battleResultPanel, "BattleResult", Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f), 820f, 400f, 7f);

            SetAttackPreviewPanelVisible(false);
            SetResultPanelVisible(false);
        }

        private void EnsurePanelPair(ref Image frame, ref Image panel, string baseName, Vector2 framePosition, Vector2 panelPosition, Vector2 anchor, float width, float height, float inset)
        {
            var frameSize = new Vector2(width, height);
            var panelSize = new Vector2(width - inset * 2f, height - inset * 2f);
            if (frame == null)
            {
                frame = CreatePanel($"{baseName}Frame", framePosition, anchor, frameSize, ThemePanelBorderGold());
            }
            frame.color = new Color(0.74f, 0.52f, 0.22f, 0.82f);
            ConfigurePanelRect(frame.rectTransform, framePosition, anchor, frameSize);

            if (panel == null)
            {
                panel = CreatePanel(
                    $"{baseName}Panel",
                    panelPosition,
                    anchor,
                    panelSize,
                    ThemePanelDark());
            }
            panel.color = new Color(0.024f, 0.04f, 0.064f, 0.82f);
            ConfigurePanelRect(panel.rectTransform, panelPosition, anchor, panelSize);
        }

        private void RefreshStageText()
        {
            if (stageText == null)
            {
                return;
            }

            if (currentStageNumber <= 0 || currentTotalStages <= 0)
            {
                stageText.text = "Stage - / -";
                return;
            }

            if (currentStageData == null)
            {
                stageText.text = $"Stage {currentStageNumber} / {currentTotalStages}";
                return;
            }

            stageText.text =
                $"Stage {currentStageNumber} / {currentTotalStages}\n" +
                $"{currentStageData.DisplayName}\n" +
                $"Difficulty: {currentStageData.DifficultyLabel}";
        }

        private string BuildObjectiveText(StageData data, bool includeDefeatCondition)
        {
            if (data == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            if (includeDefeatCondition)
            {
                builder.AppendLine("<color=#FFD98F>Theme</color>");
                builder.AppendLine(WrapText(data.ThemeName, 24));
                builder.AppendLine();
            }

            builder.AppendLine(data.VictoryCondition == VictoryConditionType.ReachGoal
                ? "<color=#FFD98F>Objective</color>\nReach a goal tile"
                : "<color=#FFD98F>Objective</color>\nDefeat all enemies");

            if (includeDefeatCondition)
            {
                builder.AppendLine();
                builder.AppendLine("<color=#FFD98F>Tip</color>");
                builder.AppendLine(BuildBattleTipText(data));
            }

            if (includeDefeatCondition && data.DefeatConditions != null && data.DefeatConditions.Contains(DefeatConditionType.AllPlayersDefeated))
            {
                builder.AppendLine();
                builder.AppendLine("<color=#FFD98F>Defeat</color>");
                builder.AppendLine("All players defeated");
            }

            if (includeDefeatCondition)
            {
            builder.AppendLine();
            builder.AppendLine("<color=#FFD98F>Limit</color>");
            builder.Append(BuildLimitText(data));
            }

            return builder.ToString().TrimEnd();
        }

        private string BuildBattleTipText(StageData data)
        {
            if (data == null || string.IsNullOrEmpty(data.Description))
            {
                return "Plan the order carefully.";
            }

            var firstSentenceEnd = data.Description.IndexOf('.');
            var tip = firstSentenceEnd >= 0 ? data.Description.Substring(0, firstSentenceEnd + 1) : data.Description;
            if (tip.Length > 48)
            {
                tip = tip.Substring(0, 45).TrimEnd() + "...";
            }

            return WrapText(tip, 28);
        }

        private string BuildResultText(string result)
        {
            if (string.IsNullOrEmpty(result) || currentStageData == null || result.StartsWith("ALL CLEAR"))
            {
                return result;
            }

            var builder = new StringBuilder();
            builder.AppendLine($"<color=#FFD98F>{result}</color>");
            builder.AppendLine();
            builder.AppendLine("<color=#FFD98F>Result Summary</color>");
            builder.AppendLine($"Stage: {currentStageNumber} / {currentTotalStages} - {currentStageData.DisplayName}");
            builder.AppendLine($"Theme: {currentStageData.ThemeName}");
            builder.AppendLine($"Difficulty: {currentStageData.DifficultyLabel}");
            builder.AppendLine(currentStageData.VictoryCondition == VictoryConditionType.ReachGoal
                ? "Objective: Reach a goal tile"
                : "Objective: Defeat all enemies");
            builder.Append($"Limit: {BuildLimitText(currentStageData)}");
            return builder.ToString();
        }

        private string CleanPredictionText(string prediction)
        {
            if (string.IsNullOrEmpty(prediction))
            {
                return "None";
            }

            return prediction
                .Replace("Prediction:", string.Empty)
                .Replace("Prediction", string.Empty)
                .Trim();
        }

        private string BuildStageSelectDetailText(StageData stage, int stageNumber, int totalStages)
        {
            if (stage == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            builder.AppendLine("<color=#FFD98F>SELECTED STAGE</color>");
            builder.AppendLine($"{stageNumber} / {totalStages}");
            builder.AppendLine(stage.DisplayName);
            builder.AppendLine();
            builder.AppendLine("<color=#FFD98F>Theme</color>");
            builder.AppendLine(WrapText(stage.ThemeName, 30));
            builder.AppendLine();
            builder.AppendLine($"<color=#FFD98F>Difficulty</color>  {GetDifficultyRichText(stage.DifficultyLabel)}");
            builder.AppendLine();
            builder.AppendLine("<color=#FFD98F>Description</color>");
            builder.Append(WrapText(stage.Description, 32));
            return builder.ToString();
        }

        private string BuildStageDataLabelText()
        {
            return "<color=#FFD98F>Stage Data</color>\nGrid\nObjective\nLimit\nForces\nTerrain";
        }

        private string BuildStageDataValueText(StageData stage)
        {
            if (stage == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine($"{stage.Width} x {stage.Height}");
            builder.AppendLine(stage.VictoryCondition == VictoryConditionType.ReachGoal
                ? "Reach a goal tile"
                : "Defeat all enemies");
            builder.AppendLine(BuildLimitText(stage));
            builder.AppendLine(BuildStageForceValue(stage));
            builder.Append(BuildStageTerrainValue(stage));
            return builder.ToString();
        }

        private string BuildResultText(
            string result,
            string rating,
            int turnNumber,
            string turnLimit,
            int playersAlive,
            int playerHpTotal,
            int enemiesAlive,
            int enemyHpTotal)
        {
            var builder = new StringBuilder();
            builder.AppendLine(BuildResultText(result));
            builder.AppendLine();
            builder.AppendLine($"Rating     {rating}");
            builder.AppendLine($"Turn       {turnNumber} / {turnLimit}");
            builder.AppendLine($"Survivors  {playersAlive}");
            builder.AppendLine($"HP Total   {playerHpTotal}");
            builder.AppendLine($"Enemies    {enemiesAlive}");
            builder.Append($"Enemy HP   {enemyHpTotal}");
            return builder.ToString();
        }

        private void UpdateSessionBestResult(string result, string rating, int turnNumber, int playersAlive, int playerHpTotal)
        {
            if (string.IsNullOrEmpty(result) || !result.StartsWith("VICTORY") || currentStageNumber <= 0)
            {
                return;
            }

            var candidate = new StageBestResult
            {
                Rating = rating,
                TurnNumber = turnNumber,
                Survivors = playersAlive,
                HpTotal = playerHpTotal
            };

            if (!sessionBestResults.TryGetValue(currentStageNumber, out var currentBest) || IsBetterStageResult(candidate, currentBest))
            {
                sessionBestResults[currentStageNumber] = candidate;
            }
        }

        private bool IsBetterStageResult(StageBestResult candidate, StageBestResult currentBest)
        {
            var candidateRatingScore = GetRatingScore(candidate.Rating);
            var currentRatingScore = GetRatingScore(currentBest.Rating);

            if (candidateRatingScore != currentRatingScore)
            {
                return candidateRatingScore > currentRatingScore;
            }

            if (candidate.TurnNumber != currentBest.TurnNumber)
            {
                return candidate.TurnNumber < currentBest.TurnNumber;
            }

            if (candidate.Survivors != currentBest.Survivors)
            {
                return candidate.Survivors > currentBest.Survivors;
            }

            return candidate.HpTotal > currentBest.HpTotal;
        }

        private int GetRatingScore(string rating)
        {
            switch (rating)
            {
                case "S":
                    return 5;
                case "A":
                    return 4;
                case "B":
                    return 3;
                case "C":
                    return 2;
                case "D":
                    return 1;
                default:
                    return 0;
            }
        }

        private string BuildStageBestResultText(int stageNumber)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<color=#FFD98F>Current Session Best</color>");

            if (!sessionBestResults.TryGetValue(stageNumber, out var bestResult))
            {
                builder.Append("<color=#AEB7C2>No Record Yet</color>");
                return builder.ToString();
            }

            builder.AppendLine($"Rating     {bestResult.Rating}");
            builder.AppendLine($"Turn       {bestResult.TurnNumber}");
            builder.AppendLine($"Survivors  {bestResult.Survivors}");
            builder.Append($"HP Total   {bestResult.HpTotal}");
            return builder.ToString();
        }

        private void AppendDescriptionLine(StringBuilder builder, string description, int wrapLength = 40)
        {
            if (!string.IsNullOrEmpty(description))
            {
                builder.AppendLine(WrapText(description, wrapLength));
            }
        }

        private string BuildLimitText(StageData data)
        {
            return data != null &&
                data.DefeatConditions != null &&
                data.DefeatConditions.Contains(DefeatConditionType.TurnLimitExceeded) &&
                data.TurnLimit > 0
                    ? $"{data.TurnLimit} Turns"
                    : "None";
        }

        private IEnumerator HideStageIntroAfterDelay()
        {
            yield return new WaitForSeconds(StageIntroDuration);
            stageIntroText.text = string.Empty;
            stageIntroText.enabled = false;
            stageIntroCoroutine = null;
        }

        private void SetControlsInfo()
        {
            EnsureTextObjects();
            controlsText.text = "Controls";
            controlsInputText.text =
                "Click Ally\n" +
                "Hover Enemy\n" +
                "Click Enemy\n" +
                "Click Blue\n" +
                "W\n" +
                "U\n" +
                "R\n" +
                "Esc";
            controlsActionText.text =
                "Select\n" +
                "Preview\n" +
                "Info / Attack\n" +
                "Move\n" +
                "Wait / Confirm\n" +
                "Undo Move\n" +
                "Restart Stage\n" +
                "Stage Select";
        }

        private void LogControlsOnce()
        {
            if (controlsLogged)
            {
                return;
            }

            controlsLogged = true;
            Debug.Log("SRPG Prototype Controls:\nClick Ally: Select\nHover Enemy: Preview\nClick Enemy: Info / Attack\nClick Blue Tile: Move\nW: Wait / Confirm\nU: Undo Move\nR: Restart Stage\nEsc/S: Stage Select\nSpace: Toggle Threat / Selected Enemy Range\nEnter: Start/Next/Retry");
        }

        private string BuildStageForceSummary(StageData data)
        {
            if (data == null || data.Units == null)
            {
                return "Forces     Players 0 / Enemies 0";
            }

            var playerCount = 0;
            var enemyCount = 0;
            foreach (var unit in data.Units)
            {
                if (unit == null)
                {
                    continue;
                }

                if (unit.Faction == Faction.Player)
                {
                    playerCount++;
                    continue;
                }

                if (unit.Faction == Faction.Enemy)
                {
                    enemyCount++;
                }
            }

            return $"Forces     Players {playerCount} / Enemies {enemyCount}";
        }

        private string BuildStageForceValue(StageData data)
        {
            if (data == null || data.Units == null)
            {
                return "Players 0 / Enemies 0";
            }

            var playerCount = 0;
            var enemyCount = 0;
            foreach (var unit in data.Units)
            {
                if (unit == null)
                {
                    continue;
                }

                if (unit.Faction == Faction.Player)
                {
                    playerCount++;
                    continue;
                }

                if (unit.Faction == Faction.Enemy)
                {
                    enemyCount++;
                }
            }

            return $"Players {playerCount} / Enemies {enemyCount}";
        }

        private string BuildStageTerrainSummary(StageData data)
        {
            if (data == null || data.Terrains == null)
            {
                return "Terrain    Obstacles 0 / Goals 0";
            }

            var obstacleCount = 0;
            var goalCount = 0;
            foreach (var terrain in data.Terrains)
            {
                if (terrain == null)
                {
                    continue;
                }

                if (terrain.TerrainType == TileTerrainType.Obstacle)
                {
                    obstacleCount++;
                    continue;
                }

                if (terrain.TerrainType == TileTerrainType.Goal)
                {
                    goalCount++;
                }
            }

            return $"Terrain    Obstacles {obstacleCount} / Goals {goalCount}";
        }

        private string BuildStageTerrainValue(StageData data)
        {
            if (data == null || data.Terrains == null)
            {
                return "Obstacles 0 / Goals 0";
            }

            var obstacleCount = 0;
            var goalCount = 0;
            foreach (var terrain in data.Terrains)
            {
                if (terrain == null)
                {
                    continue;
                }

                if (terrain.TerrainType == TileTerrainType.Obstacle)
                {
                    obstacleCount++;
                    continue;
                }

                if (terrain.TerrainType == TileTerrainType.Goal)
                {
                    goalCount++;
                }
            }

            return $"Obstacles {obstacleCount} / Goals {goalCount}";
        }

        private string WrapText(string text, int maxLineLength)
        {
            if (string.IsNullOrEmpty(text) || maxLineLength <= 0)
            {
                return string.Empty;
            }

            var words = text.Split(' ');
            var builder = new StringBuilder();
            var lineLength = 0;

            foreach (var word in words)
            {
                if (lineLength > 0 && lineLength + word.Length + 1 > maxLineLength)
                {
                    builder.AppendLine();
                    lineLength = 0;
                }

                if (lineLength > 0)
                {
                    builder.Append(' ');
                    lineLength++;
                }

                builder.Append(word);
                lineLength += word.Length;
            }

            return builder.ToString();
        }

        private void RefreshBattleLogText()
        {
            if (battleLogText == null)
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("<color=#FFD98F>Battle Log</color>");
            if (battleLogEntries.Count == 0)
            {
                builder.Append("<color=#AEB7C2>No recent actions.</color>");
            }
            else
            {
                foreach (var entry in battleLogEntries)
                {
                    builder.AppendLine(WrapText(entry, 30));
                }
            }

            battleLogText.text = builder.ToString().TrimEnd();
        }

        private Text CreateText(string objectName, Vector2 anchoredPosition, Vector2 anchor, TextAnchor alignment, int fontSize, Vector2 size)
        {
            var textObject = new GameObject(objectName, typeof(RectTransform));
            textObject.transform.SetParent(transform, false);

            var text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.raycastTarget = false;

            var rectTransform = text.rectTransform;
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            return text;
        }

        private void ConfigureRect(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 anchor, Vector2 size)
        {
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private void ConfigurePanelRect(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 anchor, Vector2 size)
        {
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private Image CreatePanel(string objectName, Vector2 anchoredPosition, Vector2 anchor, Vector2 size, Color color)
        {
            var panelObject = new GameObject(objectName, typeof(RectTransform));
            panelObject.transform.SetParent(transform, false);

            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            var rectTransform = image.rectTransform;
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            return image;
        }

        private void EnsureStageSelectObjects()
        {
            if (stageSelectBackdropImage == null)
            {
                stageSelectBackdropImage = CreatePanel("StageSelectBackdrop", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f), ThemeBackgroundDark());
                stageSelectBackdropImage.transform.SetAsFirstSibling();
            }

            if (stageSelectCenterGlowImage == null)
            {
                stageSelectCenterGlowImage = CreatePanel("StageSelectCenterGlow", new Vector2(0f, -44f), new Vector2(0.5f, 0.5f), new Vector2(760f, 620f), new Color(0.04f, 0.09f, 0.15f, 0.32f));
            }

            if (stageSelectTopInfoFrame == null)
            {
                stageSelectTopInfoFrame = CreatePanel("StageSelectTopInfoFrame", new Vector2(36f, -24f), new Vector2(0f, 1f), new Vector2(354f, 104f), ThemePanelBorderGold());
            }

            if (stageSelectTopInfoPanel == null)
            {
                stageSelectTopInfoPanel = CreatePanel("StageSelectTopInfoPanel", new Vector2(42f, -30f), new Vector2(0f, 1f), new Vector2(342f, 92f), ThemePanelDark());
            }

            if (stageSelectListFrame == null)
            {
                stageSelectListFrame = CreatePanel("StageSelectListFrame", new Vector2(58f, -142f), new Vector2(0f, 1f), new Vector2(510f, 448f), ThemePanelBorderGold());
            }

            if (stageSelectListPanel == null)
            {
                stageSelectListPanel = CreatePanel("StageSelectListPanel", new Vector2(64f, -148f), new Vector2(0f, 1f), new Vector2(498f, 436f), ThemePanelDark());
            }

            if (stageSelectDetailFrame == null)
            {
                stageSelectDetailFrame = CreatePanel("StageSelectDetailFrame", new Vector2(586f, -142f), new Vector2(0f, 1f), new Vector2(630f, 448f), ThemePanelBorderGold());
            }

            if (stageSelectDetailPanel == null)
            {
                stageSelectDetailPanel = CreatePanel("StageSelectDetailPanel", new Vector2(592f, -148f), new Vector2(0f, 1f), new Vector2(618f, 436f), ThemePanelDark());
            }

            if (stageSelectFooterFrame == null)
            {
                stageSelectFooterFrame = CreatePanel("StageSelectFooterFrame", new Vector2(0f, 20f), new Vector2(0.5f, 0f), new Vector2(1160f, 76f), ThemePanelBorderGold());
            }

            if (stageSelectFooterPanel == null)
            {
                stageSelectFooterPanel = CreatePanel("StageSelectFooterPanel", new Vector2(0f, 26f), new Vector2(0.5f, 0f), new Vector2(1148f, 64f), ThemePanelDark());
            }

            if (stageSelectHeaderText == null)
            {
                stageSelectHeaderText = CreateText("StageSelectHeaderText", new Vector2(0f, -28f), new Vector2(0.5f, 1f), TextAnchor.MiddleCenter, 50, new Vector2(700f, 78f));
                stageSelectHeaderText.color = ThemeTextAccentGold();
            }

            if (stageSelectTopInfoText == null)
            {
                stageSelectTopInfoText = CreateText("StageSelectTopInfoText", new Vector2(64f, -38f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 24, new Vector2(410f, 86f));
                stageSelectTopInfoText.color = new Color(1f, 0.86f, 0.58f, 1f);
            }

            if (stageSelectListText == null)
            {
                stageSelectListText = CreateText("StageSelectListText", new Vector2(64f, -166f), new Vector2(0f, 1f), TextAnchor.MiddleCenter, 25, new Vector2(498f, 42f));
                stageSelectListText.color = new Color(1f, 0.82f, 0.45f, 1f);
            }

            if (stageSelectDetailText == null)
            {
                stageSelectDetailText = CreateText("StageSelectDetailText", new Vector2(620f, -168f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 17, new Vector2(350f, 260f));
            }
            stageSelectDetailText.fontSize = 17;
            ConfigureRect(stageSelectDetailText.rectTransform, new Vector2(620f, -168f), new Vector2(0f, 1f), new Vector2(350f, 260f));

            if (stageSelectDataLabelText == null)
            {
                stageSelectDataLabelText = CreateText("StageSelectDataLabelText", new Vector2(620f, -440f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 17, new Vector2(88f, 128f));
            }
            stageSelectDataLabelText.fontSize = 17;
            stageSelectDataLabelText.color = new Color(1f, 0.82f, 0.45f, 1f);
            ConfigureRect(stageSelectDataLabelText.rectTransform, new Vector2(620f, -440f), new Vector2(0f, 1f), new Vector2(88f, 128f));

            if (stageSelectDataValueText == null)
            {
                stageSelectDataValueText = CreateText("StageSelectDataValueText", new Vector2(732f, -440f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 17, new Vector2(236f, 128f));
            }
            stageSelectDataValueText.fontSize = 17;
            stageSelectDataValueText.color = Color.white;
            ConfigureRect(stageSelectDataValueText.rectTransform, new Vector2(732f, -440f), new Vector2(0f, 1f), new Vector2(236f, 128f));

            if (stageSelectFooterText == null)
            {
                stageSelectFooterText = CreateText("StageSelectFooterText", new Vector2(0f, 45f), new Vector2(0.5f, 0f), TextAnchor.MiddleCenter, 21, new Vector2(1100f, 36f));
                stageSelectFooterText.color = new Color(1f, 0.88f, 0.64f, 1f);
            }

            if (stageSelectMapPreviewFrame == null)
            {
                stageSelectMapPreviewFrame = CreatePanel("StageSelectMapPreviewFrame", new Vector2(976f, -192f), new Vector2(0f, 1f), new Vector2(206f, 244f), new Color(0.74f, 0.52f, 0.22f, 0.72f));
            }

            if (stageSelectMapPreviewPanel == null)
            {
                stageSelectMapPreviewPanel = CreatePanel("StageSelectMapPreviewPanel", new Vector2(982f, -198f), new Vector2(0f, 1f), new Vector2(194f, 232f), new Color(0.01f, 0.025f, 0.04f, 0.94f));
            }

            if (stageSelectMapPreviewText == null)
            {
                stageSelectMapPreviewText = CreateText("StageSelectMapPreviewText", new Vector2(998f, -212f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 17, new Vector2(160f, 28f));
                stageSelectMapPreviewText.color = new Color(1f, 0.82f, 0.45f, 1f);
                stageSelectMapPreviewText.text = "MAP PREVIEW";
            }

            if (stageSelectMapPreviewLegendText == null)
            {
                stageSelectMapPreviewLegendText = CreateText("StageSelectMapPreviewLegendText", new Vector2(990f, -446f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 12, new Vector2(190f, 42f));
                stageSelectMapPreviewLegendText.color = new Color(0.84f, 0.9f, 0.96f, 1f);
                stageSelectMapPreviewLegendText.text = string.Empty;
            }
            stageSelectMapPreviewLegendText.fontSize = 12;
            stageSelectMapPreviewLegendText.color = new Color(0.84f, 0.9f, 0.96f, 1f);
            ConfigureRect(stageSelectMapPreviewLegendText.rectTransform, new Vector2(990f, -446f), new Vector2(0f, 1f), new Vector2(190f, 42f));

            if (stageSelectBestText == null)
            {
                stageSelectBestText = CreateText("StageSelectBestText", new Vector2(990f, -494f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 15, new Vector2(190f, 82f));
            }
            stageSelectBestText.fontSize = 15;
            stageSelectBestText.color = new Color(0.92f, 0.96f, 1f, 1f);
            ConfigureRect(stageSelectBestText.rectTransform, new Vector2(990f, -494f), new Vector2(0f, 1f), new Vector2(190f, 82f));

            EnsureStageSelectRows();
            EnsureStagePreviewCells();

            if (!stageSelectObjectsInitialized)
            {
                stageSelectObjectsInitialized = true;
                SetStageSelectVisible(false);
            }
        }

        private void EnsureStageSelectRows()
        {
            const float rowStartY = -220f;
            const float rowSpacing = 52f;

            for (var i = 0; i < MaxStageSelectRows; i++)
            {
                var rowY = rowStartY - rowSpacing * i;

                if (stageSelectRowPanels[i] == null)
                {
                    stageSelectRowPanels[i] = CreatePanel($"StageSelectRowPanel_{i + 1}", new Vector2(86f, rowY), new Vector2(0f, 1f), new Vector2(438f, 42f), new Color(0f, 0f, 0f, 0f));
                }

                if (stageSelectRowNumberTexts[i] == null)
                {
                    stageSelectRowNumberTexts[i] = CreateText($"StageSelectRowNumberText_{i + 1}", new Vector2(102f, rowY - 5f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 21, new Vector2(58f, 32f));
                    stageSelectRowNumberTexts[i].color = new Color(1f, 0.86f, 0.58f, 1f);
                }

                if (stageSelectRowNameTexts[i] == null)
                {
                    stageSelectRowNameTexts[i] = CreateText($"StageSelectRowNameText_{i + 1}", new Vector2(154f, rowY - 5f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 21, new Vector2(270f, 32f));
                }

                if (stageSelectRowDifficultyTexts[i] == null)
                {
                    stageSelectRowDifficultyTexts[i] = CreateText($"StageSelectRowDifficultyText_{i + 1}", new Vector2(426f, rowY - 5f), new Vector2(0f, 1f), TextAnchor.UpperRight, 19, new Vector2(92f, 32f));
                    stageSelectRowDifficultyTexts[i].color = new Color(0.82f, 0.78f, 1f, 1f);
                }
            }
        }

        private void EnsureStagePreviewCells()
        {
            const float cellSize = 15f;
            const float gap = 3f;
            const float startX = 1010f;
            const float startY = -254f;

            for (var y = 0; y < StagePreviewSize; y++)
            {
                for (var x = 0; x < StagePreviewSize; x++)
                {
                    var index = y * StagePreviewSize + x;
                    if (stageSelectPreviewCells[index] != null)
                    {
                        ConfigureRect(
                            stageSelectPreviewCells[index].rectTransform,
                            new Vector2(startX + (cellSize + gap) * x, startY - (cellSize + gap) * y),
                            new Vector2(0f, 1f),
                            new Vector2(cellSize, cellSize));
                        continue;
                    }

                    stageSelectPreviewCells[index] = CreatePanel(
                        $"StageSelectPreviewCell_{x}_{y}",
                        new Vector2(startX + (cellSize + gap) * x, startY - (cellSize + gap) * y),
                        new Vector2(0f, 1f),
                        new Vector2(cellSize, cellSize),
                        GetPreviewNormalColor());
                }
            }
        }

        private void RefreshStagePreview(StageData stage)
        {
            for (var i = 0; i < stageSelectPreviewCells.Length; i++)
            {
                if (stageSelectPreviewCells[i] != null)
                {
                    stageSelectPreviewCells[i].color = GetPreviewNormalColor();
                }
            }

            if (stage == null)
            {
                return;
            }

            if (stage.Terrains != null)
            {
                foreach (var terrain in stage.Terrains)
                {
                    if (terrain == null || !IsInsidePreview(terrain.Position))
                    {
                        continue;
                    }

                    var cell = GetPreviewCell(terrain.Position);
                    if (cell == null)
                    {
                        continue;
                    }

                    if (terrain.TerrainType == TileTerrainType.Obstacle)
                    {
                        cell.color = new Color(0.02f, 0.025f, 0.03f, 1f);
                    }
                    else if (terrain.TerrainType == TileTerrainType.Goal)
                    {
                        cell.color = new Color(0.05f, 0.72f, 0.62f, 1f);
                    }
                }
            }

            if (stage.Units == null)
            {
                return;
            }

            foreach (var unit in stage.Units)
            {
                if (unit == null || !IsInsidePreview(unit.Position))
                {
                    continue;
                }

                var cell = GetPreviewCell(unit.Position);
                if (cell == null)
                {
                    continue;
                }

                cell.color = unit.Faction == Faction.Player
                    ? new Color(0.2f, 0.54f, 1f, 1f)
                    : new Color(1f, 0.22f, 0.22f, 1f);
            }
        }

        private Image GetPreviewCell(Vector2Int position)
        {
            if (!IsInsidePreview(position))
            {
                return null;
            }

            var displayY = StagePreviewSize - 1 - position.y;
            return stageSelectPreviewCells[displayY * StagePreviewSize + position.x];
        }

        private bool IsInsidePreview(Vector2Int position)
        {
            return position.x >= 0 && position.x < StagePreviewSize && position.y >= 0 && position.y < StagePreviewSize;
        }

        private Color GetPreviewNormalColor()
        {
            return new Color(0.18f, 0.2f, 0.22f, 1f);
        }

        private void RefreshStageSelectRows(IReadOnlyList<StageData> stages, int selectedStageIndex)
        {
            selectedStageSelectRow = selectedStageIndex;

            for (var i = 0; i < MaxStageSelectRows; i++)
            {
                var hasStage = stages != null && i < stages.Count && stages[i] != null;
                SetImageEnabled(stageSelectRowPanels[i], hasStage);
                SetTextEnabled(stageSelectRowNumberTexts[i], hasStage);
                SetTextEnabled(stageSelectRowNameTexts[i], hasStage);
                SetTextEnabled(stageSelectRowDifficultyTexts[i], hasStage);

                if (!hasStage)
                {
                    continue;
                }

                var stage = stages[i];
                var isSelected = i == selectedStageIndex;
                stageSelectRowPanels[i].color = isSelected
                    ? GetStageSelectSelectedRowColor()
                    : new Color(0.012f, 0.043f, 0.072f, 0.56f);
                stageSelectRowNumberTexts[i].text = isSelected ? $"\u25C6 {i + 1}" : $"{i + 1}";
                stageSelectRowNameTexts[i].text = stage.DisplayName;
                stageSelectRowDifficultyTexts[i].text = $"[{stage.DifficultyLabel}]";
                stageSelectRowNumberTexts[i].color = isSelected ? new Color(1f, 0.92f, 0.58f, 1f) : new Color(1f, 0.86f, 0.58f, 1f);
                stageSelectRowNameTexts[i].color = isSelected ? new Color(1f, 0.94f, 0.76f, 1f) : new Color(0.94f, 0.96f, 1f, 1f);
                stageSelectRowDifficultyTexts[i].color = GetDifficultyColor(stage.DifficultyLabel, isSelected);
            }
        }

        private void AnimateStageSelectSelection()
        {
            if (selectedStageSelectRow < 0 ||
                selectedStageSelectRow >= stageSelectRowPanels.Length ||
                stageSelectRowPanels[selectedStageSelectRow] == null ||
                !stageSelectRowPanels[selectedStageSelectRow].enabled)
            {
                return;
            }

            stageSelectRowPanels[selectedStageSelectRow].color = GetStageSelectSelectedRowColor();
        }

        private Color GetStageSelectSelectedRowColor()
        {
            stageSelectPulseTimer += Time.deltaTime;
            var pulse = ((float)System.Math.Sin(stageSelectPulseTimer * 2.6f) + 1f) * 0.5f;
            return new Color(
                0.08f + (0.16f - 0.08f) * pulse,
                0.3f + (0.48f - 0.3f) * pulse,
                0.56f + (0.84f - 0.56f) * pulse,
                0.88f + (0.96f - 0.88f) * pulse);
        }

        private string GetDifficultyRichText(string difficulty)
        {
            return $"<color={GetDifficultyHexColor(difficulty)}>{difficulty}</color>";
        }

        private Color GetDifficultyColor(string difficulty, bool selected)
        {
            switch (difficulty)
            {
                case "Intro":
                    return selected ? new Color(0.72f, 0.92f, 1f, 1f) : new Color(0.55f, 0.82f, 1f, 1f);
                case "Easy+":
                    return selected ? new Color(0.68f, 1f, 0.62f, 1f) : new Color(0.42f, 0.86f, 0.48f, 1f);
                case "Medium":
                    return selected ? new Color(1f, 0.94f, 0.62f, 1f) : new Color(0.96f, 0.9f, 0.58f, 1f);
                case "Hard":
                    return selected ? new Color(1f, 0.68f, 0.36f, 1f) : new Color(0.95f, 0.52f, 0.24f, 1f);
                case "Hard+":
                    return selected ? new Color(1f, 0.42f, 0.42f, 1f) : new Color(0.96f, 0.28f, 0.32f, 1f);
                case "Final":
                    return selected ? new Color(0.9f, 0.62f, 1f, 1f) : new Color(0.72f, 0.46f, 1f, 1f);
                default:
                    return selected ? new Color(1f, 0.88f, 0.58f, 1f) : Color.white;
            }
        }

        private string GetDifficultyHexColor(string difficulty)
        {
            switch (difficulty)
            {
                case "Intro":
                    return "#8CD1FF";
                case "Easy+":
                    return "#6EDB7A";
                case "Medium":
                    return "#F5E694";
                case "Hard":
                    return "#F2853D";
                case "Hard+":
                    return "#F54852";
                case "Final":
                    return "#B875FF";
                default:
                    return "#FFFFFF";
            }
        }

        private void SetStageSelectVisible(bool visible)
        {
            SetImageEnabled(stageSelectBackdropImage, visible);
            SetImageEnabled(stageSelectCenterGlowImage, visible);
            SetImageEnabled(stageSelectTopInfoFrame, visible);
            SetImageEnabled(stageSelectTopInfoPanel, visible);
            SetImageEnabled(stageSelectListFrame, visible);
            SetImageEnabled(stageSelectListPanel, visible);
            SetImageEnabled(stageSelectDetailFrame, visible);
            SetImageEnabled(stageSelectDetailPanel, visible);
            SetImageEnabled(stageSelectFooterFrame, visible);
            SetImageEnabled(stageSelectFooterPanel, visible);
            SetTextEnabled(stageSelectHeaderText, visible);
            SetTextEnabled(stageSelectTopInfoText, visible);
            SetTextEnabled(stageSelectListText, visible);
            SetTextEnabled(stageSelectDetailText, visible);
            SetTextEnabled(stageSelectDataLabelText, visible);
            SetTextEnabled(stageSelectDataValueText, visible);
            SetTextEnabled(stageSelectBestText, visible);
            SetTextEnabled(stageSelectFooterText, visible);
            SetImageEnabled(stageSelectMapPreviewFrame, visible);
            SetImageEnabled(stageSelectMapPreviewPanel, visible);
            SetTextEnabled(stageSelectMapPreviewText, visible);
            SetTextEnabled(stageSelectMapPreviewLegendText, visible);

            for (var i = 0; i < MaxStageSelectRows; i++)
            {
                SetImageEnabled(stageSelectRowPanels[i], visible);
                SetTextEnabled(stageSelectRowNumberTexts[i], visible);
                SetTextEnabled(stageSelectRowNameTexts[i], visible);
                SetTextEnabled(stageSelectRowDifficultyTexts[i], visible);
            }

            for (var i = 0; i < stageSelectPreviewCells.Length; i++)
            {
                SetImageEnabled(stageSelectPreviewCells[i], visible);
            }
        }

        private void SetImageEnabled(Image image, bool visible)
        {
            if (image != null)
            {
                image.enabled = visible;
            }
        }

        private void SetTextEnabled(Text text, bool visible)
        {
            if (text != null)
            {
                text.enabled = visible;
            }
        }

        private void SetBattleHudVisible(bool visible)
        {
            SetImageEnabled(battleBackdropImage, visible);
            SetImageEnabled(battleStageFrame, visible);
            SetImageEnabled(battleStagePanel, visible);
            SetImageEnabled(battleObjectiveFrame, visible);
            SetImageEnabled(battleObjectivePanel, visible);
            SetImageEnabled(battleSelectedFrame, visible);
            SetImageEnabled(battleSelectedPanel, visible);
            SetImageEnabled(battleThreatFrame, visible);
            SetImageEnabled(battleThreatPanel, visible);
            SetImageEnabled(battleLogFrame, visible);
            SetImageEnabled(battleLogPanel, visible);
            SetImageEnabled(battleControlsFrame, visible);
            SetImageEnabled(battleControlsPanel, visible);
            SetTextEnabled(turnText, visible);
            SetTextEnabled(stageText, visible);
            SetTextEnabled(objectiveText, visible);
            SetTextEnabled(selectedUnitText, visible);
            SetTextEnabled(enemyThreatText, visible);
            SetTextEnabled(controlsText, visible);
            SetTextEnabled(controlsInputText, visible);
            SetTextEnabled(controlsActionText, visible);
            SetTextEnabled(battleLogText, visible);
            if (!visible)
            {
                SetTextEnabled(attackPreviewText, false);
                SetAttackPreviewPanelVisible(false);
                SetResultPanelVisible(false);
            }
        }

        private void SetAttackPreviewPanelVisible(bool visible)
        {
            SetImageEnabled(battleAttackPreviewFrame, visible);
            SetImageEnabled(battleAttackPreviewPanel, visible);
        }

        private void SetResultPanelVisible(bool visible)
        {
            SetImageEnabled(battleResultFrame, visible);
            SetImageEnabled(battleResultPanel, visible);
        }

        private void EnsureTitleOverlayObjects()
        {
            if (titleDimOverlayImage == null)
            {
                titleDimOverlayImage = CreatePanel("TitleDimOverlay", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f), new Color(0f, 0.01f, 0.025f, 0.34f));
            }
            titleDimOverlayImage.color = new Color(0f, 0.01f, 0.025f, 0.08f);

            if (titleMenuFrame == null)
            {
                titleMenuFrame = CreatePanel("MenuPanel", new Vector2(0f, -93f), new Vector2(0.5f, 0.5f), new Vector2(348f, 140f), new Color(0.74f, 0.52f, 0.22f, 0.88f));
            }
            titleMenuFrame.color = new Color(0.74f, 0.52f, 0.22f, 0.88f);
            ConfigureRect(titleMenuFrame.rectTransform, new Vector2(0f, -93f), new Vector2(0.5f, 0.5f), new Vector2(348f, 140f));

            if (titleMenuPanel == null)
            {
                titleMenuPanel = CreatePanel("MenuPanelBackground", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(336f, 128f), new Color(0.031f, 0.063f, 0.094f, 0.7f));
            }
            titleMenuPanel.transform.SetParent(titleMenuFrame.transform, false);
            titleMenuPanel.color = new Color(0.031f, 0.063f, 0.094f, 0.7f);
            ConfigureRect(titleMenuPanel.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(336f, 128f));

            if (titleMenuHighlightPanel == null)
            {
                titleMenuHighlightPanel = CreatePanel("TitleMenuHighlightPanel", new Vector2(0f, 222f), new Vector2(0.5f, 0f), new Vector2(1f, 1f), new Color(0f, 0f, 0f, 0f));
            }
            titleMenuHighlightPanel.color = new Color(0f, 0f, 0f, 0f);
            titleMenuHighlightPanel.enabled = false;

            EnsureTitleMenuItemObjects();

            if (titleFooterFrame == null)
            {
                titleFooterFrame = CreatePanel("BottomControlBar", new Vector2(0f, 52f), new Vector2(0.5f, 0f), new Vector2(960f, 48f), ThemePanelBorderGold());
            }
            titleFooterFrame.color = new Color(0.74f, 0.52f, 0.22f, 0.82f);
            ConfigureRect(titleFooterFrame.rectTransform, new Vector2(0f, 52f), new Vector2(0.5f, 0f), new Vector2(960f, 48f));

            if (titleFooterPanel == null)
            {
                titleFooterPanel = CreatePanel("BottomControlBarBackground", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(948f, 36f), new Color(0.031f, 0.063f, 0.094f, 0.72f));
            }
            titleFooterPanel.transform.SetParent(titleFooterFrame.transform, false);
            titleFooterPanel.color = new Color(0.031f, 0.063f, 0.094f, 0.72f);
            ConfigureRect(titleFooterPanel.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(948f, 36f));

            if (titlePromptText == null)
            {
                titlePromptText = CreateText("ControlText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 20, new Vector2(920f, 34f));
                titlePromptText.color = ThemeTextMutedGold();
            }
            titlePromptText.transform.SetParent(titleFooterFrame.transform, false);
            titlePromptText.fontSize = 20;
            ConfigureRect(titlePromptText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(920f, 34f));

            RefreshTitleMenuText();
            RefreshTitlePromptText(false);
            UpdateTitleHighlightPosition();

            if (!titleOverlayInitialized)
            {
                titleOverlayInitialized = true;
                SetTitleOverlayVisible(false);
            }
        }

        private void EnsureTitleMenuItemObjects()
        {
            if (titleMenuPanel == null)
            {
                return;
            }

            if (titleMenuItemsRootPanel == null)
            {
                titleMenuItemsRootPanel = CreatePanel("MenuItemsRoot", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(336f, 100f), new Color(0f, 0f, 0f, 0f));
            }
            titleMenuItemsRootPanel.transform.SetParent(titleMenuPanel.transform, false);
            titleMenuItemsRootPanel.color = new Color(0f, 0f, 0f, 0f);
            titleMenuItemsRoot = titleMenuItemsRootPanel.rectTransform;
            ConfigureRect(titleMenuItemsRoot, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(336f, 100f));

            titleCursorText = EnsureTitleMenuChildText(titleCursorText, "CursorText", new Vector2(-66f, 36f), TextAnchor.MiddleCenter, 21, new Vector2(30f, 28f));
            titleStartText = EnsureTitleMenuChildText(titleStartText, "StartText", new Vector2(0f, 36f), TextAnchor.MiddleCenter, 21, new Vector2(260f, 28f));
            titleStageSelectText = EnsureTitleMenuChildText(titleStageSelectText, "StageSelectText", new Vector2(0f, 12f), TextAnchor.MiddleCenter, 21, new Vector2(260f, 28f));
            titleOptionsText = EnsureTitleMenuChildText(titleOptionsText, "OptionsText", new Vector2(0f, -12f), TextAnchor.MiddleCenter, 21, new Vector2(260f, 28f));
            titleExitText = EnsureTitleMenuChildText(titleExitText, "ExitText", new Vector2(0f, -36f), TextAnchor.MiddleCenter, 21, new Vector2(260f, 28f));
        }

        private void RefreshOptionsText(AudioManager audioManager)
        {
            if (optionsBodyText == null)
            {
                return;
            }

            var master = audioManager != null ? audioManager.MasterVolume : 0f;
            var bgm = audioManager != null ? audioManager.BgmVolume : 0f;
            var se = audioManager != null ? audioManager.SeVolume : 0f;
            var mute = audioManager != null && audioManager.MuteAll;
            var resolution = $"{Screen.width} x {Screen.height}";
            var displayMode = Screen.fullScreen ? "Full Screen" : "Window";

            var lines = new[]
            {
                BuildOptionLine(0, "Master", FormatVolume(master)),
                BuildOptionLine(1, "BGM", FormatVolume(bgm)),
                BuildOptionLine(2, "SE", FormatVolume(se)),
                BuildOptionLine(3, "Mute", mute ? "ON" : "OFF"),
                BuildOptionLine(4, "Resolution", resolution),
                BuildOptionLine(5, "Display", displayMode),
                BuildOptionLine(6, "Back", "Title")
            };

            optionsBodyText.text = string.Join("\n", lines);
        }

        private string BuildOptionLine(int row, string label, string value)
        {
            var cursor = row == selectedOptionsRow ? "◆" : " ";
            var color = row == selectedOptionsRow ? "#FFD98F" : "#FFFFFF";
            return $"<color=#C99A45>{cursor}</color> <color={color}>{label,-10}</color>  {value}";
        }

        private string FormatVolume(float value)
        {
            var percent = Mathf.RoundToInt(Mathf.Clamp01(value) * 100f);
            var filled = Mathf.RoundToInt(Mathf.Clamp01(value) * 10f);
            var builder = new StringBuilder();
            builder.Append('[');
            for (var i = 0; i < 10; i++)
            {
                builder.Append(i < filled ? '■' : '□');
            }

            builder.Append("] ");
            builder.Append(percent.ToString("D3"));
            builder.Append('%');
            return builder.ToString();
        }

        private void EnsureOptionsObjects()
        {
            if (optionsFrame == null)
            {
                optionsFrame = CreatePanel("OptionsPanel", new Vector2(0f, -76f), new Vector2(0.5f, 0.5f), new Vector2(600f, 386f), ThemePanelBorderGold());
            }
            optionsFrame.color = new Color(0.74f, 0.52f, 0.22f, 0.82f);
            ConfigureRect(optionsFrame.rectTransform, new Vector2(0f, -76f), new Vector2(0.5f, 0.5f), new Vector2(600f, 386f));

            if (optionsPanel == null)
            {
                optionsPanel = CreatePanel("OptionsPanelBackground", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(586f, 372f), ThemePanelDark());
            }
            optionsPanel.transform.SetParent(optionsFrame.transform, false);
            optionsPanel.color = new Color(0.024f, 0.04f, 0.064f, 0.84f);
            ConfigureRect(optionsPanel.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(586f, 372f));

            if (optionsHeaderText == null)
            {
                optionsHeaderText = CreateText("OptionsHeaderText", new Vector2(0f, 150f), new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 28, new Vector2(520f, 44f));
            }
            optionsHeaderText.transform.SetParent(optionsPanel.transform, false);
            optionsHeaderText.fontSize = 28;
            optionsHeaderText.alignment = TextAnchor.MiddleCenter;
            optionsHeaderText.color = ThemeTextAccentGold();
            optionsHeaderText.text = "OPTIONS";
            ConfigureRect(optionsHeaderText.rectTransform, new Vector2(0f, 150f), new Vector2(0.5f, 0.5f), new Vector2(520f, 44f));

            if (optionsBodyText == null)
            {
                optionsBodyText = CreateText("OptionsBodyText", new Vector2(0f, 2f), new Vector2(0.5f, 0.5f), TextAnchor.MiddleLeft, 20, new Vector2(500f, 252f));
            }
            optionsBodyText.transform.SetParent(optionsPanel.transform, false);
            optionsBodyText.fontSize = 20;
            optionsBodyText.alignment = TextAnchor.MiddleLeft;
            optionsBodyText.color = ThemeTextPrimary();
            ConfigureRect(optionsBodyText.rectTransform, new Vector2(0f, 2f), new Vector2(0.5f, 0.5f), new Vector2(500f, 252f));

            if (optionsFooterText == null)
            {
                optionsFooterText = CreateText("OptionsFooterText", new Vector2(0f, -158f), new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 14, new Vector2(540f, 30f));
            }
            optionsFooterText.transform.SetParent(optionsPanel.transform, false);
            optionsFooterText.fontSize = 14;
            optionsFooterText.alignment = TextAnchor.MiddleCenter;
            optionsFooterText.color = ThemeTextMutedGold();
            optionsFooterText.text = "Left / Right: Adjust    Enter: Toggle / Back    Esc: Title";
            ConfigureRect(optionsFooterText.rectTransform, new Vector2(0f, -158f), new Vector2(0.5f, 0.5f), new Vector2(540f, 30f));

            if (!optionsOverlayInitialized)
            {
                optionsOverlayInitialized = true;
                SetOptionsOverlayVisible(false);
            }
        }

        private Text EnsureTitleMenuChildText(Text text, string objectName, Vector2 localPosition, TextAnchor alignment, int fontSize, Vector2 size)
        {
            if (text == null)
            {
                text = CreateText(objectName, localPosition, new Vector2(0.5f, 0.5f), alignment, fontSize, size);
                text.color = ThemeTextMutedGold();
            }

            text.transform.SetParent(titleMenuItemsRoot, false);
            text.fontSize = fontSize;
            text.alignment = alignment;
            ConfigureRect(text.rectTransform, localPosition, new Vector2(0.5f, 0.5f), size);
            return text;
        }

        private void SetTitleOverlayVisible(bool visible)
        {
            SetImageEnabled(titleDimOverlayImage, visible);
            SetImageEnabled(titleMenuFrame, visible);
            SetImageEnabled(titleMenuPanel, visible);
            SetImageEnabled(titleMenuHighlightPanel, false);
            SetTextEnabled(titleCursorText, visible);
            SetTextEnabled(titleStartText, visible);
            SetTextEnabled(titleStageSelectText, visible);
            SetTextEnabled(titleOptionsText, visible);
            SetTextEnabled(titleExitText, visible);
            SetImageEnabled(titleFooterFrame, visible);
            SetImageEnabled(titleFooterPanel, visible);
            SetTextEnabled(titlePromptText, visible);
        }

        private void SetOptionsOverlayVisible(bool visible)
        {
            SetImageEnabled(titleDimOverlayImage, visible);
            SetImageEnabled(optionsFrame, visible);
            SetImageEnabled(optionsPanel, visible);
            SetTextEnabled(optionsHeaderText, visible);
            SetTextEnabled(optionsBodyText, visible);
            SetTextEnabled(optionsFooterText, visible);
            SetImageEnabled(titleFooterFrame, visible);
            SetImageEnabled(titleFooterPanel, visible);
            SetTextEnabled(titlePromptText, visible);

            if (visible && titlePromptText != null)
            {
                RefreshTitlePromptText(true);
            }
        }

        private void RefreshTitlePromptText(bool optionsMode)
        {
            if (titlePromptText == null)
            {
                return;
            }

            titlePromptText.color = ThemeTextMutedGold();
            titlePromptText.alignment = TextAnchor.MiddleCenter;
            titlePromptText.text = optionsMode
                ? "<color=#FFD98F>Left / Right</color>: Adjust      <color=#C99A45>◆</color>      <color=#FFD98F>Enter</color>: Toggle / Back      <color=#C99A45>◆</color>      <color=#FFD98F>Esc</color>: Title"
                : "<color=#FFD98F>Enter</color>: Confirm      <color=#C99A45>◆</color>      <color=#FFD98F>Up / Down</color>: Select      <color=#C99A45>◆</color>      <color=#FFD98F>Esc</color>: Back";
        }

        private void BringTitleObjectsToFront()
        {
            if (titleBackgroundImage != null)
            {
                titleBackgroundImage.transform.SetAsFirstSibling();
            }

            BringToFront(titleDimOverlayImage);
            BringToFront(titleMenuFrame);
            BringToFront(titleMenuPanel);
            BringToFront(titleMenuHighlightPanel);
            BringToFront(titleMenuItemsRootPanel);
            BringToFront(titleCursorText);
            BringToFront(titleStartText);
            BringToFront(titleStageSelectText);
            BringToFront(titleOptionsText);
            BringToFront(titleExitText);
            BringToFront(titleFooterFrame);
            BringToFront(titleFooterPanel);
            if (titlePromptText != null)
            {
                titlePromptText.transform.SetParent(titleFooterFrame.transform, false);
                ConfigureRect(titlePromptText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(920f, 34f));
            }
            BringToFront(titlePromptText);
        }

        private void BringOptionsObjectsToFront()
        {
            if (titleBackgroundImage != null)
            {
                titleBackgroundImage.transform.SetAsFirstSibling();
            }

            BringToFront(titleDimOverlayImage);
            BringToFront(optionsFrame);
            BringToFront(optionsPanel);
            BringToFront(optionsHeaderText);
            BringToFront(optionsBodyText);
            BringToFront(optionsFooterText);
            BringToFront(titleFooterFrame);
            BringToFront(titleFooterPanel);
            if (titlePromptText != null)
            {
                titlePromptText.transform.SetParent(titleFooterFrame.transform, false);
                ConfigureRect(titlePromptText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(920f, 34f));
            }
            BringToFront(titlePromptText);
        }

        private void BringToFront(Component component)
        {
            if (component == null || component.transform == null)
            {
                return;
            }

            var setAsLastSibling = component.transform.GetType().GetMethod("SetAsLastSibling");
            if (setAsLastSibling != null)
            {
                setAsLastSibling.Invoke(component.transform, null);
            }
        }

        private void RefreshTitleMenuText()
        {
            if (titleStartText == null || titleStageSelectText == null || titleOptionsText == null || titleExitText == null || titleCursorText == null)
            {
                return;
            }

            var labels = new[] { "Start", "Stage Select", "Options", "Exit" };
            var texts = new[] { titleStartText, titleStageSelectText, titleOptionsText, titleExitText };
            for (var i = 0; i < texts.Length; i++)
            {
                texts[i].text = labels[i];
                if (i == selectedTitleMenuRow)
                {
                    texts[i].color = new Color(1f, 0.9f, 0.66f, 1f);
                }
                else
                {
                    texts[i].color = ThemeTextMutedGold();
                }
            }

            titleCursorText.text = "◆";
            titleCursorText.color = ThemeTextAccentGold();
        }

        private void UpdateTitleHighlightPosition()
        {
            if (titleCursorText == null)
            {
                return;
            }

            var rowIndex = System.Math.Max(0, System.Math.Min(selectedTitleMenuRow, 3));
            var rowY = 36f - rowIndex * 24f;
            ConfigureRect(titleCursorText.rectTransform, new Vector2(-66f, rowY), new Vector2(0.5f, 0.5f), new Vector2(30f, 28f));
        }

        private void AnimateTitleSelection()
        {
            if (titleCursorText == null || !titleCursorText.enabled)
            {
                return;
            }
            titlePulseTimer += Time.deltaTime;
            var pulse = ((float)System.Math.Sin(titlePulseTimer * 2.2f) + 1f) * 0.5f;
            titleCursorText.color = new Color(1f, 0.78f + 0.14f * pulse, 0.38f + 0.18f * pulse, 1f);
        }

        private Color GetTitleSelectedRowColor()
        {
            titlePulseTimer += Time.deltaTime;
            var pulse = ((float)System.Math.Sin(titlePulseTimer * 2.2f) + 1f) * 0.5f;
            return new Color(
                0.08f + (0.14f - 0.08f) * pulse,
                0.28f + (0.44f - 0.28f) * pulse,
                0.52f + (0.78f - 0.52f) * pulse,
                0.58f + (0.78f - 0.58f) * pulse);
        }

        private void EnsureTitleBackgroundImage()
        {
            if (titleBackgroundImage != null)
            {
                return;
            }

            var imageObject = new GameObject("TitleBackgroundImage", typeof(RectTransform));
            imageObject.transform.SetParent(transform, false);
            imageObject.transform.SetAsFirstSibling();

            titleBackgroundImage = imageObject.AddComponent<Image>();
            titleBackgroundImage.raycastTarget = false;
            titleBackgroundImage.preserveAspect = true;

            var aspectFitter = imageObject.AddComponent<AspectRatioFitter>();
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            aspectFitter.aspectRatio = 16f / 9f;

            var rectTransform = titleBackgroundImage.rectTransform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            var texture = Resources.Load<Texture2D>("Title/TitleBackground");
            if (texture != null)
            {
                titleBackgroundImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
            }

            titleBackgroundImage.enabled = false;
        }

        private struct StageBestResult
        {
            public string Rating;
            public int TurnNumber;
            public int Survivors;
            public int HpTotal;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapBattleUI()
        {
            if (FindAnyObjectByType<BattleUI>() != null)
            {
                return;
            }

            var uiObject = new GameObject("BattleUI");
            uiObject.AddComponent<Canvas>();
            uiObject.AddComponent<BattleUI>();
        }
    }
}
