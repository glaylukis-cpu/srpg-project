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
        [SerializeField] private Image stageIntroFrame;
        [SerializeField] private Image stageIntroPanel;
        [SerializeField] private Image tutorialHintFrame;
        [SerializeField] private Image tutorialHintPanel;
        [SerializeField] private Text tutorialHintText;
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
        [SerializeField] private Image battleResultBackdrop;
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
        private const float BattleHudInset = 3f;
        private const int BattleHudTitleFontSize = 12;
        private const int BattleHudBodyFontSize = 11;
        private const int BattleHudSmallFontSize = 10;
        private const int TutorialStageNumber = 1;
        private const float UiFadeDuration = 0.16f;
        private const float UiPopScale = 0.97f;
        private const float UiPulseDuration = 0.14f;
        private readonly List<string> battleLogEntries = new List<string>();
        private readonly Dictionary<int, StageBestResult> sessionBestResults = new Dictionary<int, StageBestResult>();
        private readonly Image[] stageSelectRowPanels = new Image[MaxStageSelectRows];
        private readonly Text[] stageSelectRowNumberTexts = new Text[MaxStageSelectRows];
        private readonly Text[] stageSelectRowNameTexts = new Text[MaxStageSelectRows];
        private readonly Text[] stageSelectRowDifficultyTexts = new Text[MaxStageSelectRows];
        private readonly Image[] stageSelectPreviewCells = new Image[StagePreviewSize * StagePreviewSize];
        private Font defaultFont;
        private Coroutine stageIntroCoroutine;
        private Coroutine stageIntroAnimationCoroutine;
        private Coroutine resultAnimationCoroutine;
        private Coroutine tutorialHintAnimationCoroutine;
        private Coroutine battleLogAnimationCoroutine;
        private Coroutine enemyThreatAnimationCoroutine;
        private int selectedStageSelectRow = -1;
        private int selectedTitleMenuRow;
        private int selectedOptionsRow;
        private float stageSelectPulseTimer;
        private float titlePulseTimer;
        private bool titleOverlayInitialized;
        private bool optionsOverlayInitialized;
        private bool stageSelectObjectsInitialized;
        private bool tutorialHintActive;
        private bool resultVisible;
        private string currentResultStyleKey = string.Empty;
        private int tutorialHintStep = -1;
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

        private Color BattleHudBackdropColor()
        {
            return new Color(0.003f, 0.007f, 0.014f, 0.14f);
        }

        private Color BattleHudPanelColor()
        {
            return new Color(0.025f, 0.022f, 0.018f, 0.82f);
        }

        private Color BattleHudFrameColor()
        {
            return new Color(0.58f, 0.42f, 0.18f, 0.62f);
        }

        private Color BattleHudPrimaryTextColor()
        {
            return new Color(0.93f, 0.94f, 0.92f, 1f);
        }

        private Color BattleHudSecondaryTextColor()
        {
            return new Color(0.77f, 0.8f, 0.78f, 1f);
        }

        private Color BattleHudAccentTextColor()
        {
            return new Color(0.94f, 0.74f, 0.38f, 1f);
        }

        private Color BattleHudMutedAccentTextColor()
        {
            return new Color(0.84f, 0.68f, 0.42f, 1f);
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
            BeginTutorialHintForStage(currentStage);
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
                    "<color=#EBC77A>Selected</color>\n" +
                    "No Unit Selected";
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("<color=#EBC77A>Selected</color>");
            builder.AppendLine(unit.name);
            builder.AppendLine($"<color=#EBC77A>Type</color>    {unit.UnitType}");

            if (unit.Faction == Faction.Enemy)
            {
                var prediction = TurnManager.Instance != null ? TurnManager.Instance.GetEnemyActionPrediction(unit) : "Prediction:\nNone";
                builder.AppendLine($"<color=#EBC77A>HP</color>      {unit.CurrentHp} / {unit.MaxHp}");
                builder.AppendLine($"<color=#EBC77A>AI</color>      {unit.EnemyAIType}");
                builder.AppendLine("<color=#EBC77A>Prediction</color>");
                builder.Append(WrapText(CleanPredictionText(prediction), 26));
            }
            else
            {
                builder.AppendLine($"<color=#EBC77A>HP</color>      {unit.CurrentHp} / {unit.MaxHp}");
                builder.AppendLine($"<color=#EBC77A>ATK</color>     {unit.AttackPower}");
                builder.AppendLine($"<color=#EBC77A>MOVE</color>    {unit.MovePower}");
                builder.Append($"<color=#EBC77A>RANGE</color>   {unit.AttackRange}");
            }

            selectedUnitText.text = builder.ToString();
        }

        public void SetEnemyThreatVisible(bool visible)
        {
            EnsureTextObjects();
            enemyThreatText.text = visible ? "Enemy Threat: ON" : "Enemy Threat: OFF";
            PlayEnemyThreatToggleAnimation(visible);
        }

        public void NotifyTutorialPlayerSelectionOrMove()
        {
            AdvanceTutorialHintTo(1);
        }

        public void NotifyTutorialEnemyThreatEnabled()
        {
            AdvanceTutorialHintTo(2);
        }

        public void NotifyTutorialPlayerActionCompleted()
        {
            if (tutorialHintStep == 2)
            {
                AdvanceTutorialHintTo(3);
            }
        }

        public void NotifyTutorialResetTurnSucceeded()
        {
            AdvanceTutorialHintTo(4);
        }

        public void NotifyTutorialEndTurnAvailable()
        {
            if (tutorialHintStep >= 3)
            {
                AdvanceTutorialHintTo(4);
            }
        }

        public void NotifyTutorialEndTurnSucceeded()
        {
            HideTutorialHint();
        }

        public void ShowResult(string result)
        {
            EnsureTextObjects();
            HideTitleScreen();
            HideStageIntro();
            HideTutorialHint();
            resultVisible = true;
            ApplyResultPanelStyle(result);
            SetResultPanelVisible(true);
            resultText.fontSize = 16;
            resultText.text = BuildResultText(result);
            resultText.enabled = true;
            PlayResultPanelAnimation(result);
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
            HideTutorialHint();
            UpdateSessionBestResult(result, rating, turnNumber, playersAlive, playerHpTotal);
            resultVisible = true;
            ApplyResultPanelStyle(result);
            SetResultPanelVisible(true);
            resultText.fontSize = 15;
            resultText.text = BuildResultText(result, rating, turnNumber, turnLimit, playersAlive, playerHpTotal, enemiesAlive, enemyHpTotal);
            resultText.enabled = true;
            PlayResultPanelAnimation(result);
        }

        public void ShowStageIntro(int currentStage, int totalStages, StageData data)
        {
            EnsureTextObjects();
            ClearResult();
            HideTitleScreen();
            HideStageSelect();

            var builder = new StringBuilder();
            builder.AppendLine($"<size=16><color=#EBC77A>Stage {currentStage} / {totalStages}</color></size>");
            if (data != null)
            {
                builder.AppendLine($"<size=24><color=#F0C66B>{data.DisplayName}</color></size>");
                builder.AppendLine($"<color=#BFC6C2>Theme</color>  {data.ThemeName}");
                builder.AppendLine($"<color=#BFC6C2>Difficulty</color>  {data.DifficultyLabel}");
                AppendDescriptionLine(builder, data.Description);
            }

            builder.AppendLine();
            builder.AppendLine(BuildObjectiveText(data, false));
            builder.Append($"<color=#BFC6C2>Limit</color>  {BuildLimitText(data)}");

            if (stageIntroCoroutine != null)
            {
                StopCoroutine(stageIntroCoroutine);
                stageIntroCoroutine = null;
            }

            stageIntroText.text = builder.ToString();
            SetStageIntroPanelVisible(true);
            stageIntroText.enabled = true;
            PlayStageIntroAnimation();
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
            battleLogEntries.Add(FormatBattleLogMessage(message));

            while (battleLogEntries.Count > MaxBattleLogEntries)
            {
                battleLogEntries.RemoveAt(0);
            }

            RefreshBattleLogText();
        }

        public List<string> CaptureBattleLog()
        {
            return new List<string>(battleLogEntries);
        }

        public void RestoreBattleLog(IEnumerable<string> entries)
        {
            EnsureTextObjects();
            battleLogEntries.Clear();

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    if (string.IsNullOrEmpty(entry))
                    {
                        continue;
                    }

                    battleLogEntries.Add(entry);
                    if (battleLogEntries.Count > MaxBattleLogEntries)
                    {
                        battleLogEntries.RemoveAt(0);
                    }
                }
            }

            RefreshBattleLogText();
            PlayBattleLogAnimation();
        }

        private static string FormatBattleLogMessage(string message)
        {
            var text = message.Trim().TrimEnd('.');
            var hasInternalUnitId =
                text.IndexOf("_Player_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("_Enemy_", System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (!hasInternalUnitId)
            {
                return text + ".";
            }

            var undoMarker = " movement undone";
            var markerIndex = text.IndexOf(undoMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                return $"Undo: {GetBattleUnitLabel(text.Substring(0, markerIndex))} returned.";
            }

            var moveMarker = " moved to ";
            markerIndex = text.IndexOf(moveMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                return $"{GetBattleUnitLabel(text.Substring(0, markerIndex))} moved.";
            }

            var attackMarker = " attacked ";
            markerIndex = text.IndexOf(attackMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                var actor = GetBattleUnitLabel(text.Substring(0, markerIndex));
                var attackDetails = text.Substring(markerIndex + attackMarker.Length);
                var damageMarker = " for ";
                var damageIndex = attackDetails.IndexOf(damageMarker, System.StringComparison.Ordinal);
                if (damageIndex > 0)
                {
                    var target = GetBattleUnitLabel(attackDetails.Substring(0, damageIndex));
                    var damage = attackDetails.Substring(damageIndex + damageMarker.Length);
                    return $"{actor} attacked {target} for {damage}.";
                }

                return $"{actor} attacked {GetBattleUnitLabel(attackDetails)}.";
            }

            var noTargetMarker = " has no target";
            markerIndex = text.IndexOf(noTargetMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                return $"{GetBattleUnitLabel(text.Substring(0, markerIndex))} held position.";
            }

            var guardMarker = " guarded";
            markerIndex = text.IndexOf(guardMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                return $"{GetBattleUnitLabel(text.Substring(0, markerIndex))} is guarding.";
            }

            var waitMarker = " waited";
            markerIndex = text.IndexOf(waitMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                return $"{GetBattleUnitLabel(text.Substring(0, markerIndex))} waited.";
            }

            var defeatedMarker = " defeated";
            markerIndex = text.IndexOf(defeatedMarker, System.StringComparison.Ordinal);
            if (markerIndex > 0)
            {
                return $"{GetBattleUnitLabel(text.Substring(0, markerIndex))} was defeated.";
            }

            return text + ".";
        }

        private static string GetBattleUnitLabel(string internalName)
        {
            var name = internalName.Trim();
            if (name.IndexOf("_Player_", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Ally";
            }

            if (name.IndexOf("_Enemy_", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Enemy";
            }

            return name.Replace('_', ' ');
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
            StopResultPanelAnimation();
            resultVisible = false;
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

            StopStageIntroAnimation();
            stageIntroText.text = string.Empty;
            stageIntroText.enabled = false;
            SetStageIntroPanelVisible(false);
        }

        private void BeginTutorialHintForStage(int stageNumber)
        {
            if (stageNumber != TutorialStageNumber)
            {
                HideTutorialHint();
                tutorialHintStep = -1;
                return;
            }

            tutorialHintActive = true;
            tutorialHintStep = 0;
            ShowTutorialHint(BuildTutorialHintText(tutorialHintStep));
        }

        private void AdvanceTutorialHintTo(int step)
        {
            if (!tutorialHintActive || currentStageNumber != TutorialStageNumber || step <= tutorialHintStep)
            {
                return;
            }

            tutorialHintStep = step;
            ShowTutorialHint(BuildTutorialHintText(tutorialHintStep));
        }

        private void ShowTutorialHint(string text)
        {
            EnsureTutorialHintObjects();
            tutorialHintText.text = text;
            SetTutorialHintVisible(true);
            PlayTutorialHintAnimation();
        }

        private void HideTutorialHint()
        {
            tutorialHintActive = false;
            StopTutorialHintAnimation();
            SetTutorialHintVisible(false);
        }

        private string BuildTutorialHintText(int step)
        {
            switch (step)
            {
                case 1:
                    return "Use Space to preview enemy threat.";
                case 2:
                    return "Enemy intent shows attacks, movement, and guards.";
                case 3:
                    return "Try Shift+U to reset the turn if your plan looks unsafe.";
                case 4:
                    return "When your plan is ready, press Enter to end your turn.";
                default:
                    return "Plan your turn.\nSelect an ally and choose a move.";
            }
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
                turnText = CreateText("TurnText", new Vector2(24f, -72f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 15, new Vector2(196f, 42f));
            }
            turnText.fontSize = 15;
            turnText.color = BattleHudPrimaryTextColor();
            ConfigureRect(turnText.rectTransform, new Vector2(24f, -72f), new Vector2(0f, 1f), new Vector2(196f, 42f));

            if (stageText == null)
            {
                stageText = CreateText("StageText", new Vector2(24f, -20f), new Vector2(0f, 1f), TextAnchor.UpperLeft, 15, new Vector2(196f, 50f));
            }
            stageText.fontSize = 15;
            stageText.color = BattleHudAccentTextColor();
            ConfigureRect(stageText.rectTransform, new Vector2(24f, -20f), new Vector2(0f, 1f), new Vector2(196f, 50f));

            if (objectiveText == null)
            {
                objectiveText = CreateText("ObjectiveText", new Vector2(26f, -146f), new Vector2(0f, 1f), TextAnchor.UpperLeft, BattleHudBodyFontSize, new Vector2(196f, 132f));
            }
            objectiveText.fontSize = BattleHudBodyFontSize;
            objectiveText.color = BattleHudSecondaryTextColor();
            ConfigureRect(objectiveText.rectTransform, new Vector2(26f, -146f), new Vector2(0f, 1f), new Vector2(196f, 132f));

            if (selectedUnitText == null)
            {
                selectedUnitText = CreateText("SelectedUnitText", new Vector2(28f, 28f), new Vector2(0f, 0f), TextAnchor.UpperLeft, 13, new Vector2(194f, 112f));
            }
            selectedUnitText.fontSize = 13;
            selectedUnitText.alignment = TextAnchor.UpperLeft;
            selectedUnitText.lineSpacing = 1.08f;
            selectedUnitText.color = BattleHudPrimaryTextColor();
            ConfigureRect(selectedUnitText.rectTransform, new Vector2(28f, 28f), new Vector2(0f, 0f), new Vector2(194f, 112f));

            if (enemyThreatText == null)
            {
                enemyThreatText = CreateText("EnemyThreatText", new Vector2(-22f, -24f), new Vector2(1f, 1f), TextAnchor.UpperRight, 14, new Vector2(180f, 20f));
            }
            enemyThreatText.fontSize = 14;
            enemyThreatText.alignment = TextAnchor.UpperRight;
            enemyThreatText.color = BattleHudAccentTextColor();
            ConfigureRect(enemyThreatText.rectTransform, new Vector2(-22f, -24f), new Vector2(1f, 1f), new Vector2(180f, 20f));

            if (controlsText == null)
            {
                controlsText = CreateText("ControlsText", new Vector2(-22f, 128f), new Vector2(1f, 0f), TextAnchor.UpperLeft, BattleHudTitleFontSize, new Vector2(192f, 18f));
            }
            controlsText.fontSize = BattleHudTitleFontSize;
            controlsText.alignment = TextAnchor.UpperLeft;
            controlsText.color = BattleHudAccentTextColor();
            ConfigureRect(controlsText.rectTransform, new Vector2(-22f, 128f), new Vector2(1f, 0f), new Vector2(192f, 18f));

            if (controlsInputText == null)
            {
                controlsInputText = CreateText("ControlsInputText", new Vector2(-124f, 26f), new Vector2(1f, 0f), TextAnchor.UpperLeft, BattleHudSmallFontSize, new Vector2(94f, 92f));
            }
            controlsInputText.fontSize = BattleHudSmallFontSize;
            controlsInputText.alignment = TextAnchor.UpperLeft;
            controlsInputText.lineSpacing = 1.08f;
            controlsInputText.color = BattleHudMutedAccentTextColor();
            ConfigureRect(controlsInputText.rectTransform, new Vector2(-124f, 24f), new Vector2(1f, 0f), new Vector2(94f, 104f));

            if (controlsActionText == null)
            {
                controlsActionText = CreateText("ControlsActionText", new Vector2(-22f, 26f), new Vector2(1f, 0f), TextAnchor.UpperLeft, BattleHudSmallFontSize, new Vector2(92f, 92f));
            }
            controlsActionText.fontSize = BattleHudSmallFontSize;
            controlsActionText.alignment = TextAnchor.UpperLeft;
            controlsActionText.lineSpacing = 1.08f;
            controlsActionText.color = BattleHudPrimaryTextColor();
            ConfigureRect(controlsActionText.rectTransform, new Vector2(-22f, 24f), new Vector2(1f, 0f), new Vector2(92f, 104f));

            if (battleLogText == null)
            {
                battleLogText = CreateText("BattleLogText", new Vector2(-22f, -30f), new Vector2(1f, 0.5f), TextAnchor.UpperLeft, BattleHudBodyFontSize, new Vector2(194f, 106f));
                RefreshBattleLogText();
            }
            battleLogText.fontSize = BattleHudBodyFontSize;
            battleLogText.alignment = TextAnchor.UpperLeft;
            battleLogText.lineSpacing = 1.08f;
            battleLogText.color = BattleHudPrimaryTextColor();
            ConfigureRect(battleLogText.rectTransform, new Vector2(-22f, -26f), new Vector2(1f, 0.5f), new Vector2(194f, 98f));

            if (attackPreviewText == null)
            {
                attackPreviewText = CreateText("AttackPreviewText", new Vector2(0f, 26f), new Vector2(0.5f, 0f), TextAnchor.MiddleCenter, 14, new Vector2(360f, 56f));
                attackPreviewText.enabled = false;
            }
            attackPreviewText.fontSize = 14;
            attackPreviewText.color = BattleHudPrimaryTextColor();
            ConfigureRect(attackPreviewText.rectTransform, new Vector2(0f, 26f), new Vector2(0.5f, 0f), new Vector2(360f, 56f));

            if (resultText == null)
            {
                resultText = CreateText("ResultText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 34, new Vector2(980f, 430f));
                resultText.enabled = false;
            }
            EnsureResultTextParent();
            resultText.alignment = TextAnchor.MiddleCenter;
            resultText.lineSpacing = 1.08f;
            resultText.color = BattleHudPrimaryTextColor();
            ConfigureRect(resultText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(700f, 300f));

            if (stageIntroText == null)
            {
                stageIntroText = CreateText("StageIntroText", Vector2.zero, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter, 17, new Vector2(610f, 230f));
                stageIntroText.enabled = false;
            }
            EnsureStageIntroPanelObjects();
            stageIntroText.fontSize = 17;
            stageIntroText.alignment = TextAnchor.MiddleCenter;
            stageIntroText.lineSpacing = 1.05f;
            stageIntroText.color = BattleHudPrimaryTextColor();
            ConfigureRect(stageIntroText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(610f, 230f));

            EnsureTutorialHintObjects();

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
                battleBackdropImage = CreatePanel("BattleBackdrop", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f), BattleHudBackdropColor());
                battleBackdropImage.transform.SetAsFirstSibling();
            }
            battleBackdropImage.color = BattleHudBackdropColor();

            EnsurePanelPair(ref battleStageFrame, ref battleStagePanel, "BattleStage", new Vector2(126f, -70f), new Vector2(126f, -70f), new Vector2(0f, 1f), 228f, 116f, BattleHudInset);
            EnsurePanelPair(ref battleObjectiveFrame, ref battleObjectivePanel, "BattleObjective", new Vector2(126f, -216f), new Vector2(126f, -216f), new Vector2(0f, 1f), 228f, 164f, BattleHudInset);
            EnsurePanelPair(ref battleSelectedFrame, ref battleSelectedPanel, "BattleSelected", new Vector2(126f, 86f), new Vector2(126f, 86f), new Vector2(0f, 0f), 228f, 140f, BattleHudInset);
            EnsurePanelPair(ref battleThreatFrame, ref battleThreatPanel, "BattleThreat", new Vector2(-112f, -28f), new Vector2(-112f, -28f), new Vector2(1f, 1f), 208f, 36f, BattleHudInset);
            EnsurePanelPair(ref battleLogFrame, ref battleLogPanel, "BattleLog", new Vector2(-124f, -30f), new Vector2(-124f, -30f), new Vector2(1f, 0.5f), 228f, 132f, BattleHudInset);
            EnsurePanelPair(ref battleControlsFrame, ref battleControlsPanel, "BattleControls", new Vector2(-124f, 88f), new Vector2(-124f, 88f), new Vector2(1f, 0f), 228f, 148f, BattleHudInset);
            EnsurePanelPair(ref battleAttackPreviewFrame, ref battleAttackPreviewPanel, "BattleAttackPreview", new Vector2(0f, 58f), new Vector2(0f, 58f), new Vector2(0.5f, 0f), 396f, 72f, BattleHudInset);
            if (battleResultBackdrop == null)
            {
                battleResultBackdrop = CreatePanel("BattleResultBackdrop", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f), new Color(0f, 0.006f, 0.012f, 0f));
            }
            battleResultBackdrop.color = new Color(0f, 0.006f, 0.012f, 0f);
            EnsurePanelPair(ref battleResultFrame, ref battleResultPanel, "BattleResult", Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f), 820f, 400f, 10f);
            if (!resultVisible)
            {
                ApplyResultPanelStyle(string.Empty);
            }

            SetAttackPreviewPanelVisible(false);
            if (!resultVisible)
            {
                SetResultPanelVisible(false);
            }
        }

        private void EnsureTutorialHintObjects()
        {
            if (tutorialHintFrame == null)
            {
                tutorialHintFrame = CreatePanel("TutorialHintFrame", new Vector2(0f, -32f), new Vector2(0.5f, 1f), new Vector2(500f, 60f), BattleHudFrameColor());
            }
            tutorialHintFrame.color = BattleHudFrameColor();
            ConfigureRect(tutorialHintFrame.rectTransform, new Vector2(0f, -32f), new Vector2(0.5f, 1f), new Vector2(500f, 60f));

            if (tutorialHintPanel == null)
            {
                tutorialHintPanel = CreatePanel("TutorialHintPanel", new Vector2(0f, -32f), new Vector2(0.5f, 1f), new Vector2(490f, 50f), BattleHudPanelColor());
            }
            tutorialHintPanel.color = BattleHudPanelColor();
            ConfigureRect(tutorialHintPanel.rectTransform, new Vector2(0f, -32f), new Vector2(0.5f, 1f), new Vector2(490f, 50f));

            if (tutorialHintText == null)
            {
                tutorialHintText = CreateText("TutorialHintText", new Vector2(0f, -32f), new Vector2(0.5f, 1f), TextAnchor.MiddleCenter, 14, new Vector2(462f, 44f));
            }
            tutorialHintText.fontSize = 14;
            tutorialHintText.alignment = TextAnchor.MiddleCenter;
            tutorialHintText.color = BattleHudPrimaryTextColor();
            ConfigureRect(tutorialHintText.rectTransform, new Vector2(0f, -32f), new Vector2(0.5f, 1f), new Vector2(462f, 44f));

            SetTutorialHintVisible(tutorialHintActive);
        }

        private void EnsureStageIntroPanelObjects()
        {
            if (stageIntroFrame == null)
            {
                stageIntroFrame = CreatePanel("StageIntroFrame", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(680f, 304f), new Color(0.64f, 0.46f, 0.18f, 0.72f));
            }
            stageIntroFrame.color = new Color(0.64f, 0.46f, 0.18f, 0.72f);
            ConfigurePanelRect(stageIntroFrame.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(680f, 304f));

            if (stageIntroPanel == null)
            {
                stageIntroPanel = CreatePanel("StageIntroPanel", Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(656f, 280f), new Color(0.045f, 0.037f, 0.028f, 0.9f));
            }
            stageIntroPanel.color = new Color(0.045f, 0.037f, 0.028f, 0.9f);
            ConfigurePanelRect(stageIntroPanel.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(656f, 280f));

            if (stageIntroText != null && stageIntroText.transform.parent != stageIntroPanel.transform)
            {
                stageIntroText.transform.SetParent(stageIntroPanel.transform, false);
            }

            SetStageIntroPanelVisible(stageIntroText != null && stageIntroText.enabled);
        }

        private void EnsurePanelPair(ref Image frame, ref Image panel, string baseName, Vector2 framePosition, Vector2 panelPosition, Vector2 anchor, float width, float height, float inset)
        {
            var frameSize = new Vector2(width, height);
            var panelSize = new Vector2(width - inset * 2f, height - inset * 2f);
            if (frame == null)
            {
                frame = CreatePanel($"{baseName}Frame", framePosition, anchor, frameSize, BattleHudFrameColor());
            }
            frame.color = BattleHudFrameColor();
            ConfigurePanelRect(frame.rectTransform, framePosition, anchor, frameSize);

            if (panel == null)
            {
                panel = CreatePanel(
                    $"{baseName}Panel",
                    panelPosition,
                    anchor,
                    panelSize,
                    BattleHudPanelColor());
            }
            panel.color = BattleHudPanelColor();
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
            var objective = data.VictoryCondition == VictoryConditionType.ReachGoal
                ? "Reach a goal tile"
                : "Defeat all enemies";

            if (!includeDefeatCondition)
            {
                builder.AppendLine($"<color=#FFD98F>Objective</color>\n{objective}");
                return builder.ToString().TrimEnd();
            }

            builder.Append("<color=#FFD98F>Theme:</color> ");
            builder.AppendLine(WrapText(data.ThemeName, 24));
            builder.AppendLine($"<color=#FFD98F>Goal:</color> {objective}");
            builder.Append("<color=#FFD98F>Tip:</color> ");
            builder.AppendLine(BuildBattleTipText(data));

            if (data.DefeatConditions != null && data.DefeatConditions.Contains(DefeatConditionType.AllPlayersDefeated))
            {
                builder.AppendLine("<color=#FFD98F>Lose:</color> All players defeated");
            }

            builder.Append("<color=#FFD98F>Limit:</color> ");
            builder.Append(BuildLimitText(data));

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
            if (tip.StartsWith("Learn the core flow:"))
            {
                return "Select allies, move, attack.";
            }

            if (tip.Length > 48)
            {
                tip = tip.Substring(0, 45).TrimEnd() + "...";
            }

            return WrapText(tip, 28);
        }

        private string BuildResultText(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }

            if (currentStageData == null || result.StartsWith("ALL CLEAR"))
            {
                return BuildSimpleResultText(result);
            }

            var builder = new StringBuilder();
            AppendResultHeader(builder, result);
            builder.AppendLine();
            builder.AppendLine("<size=17><color=#EBC77A>Result Summary</color></size>");
            builder.AppendLine($"<color=#BFC6C2>Stage</color>      {currentStageNumber} / {currentTotalStages} - {currentStageData.DisplayName}");
            builder.AppendLine($"<color=#BFC6C2>Theme</color>      {currentStageData.ThemeName}");
            builder.AppendLine(currentStageData.VictoryCondition == VictoryConditionType.ReachGoal
                ? "<color=#BFC6C2>Objective</color>  Reach a goal tile"
                : "<color=#BFC6C2>Objective</color>  Defeat all enemies");
            builder.Append($"<color=#BFC6C2>Limit</color>      {BuildLimitText(currentStageData)}");
            return builder.ToString();
        }

        private string BuildSimpleResultText(string result)
        {
            var builder = new StringBuilder();
            AppendResultHeader(builder, result);
            return builder.ToString().TrimEnd();
        }

        private void AppendResultHeader(StringBuilder builder, string result)
        {
            var lines = result.Split('\n');
            var title = lines.Length > 0 ? lines[0].Trim() : result.Trim();
            builder.AppendLine($"<size={GetResultTitleSize(result)}><color={GetResultTitleColor(result)}>{title}</color></size>");

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                builder.AppendLine($"<size=16><color=#D8D2C4>{line}</color></size>");
            }
        }

        private int GetResultTitleSize(string result)
        {
            return result != null && result.StartsWith("ALL CLEAR") ? 44 : 38;
        }

        private string GetResultTitleColor(string result)
        {
            if (result != null && result.StartsWith("DEFEAT"))
            {
                return "#D47A68";
            }

            if (result != null && result.StartsWith("ALL CLEAR"))
            {
                return "#F4E4A5";
            }

            return "#F0C66B";
        }

        private void ApplyResultPanelStyle(string result)
        {
            EnsureResultTextParent();
            currentResultStyleKey = result ?? string.Empty;

            if (battleResultFrame != null)
            {
                battleResultFrame.color = GetResultFrameColor(result, 1f);
                ConfigurePanelRect(battleResultFrame.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), GetResultFrameSize(result));
            }

            if (battleResultPanel != null)
            {
                battleResultPanel.color = GetResultPanelColor(1f);
                ConfigurePanelRect(battleResultPanel.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), GetResultPanelSize(result));
            }

            if (battleResultBackdrop != null)
            {
                battleResultBackdrop.color = GetResultBackdropColor(1f);
                ConfigurePanelRect(battleResultBackdrop.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f));
            }

            if (resultText != null)
            {
                resultText.alignment = TextAnchor.MiddleCenter;
                resultText.lineSpacing = 1f;
                resultText.color = BattleHudPrimaryTextColor();
                ConfigureRect(resultText.rectTransform, Vector2.zero, new Vector2(0.5f, 0.5f), GetResultTextSize(result));
            }
        }

        private void EnsureResultTextParent()
        {
            if (resultText == null || battleResultPanel == null)
            {
                return;
            }

            if (resultText.transform.parent != battleResultPanel.transform)
            {
                resultText.transform.SetParent(battleResultPanel.transform, false);
            }
        }

        private Color GetResultFrameColor(string result, float alphaScale)
        {
            if (result != null && result.StartsWith("DEFEAT"))
            {
                return new Color(0.58f, 0.25f, 0.18f, 0.96f * alphaScale);
            }

            if (result != null && result.StartsWith("ALL CLEAR"))
            {
                return new Color(0.82f, 0.68f, 0.34f, 0.98f * alphaScale);
            }

            return new Color(0.72f, 0.54f, 0.24f, 0.98f * alphaScale);
        }

        private Color GetResultPanelColor(float alphaScale)
        {
            return new Color(0.055f, 0.043f, 0.032f, 0.92f * alphaScale);
        }

        private Color GetResultBackdropColor(float alphaScale)
        {
            return new Color(0f, 0.006f, 0.012f, 0.32f * alphaScale);
        }

        private Vector2 GetResultFrameSize(string result)
        {
            return result != null && result.StartsWith("ALL CLEAR")
                ? new Vector2(560f, 220f)
                : new Vector2(760f, 360f);
        }

        private Vector2 GetResultPanelSize(string result)
        {
            return result != null && result.StartsWith("ALL CLEAR")
                ? new Vector2(534f, 194f)
                : new Vector2(734f, 334f);
        }

        private Vector2 GetResultTextSize(string result)
        {
            return result != null && result.StartsWith("ALL CLEAR")
                ? new Vector2(500f, 150f)
                : new Vector2(670f, 292f);
        }

        private void PlayStageIntroAnimation()
        {
            StopUiAnimation(ref stageIntroAnimationCoroutine);
            stageIntroAnimationCoroutine = StartCoroutine(AnimateStageIntroPanel());
        }

        private IEnumerator AnimateStageIntroPanel()
        {
            var elapsed = 0f;
            while (elapsed < UiFadeDuration)
            {
                var t = EaseOut(elapsed / UiFadeDuration);
                SetStageIntroVisualState(t, UiPopScale + (1f - UiPopScale) * t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetStageIntroVisualState(1f, 1f);
            stageIntroAnimationCoroutine = null;
        }

        private void StopStageIntroAnimation()
        {
            StopUiAnimation(ref stageIntroAnimationCoroutine);
            SetStageIntroVisualState(1f, 1f);
        }

        private void SetStageIntroVisualState(float alphaScale, float scale)
        {
            alphaScale = Mathf.Clamp01(alphaScale);
            SetLocalScale(stageIntroFrame, scale);
            SetLocalScale(stageIntroPanel, scale);
            if (stageIntroFrame != null)
            {
                stageIntroFrame.color = new Color(0.64f, 0.46f, 0.18f, 0.72f * alphaScale);
            }

            if (stageIntroPanel != null)
            {
                stageIntroPanel.color = new Color(0.045f, 0.037f, 0.028f, 0.9f * alphaScale);
            }

            if (stageIntroText != null)
            {
                stageIntroText.color = BattleHudPrimaryTextColor(alphaScale);
            }
        }

        private void PlayResultPanelAnimation(string result)
        {
            StopUiAnimation(ref resultAnimationCoroutine);
            resultAnimationCoroutine = StartCoroutine(AnimateResultPanel(result));
        }

        private IEnumerator AnimateResultPanel(string result)
        {
            var elapsed = 0f;
            while (elapsed < UiFadeDuration)
            {
                var t = EaseOut(elapsed / UiFadeDuration);
                SetResultVisualState(result, t, UiPopScale + (1f - UiPopScale) * t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetResultVisualState(result, 1f, 1f);
            resultAnimationCoroutine = null;
        }

        private void StopResultPanelAnimation()
        {
            StopUiAnimation(ref resultAnimationCoroutine);
            SetResultVisualState(currentResultStyleKey, 1f, 1f);
        }

        private void SetResultVisualState(string result, float alphaScale, float scale)
        {
            alphaScale = Mathf.Clamp01(alphaScale);
            SetLocalScale(battleResultFrame, scale);
            SetLocalScale(battleResultPanel, scale);
            if (battleResultBackdrop != null)
            {
                battleResultBackdrop.color = GetResultBackdropColor(alphaScale);
            }

            if (battleResultFrame != null)
            {
                battleResultFrame.color = GetResultFrameColor(result, alphaScale);
            }

            if (battleResultPanel != null)
            {
                battleResultPanel.color = GetResultPanelColor(alphaScale);
            }

            if (resultText != null)
            {
                resultText.color = BattleHudPrimaryTextColor(alphaScale);
            }
        }

        private void PlayTutorialHintAnimation()
        {
            StopUiAnimation(ref tutorialHintAnimationCoroutine);
            tutorialHintAnimationCoroutine = StartCoroutine(AnimateTutorialHint());
        }

        private IEnumerator AnimateTutorialHint()
        {
            var elapsed = 0f;
            while (elapsed < UiPulseDuration)
            {
                var t = EaseOut(elapsed / UiPulseDuration);
                SetTutorialHintVisualState(t, 0.985f + 0.015f * t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetTutorialHintVisualState(1f, 1f);
            tutorialHintAnimationCoroutine = null;
        }

        private void StopTutorialHintAnimation()
        {
            StopUiAnimation(ref tutorialHintAnimationCoroutine);
            SetTutorialHintVisualState(1f, 1f);
        }

        private void SetTutorialHintVisualState(float alphaScale, float scale)
        {
            alphaScale = Mathf.Clamp01(alphaScale);
            SetLocalScale(tutorialHintFrame, scale);
            SetLocalScale(tutorialHintPanel, scale);
            SetLocalScale(tutorialHintText, scale);
            if (tutorialHintFrame != null)
            {
                tutorialHintFrame.color = new Color(0.58f, 0.42f, 0.18f, 0.62f * alphaScale);
            }

            if (tutorialHintPanel != null)
            {
                tutorialHintPanel.color = new Color(0.025f, 0.022f, 0.018f, 0.82f * alphaScale);
            }

            if (tutorialHintText != null)
            {
                tutorialHintText.color = BattleHudPrimaryTextColor(alphaScale);
            }
        }

        private void PlayBattleLogAnimation()
        {
            StopUiAnimation(ref battleLogAnimationCoroutine);
            battleLogAnimationCoroutine = StartCoroutine(AnimateBattleLogPulse());
        }

        private IEnumerator AnimateBattleLogPulse()
        {
            var elapsed = 0f;
            while (elapsed < UiPulseDuration)
            {
                var t = EaseOut(elapsed / UiPulseDuration);
                var lift = 1f - t;
                SetLocalScale(battleLogFrame, 0.99f + 0.01f * t);
                SetLocalScale(battleLogPanel, 0.99f + 0.01f * t);
                SetLocalScale(battleLogText, 0.99f + 0.01f * t);
                if (battleLogFrame != null)
                {
                    battleLogFrame.color = new Color(0.62f + 0.12f * lift, 0.46f + 0.08f * lift, 0.2f + 0.04f * lift, 0.62f + 0.16f * lift);
                }

                if (battleLogPanel != null)
                {
                    battleLogPanel.color = new Color(0.025f + 0.018f * lift, 0.022f + 0.012f * lift, 0.018f + 0.006f * lift, 0.82f + 0.06f * lift);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            SetBattleLogVisualFinal();
            battleLogAnimationCoroutine = null;
        }

        private void SetBattleLogVisualFinal()
        {
            SetLocalScale(battleLogFrame, 1f);
            SetLocalScale(battleLogPanel, 1f);
            SetLocalScale(battleLogText, 1f);
            if (battleLogFrame != null)
            {
                battleLogFrame.color = BattleHudFrameColor();
            }

            if (battleLogPanel != null)
            {
                battleLogPanel.color = BattleHudPanelColor();
            }
        }

        private void PlayEnemyThreatToggleAnimation(bool visible)
        {
            StopUiAnimation(ref enemyThreatAnimationCoroutine);
            enemyThreatAnimationCoroutine = StartCoroutine(AnimateEnemyThreatToggle(visible));
        }

        private IEnumerator AnimateEnemyThreatToggle(bool visible)
        {
            var elapsed = 0f;
            while (elapsed < UiPulseDuration)
            {
                var t = EaseOut(elapsed / UiPulseDuration);
                var lift = 1f - t;
                var scale = 0.985f + 0.015f * t;
                SetLocalScale(battleThreatFrame, scale);
                SetLocalScale(battleThreatPanel, scale);
                SetLocalScale(enemyThreatText, scale);
                if (battleThreatFrame != null)
                {
                    battleThreatFrame.color = visible
                        ? new Color(0.58f + 0.12f * lift, 0.42f + 0.12f * lift, 0.18f + 0.03f * lift, 0.62f + 0.18f * lift)
                        : new Color(0.58f, 0.42f, 0.18f, 0.62f + 0.1f * lift);
                }

                if (battleThreatPanel != null)
                {
                    battleThreatPanel.color = visible
                        ? new Color(0.038f, 0.034f + 0.018f * lift, 0.028f, 0.82f + 0.08f * lift)
                        : BattleHudPanelColor();
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            SetEnemyThreatVisualFinal();
            enemyThreatAnimationCoroutine = null;
        }

        private void SetEnemyThreatVisualFinal()
        {
            SetLocalScale(battleThreatFrame, 1f);
            SetLocalScale(battleThreatPanel, 1f);
            SetLocalScale(enemyThreatText, 1f);
            if (battleThreatFrame != null)
            {
                battleThreatFrame.color = BattleHudFrameColor();
            }

            if (battleThreatPanel != null)
            {
                battleThreatPanel.color = BattleHudPanelColor();
            }

            if (enemyThreatText != null)
            {
                enemyThreatText.color = BattleHudAccentTextColor();
            }
        }

        private void StopUiAnimation(ref Coroutine routine)
        {
            if (routine == null)
            {
                return;
            }

            StopCoroutine(routine);
            routine = null;
        }

        private float EaseOut(float value)
        {
            value = Mathf.Clamp01(value);
            return value * (2f - value);
        }

        private void SetLocalScale(Component component, float scale)
        {
            if (component != null)
            {
                component.transform.localScale = Vector3.one * scale;
            }
        }

        private Color BattleHudPrimaryTextColor(float alpha)
        {
            return new Color(0.93f, 0.94f, 0.92f, alpha);
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
            builder.AppendLine($"<color=#EBC77A>Rating</color>     {rating}");
            builder.AppendLine($"<color=#BFC6C2>Turn</color>       {turnNumber} / {turnLimit}");
            builder.AppendLine($"<color=#BFC6C2>Survivors</color>  {playersAlive}    <color=#BFC6C2>HP Total</color> {playerHpTotal}");
            builder.Append($"<color=#BFC6C2>Enemies</color>    {enemiesAlive}    <color=#BFC6C2>Enemy HP</color> {enemyHpTotal}");
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
            StopStageIntroAnimation();
            stageIntroText.text = string.Empty;
            stageIntroText.enabled = false;
            SetStageIntroPanelVisible(false);
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
                "Shift+U\n" +
                "R\n" +
                "Esc";
            controlsActionText.text =
                "Select\n" +
                "Preview\n" +
                "Info / Attack\n" +
                "Move\n" +
                "Wait / Confirm\n" +
                "Undo Move\n" +
                "Reset Turn\n" +
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
            Debug.Log("SRPG Prototype Controls:\nClick Ally: Select\nHover Enemy: Preview\nClick Enemy: Info / Attack\nClick Blue Tile: Move\nW: Wait / Confirm\nU: Undo Move\nShift+U: Reset Turn\nR: Restart Stage\nEsc/S: Stage Select\nSpace: Toggle Threat / Selected Enemy Range\nEnter: Start/Next/Retry");
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
            builder.AppendLine("<color=#EBC77A>Battle Log</color>");
            if (battleLogEntries.Count == 0)
            {
                builder.Append("<color=#B8BDB8>No recent actions.</color>");
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
            SetTutorialHintVisible(visible && tutorialHintActive);
            if (!visible)
            {
                SetTextEnabled(attackPreviewText, false);
                SetAttackPreviewPanelVisible(false);
                SetResultPanelVisible(resultVisible);
            }
        }

        private void SetTutorialHintVisible(bool visible)
        {
            SetImageEnabled(tutorialHintFrame, visible);
            SetImageEnabled(tutorialHintPanel, visible);
            SetTextEnabled(tutorialHintText, visible);
        }

        private void SetStageIntroPanelVisible(bool visible)
        {
            SetImageEnabled(stageIntroFrame, visible);
            SetImageEnabled(stageIntroPanel, visible);
            SetTextEnabled(stageIntroText, visible);
            if (visible)
            {
                BringToFront(stageIntroFrame);
                BringToFront(stageIntroPanel);
                BringToFront(stageIntroText);
            }
        }

        private void SetAttackPreviewPanelVisible(bool visible)
        {
            SetImageEnabled(battleAttackPreviewFrame, visible);
            SetImageEnabled(battleAttackPreviewPanel, visible);
        }

        private void SetResultPanelVisible(bool visible)
        {
            SetImageEnabled(battleResultBackdrop, visible);
            SetImageEnabled(battleResultFrame, visible);
            SetImageEnabled(battleResultPanel, visible);
            SetTextEnabled(resultText, visible && resultVisible);
            if (visible)
            {
                BringToFront(battleResultBackdrop);
                BringToFront(battleResultFrame);
                BringToFront(battleResultPanel);
                BringToFront(resultText);
            }
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
