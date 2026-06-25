using System.Collections;
using System.Collections.Generic;
using SRPG.Audio;
using SRPG.Debugging;
using SRPG.Grid;
using SRPG.Stage;
using SRPG.UI;
using SRPG.Units;
using SRPG.Visual;
using UnityEngine;

namespace SRPG.Battle
{
    public class PlayerController : MonoBehaviour
    {
        private readonly HashSet<Tile> currentMoveTiles = new HashSet<Tile>();
        private readonly HashSet<Tile> currentAttackTiles = new HashSet<Tile>();

        private GridManager gridManager;
        private TurnManager turnManager;
        private EnemyIntentPreview enemyIntentPreview;
        private Unit selectedUnit;
        private bool showEnemyThreats;
        private bool selectedUnitHasMovedThisAction;
        private Vector2Int selectedUnitPositionBeforeMove;
        private bool canUndoSelectedUnitMove;
        private bool isAnimating;

        public static PlayerController Instance { get; private set; }

        public Unit SelectedUnit => selectedUnit;
        public bool IsAnimating => isAnimating;
        public bool IsEnemyThreatVisible => showEnemyThreats;

        public void ResetControllerState()
        {
            if (selectedUnit != null)
            {
                selectedUnit.SetSelected(false);
            }

            selectedUnit = null;
            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            isAnimating = false;
            showEnemyThreats = false;
            enemyIntentPreview?.Clear();
            currentMoveTiles.Clear();
            currentAttackTiles.Clear();

            gridManager = FindAnyObjectByType<GridManager>();
            if (gridManager != null)
            {
                gridManager.ClearAllHighlights();
            }

            BattleUI.Instance?.SetSelectedUnit(null);
            BattleUI.Instance?.SetEnemyThreatVisible(false);
            BattleUI.Instance?.ClearAttackPreview();
        }

        public void HandleTileClicked(Tile tile)
        {
            if (isAnimating || IsMenuOpen() || IsBattleEnded() || !CanPlayerAct() || selectedUnit == null || !selectedUnit.IsPlayerControlled || selectedUnitHasMovedThisAction || tile == null || !currentMoveTiles.Contains(tile))
            {
                return;
            }

            var actingUnit = selectedUnit;
            selectedUnitPositionBeforeMove = actingUnit.GridPosition;
            if (actingUnit.MoveTo(tile.Coordinates))
            {
                AudioManager.Instance?.PlayConfirmSe();
                selectedUnitHasMovedThisAction = true;
                canUndoSelectedUnitMove = true;
                BattleUI.Instance?.ClearAttackPreview();
                ShowSelectedUnitAttackRange(actingUnit);
                BattleUI.Instance?.SetSelectedUnit(actingUnit);
                BattleUI.Instance?.NotifyTutorialPlayerSelectionOrMove();

                if (GetTurnManager()?.CheckVictoryConditions() == true)
                {
                    ClearSelection();
                }
            }
        }

        public void HandleUnitClicked(Unit unit)
        {
            if (isAnimating || IsMenuOpen() || IsBattleEnded() || unit == null || unit.IsDead)
            {
                return;
            }

            var manager = GetTurnManager();
            if (manager == null)
            {
                return;
            }

            if (selectedUnit != null && selectedUnit.IsPlayerControlled && selectedUnitHasMovedThisAction)
            {
                if (unit.Faction != selectedUnit.Faction)
                {
                    TryAttackSelectedUnit(unit);
                }

                return;
            }

            if (selectedUnit != null && selectedUnit.IsPlayerControlled && unit.Faction != selectedUnit.Faction)
            {
                TryAttackSelectedUnit(unit);
                return;
            }

            if (unit.IsPlayerControlled)
            {
                if (!manager.CanSelectUnit(unit))
                {
                    return;
                }

                if (selectedUnit == unit)
                {
                    ClearSelection();
                    return;
                }

                SelectPlayerUnit(unit);
                return;
            }

            if (selectedUnit == unit)
            {
                ClearSelection();
                return;
            }

            SelectEnemyUnit(unit);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            enemyIntentPreview = GetComponent<EnemyIntentPreview>();
            if (enemyIntentPreview == null)
            {
                enemyIntentPreview = gameObject.AddComponent<EnemyIntentPreview>();
            }
        }

        private void Start()
        {
            gridManager = FindAnyObjectByType<GridManager>();
            turnManager = FindAnyObjectByType<TurnManager>();
            if (gridManager == null)
            {
                return;
            }

            gridManager.TileClicked += HandleTileClicked;
            gridManager.UnitRegistered += HandleUnitRegistered;

            foreach (var unit in gridManager.Units)
            {
                HandleUnitRegistered(unit);
            }

            BattleUI.Instance?.SetSelectedUnit(null);
            BattleUI.Instance?.SetEnemyThreatVisible(showEnemyThreats);
        }

        private void Update()
        {
            if (IsMenuOpen())
            {
                return;
            }

            if (isAnimating)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                TryWaitSelectedUnit();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    TryRestorePlayerTurn();
                    return;
                }

                TryUndoSelectedUnitMove();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsBattleEnded())
                {
                    return;
                }

                ToggleEnemyThreatHighlights();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                TryEndPlayerTurn();
            }

            if (showEnemyThreats)
            {
                if (CanPlayerAct())
                {
                    RefreshEnemyThreatHighlights();
                }
                else
                {
                    enemyIntentPreview?.Clear();
                }
            }
        }

        private void TryEndPlayerTurn()
        {
            if (isAnimating || IsMenuOpen() || IsBattleEnded())
            {
                return;
            }

            var manager = GetTurnManager();
            if (manager == null || !manager.IsPlayerTurn)
            {
                return;
            }

            if (!manager.TryEndPlayerTurn())
            {
                AudioManager.Instance?.PlayCancelSe();
                return;
            }

            AudioManager.Instance?.PlayConfirmSe();
            gridManager?.ClearEnemyThreatHighlights();
            enemyIntentPreview?.Clear();
            BattleUI.Instance?.NotifyTutorialEndTurnSucceeded();
        }

        private void OnDestroy()
        {
            if (gridManager != null)
            {
                gridManager.TileClicked -= HandleTileClicked;
                gridManager.UnitRegistered -= HandleUnitRegistered;

                foreach (var unit in gridManager.Units)
                {
                    if (unit != null)
                    {
                        unit.Clicked -= HandleUnitClicked;
                        unit.HoverEntered -= HandleUnitHoverEnter;
                        unit.HoverExited -= HandleUnitHoverExit;
                    }
                }
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void SelectPlayerUnit(Unit unit)
        {
            ClearSelection();

            selectedUnit = unit;
            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            selectedUnit.SetSelected(true);
            BattleUI.Instance?.SetSelectedUnit(unit);
            ShowSelectedUnitRanges(unit);
            AudioManager.Instance?.PlayConfirmSe();
            BattleUI.Instance?.NotifyTutorialPlayerSelectionOrMove();
        }

        private void SelectEnemyUnit(Unit unit)
        {
            ClearSelection();

            selectedUnit = unit;
            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            selectedUnit.SetSelected(true);
            BattleUI.Instance?.SetSelectedUnit(unit);
            RefreshEnemyThreatHighlights();
            AudioManager.Instance?.PlayCursorSe();
            DevLogger.Log($"{unit.name} selected for info.");
        }

        private void ShowSelectedUnitRanges(Unit unit)
        {
            currentMoveTiles.Clear();
            currentAttackTiles.Clear();
            gridManager.ClearPlayerHighlights();

            var reachableTiles = gridManager.GetReachableTiles(unit.GridPosition, unit.MovePower);
            foreach (var tile in reachableTiles)
            {
                tile.SetMoveHighlight(true);
                currentMoveTiles.Add(tile);
            }

            var attackRangeTiles = gridManager.GetAttackRangeTiles(unit);
            foreach (var tile in attackRangeTiles)
            {
                if (!HasAttackableEnemyOnTile(unit, tile))
                {
                    continue;
                }

                tile.SetAttackHighlight(true);
                currentAttackTiles.Add(tile);
            }

            DevLogger.Log($"{unit.name} movement range and attack targets shown.");
        }

        private void ShowSelectedUnitAttackRange(Unit unit)
        {
            currentMoveTiles.Clear();
            currentAttackTiles.Clear();
            gridManager.ClearPlayerHighlights();

            var attackRangeTiles = gridManager.GetAttackRangeTiles(unit);
            foreach (var tile in attackRangeTiles)
            {
                tile.SetAttackHighlight(true);
                currentAttackTiles.Add(tile);
            }

            DevLogger.Log($"{unit.name} post-move attack range shown.");
        }

        private bool HasAttackableEnemyOnTile(Unit attacker, Tile tile)
        {
            return attacker != null
                && tile != null
                && tile.Occupant != null
                && !tile.Occupant.IsDead
                && tile.Occupant.Faction != attacker.Faction;
        }

        private void ClearSelection()
        {
            if (selectedUnit != null)
            {
                selectedUnit.SetSelected(false);
            }

            selectedUnit = null;
            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            BattleUI.Instance?.SetSelectedUnit(null);
            BattleUI.Instance?.ClearAttackPreview();
            currentMoveTiles.Clear();
            currentAttackTiles.Clear();

            if (gridManager != null)
            {
                gridManager.ClearPlayerHighlights();
                DevLogger.Log("Highlights cleared.");
                RefreshEnemyThreatHighlights();
            }
        }

        private void TryAttackSelectedUnit(Unit target)
        {
            if (isAnimating || IsBattleEnded() || !CanPlayerAct() || selectedUnit == null || selectedUnit.HasActed || selectedUnit.IsDead)
            {
                return;
            }

            var actingUnit = selectedUnit;
            if (!actingUnit.CanAttack(target))
            {
                return;
            }

            StartCoroutine(AttackSelectedUnitRoutine(actingUnit, target));
        }

        private IEnumerator AttackSelectedUnitRoutine(Unit actingUnit, Unit target)
        {
            isAnimating = true;
            BattleUI.Instance?.ClearAttackPreview();

            yield return UnitAttackAnimator.PlayAttackAnimation(actingUnit, target);

            if (!IsBattleEnded() && actingUnit != null && target != null && !actingUnit.IsDead && !target.IsDead && actingUnit.Attack(target))
            {
                CompleteSelectedUnitAction(actingUnit);
            }

            isAnimating = false;
        }

        private void TryWaitSelectedUnit()
        {
            if (isAnimating || IsBattleEnded() || !CanPlayerAct() || selectedUnit == null || !selectedUnit.IsPlayerControlled || selectedUnit.IsDead || selectedUnit.HasActed)
            {
                return;
            }

            var actingUnit = selectedUnit;
            Debug.Log($"{actingUnit.name} waited");
            BattleUI.Instance?.AddBattleLog($"{actingUnit.name} waited");
            AudioManager.Instance?.PlayConfirmSe();
            actingUnit.PlayWaitEffect();
            CompleteSelectedUnitAction(actingUnit);
        }

        private void CompleteSelectedUnitAction(Unit actingUnit)
        {
            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            BattleUI.Instance?.ClearAttackPreview();
            var manager = GetTurnManager();
            manager?.NotifyPlayerUnitActed(actingUnit);
            BattleUI.Instance?.NotifyTutorialPlayerActionCompleted();
            if (manager != null && manager.CanEndPlayerTurn)
            {
                BattleUI.Instance?.NotifyTutorialEndTurnAvailable();
            }
            ClearSelection();
        }

        private void TryUndoSelectedUnitMove()
        {
            if (isAnimating || IsBattleEnded() || !CanPlayerAct() || selectedUnit == null || !selectedUnit.IsPlayerControlled || selectedUnit.IsDead || selectedUnit.HasActed || !selectedUnitHasMovedThisAction || !canUndoSelectedUnitMove)
            {
                return;
            }

            var unit = selectedUnit;
            if (!unit.MoveTo(selectedUnitPositionBeforeMove))
            {
                Debug.LogWarning($"{unit.name} movement undo failed at ({selectedUnitPositionBeforeMove.x}, {selectedUnitPositionBeforeMove.y}).");
                return;
            }

            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            BattleUI.Instance?.ClearAttackPreview();
            BattleUI.Instance?.AddBattleLog($"{unit.name} movement undone");
            BattleUI.Instance?.SetSelectedUnit(unit);
            ShowSelectedUnitRanges(unit);
            AudioManager.Instance?.PlayUndoSe();
            Debug.Log($"{unit.name} movement undone.");
        }

        private void TryRestorePlayerTurn()
        {
            if (isAnimating || IsMenuOpen() || IsBattleEnded())
            {
                return;
            }

            var manager = GetTurnManager();
            if (manager == null || !manager.CanRestorePlayerTurn)
            {
                AudioManager.Instance?.PlayCancelSe();
                return;
            }

            showEnemyThreats = false;
            ClearSelection();
            gridManager?.ClearAllHighlights();
            enemyIntentPreview?.Clear();

            if (!manager.TryRestorePlayerTurn(out var restoredEnemyThreatVisible))
            {
                AudioManager.Instance?.PlayCancelSe();
                return;
            }

            selectedUnitHasMovedThisAction = false;
            canUndoSelectedUnitMove = false;
            currentMoveTiles.Clear();
            currentAttackTiles.Clear();
            BattleUI.Instance?.SetSelectedUnit(null);
            BattleUI.Instance?.ClearAttackPreview();
            BattleUI.Instance?.SetTurnInfo(manager.TurnNumber, manager.CurrentPhase.ToString());
            BattleUI.Instance?.NotifyTutorialResetTurnSucceeded();
            RestoreEnemyThreatStateAfterUndo(restoredEnemyThreatVisible);
            AudioManager.Instance?.PlayUndoSe();
            Debug.Log($"Turn {manager.TurnNumber} restored to player turn start.");
        }

        private void RestoreEnemyThreatStateAfterUndo(bool enemyThreatVisible)
        {
            showEnemyThreats = enemyThreatVisible;
            gridManager?.ClearEnemyThreatHighlights();
            enemyIntentPreview?.Clear();
            BattleUI.Instance?.SetEnemyThreatVisible(showEnemyThreats);

            if (showEnemyThreats)
            {
                RefreshEnemyThreatHighlights();
            }
        }

        private void ToggleEnemyThreatHighlights()
        {
            showEnemyThreats = !showEnemyThreats;
            RefreshEnemyThreatHighlights();
            BattleUI.Instance?.SetEnemyThreatVisible(showEnemyThreats);
            AudioManager.Instance?.PlayCursorSe();
            if (showEnemyThreats)
            {
                BattleUI.Instance?.NotifyTutorialEnemyThreatEnabled();
            }
            DevLogger.Log(showEnemyThreats ? "Enemy threat range ON" : "Enemy threat range OFF");
        }

        private void RefreshEnemyThreatHighlights()
        {
            if (gridManager == null)
            {
                return;
            }

            gridManager.ClearEnemyThreatHighlights();
            if (!showEnemyThreats)
            {
                enemyIntentPreview?.Clear();
                return;
            }

            var selectedEnemy = selectedUnit != null && selectedUnit.Faction == Faction.Enemy && !selectedUnit.IsDead
                ? selectedUnit
                : null;

            var guardianReactionTiles = selectedEnemy != null
                ? gridManager.GetGuardianReactionTiles(selectedEnemy)
                : gridManager.GetGuardianReactionTiles();

            foreach (var tile in guardianReactionTiles)
            {
                tile.SetGuardianReactionHighlight(true);
            }

            var enemyAttackThreatTiles = selectedEnemy != null
                ? gridManager.GetEnemyThreatTiles(selectedEnemy)
                : gridManager.GetEnemyThreatTiles();

            foreach (var tile in enemyAttackThreatTiles)
            {
                tile.SetEnemyThreatHighlight(true);
            }

            var enemyMoveThreatTiles = selectedEnemy != null
                ? gridManager.GetEnemyMoveThreatTiles(selectedEnemy)
                : gridManager.GetEnemyMoveThreatTiles();

            foreach (var tile in enemyMoveThreatTiles)
            {
                tile.SetEnemyMoveThreatHighlight(true);
            }

            enemyIntentPreview?.Refresh(gridManager, selectedEnemy);
        }

        private void HandleUnitRegistered(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            unit.Clicked -= HandleUnitClicked;
            unit.Clicked += HandleUnitClicked;
            unit.HoverEntered -= HandleUnitHoverEnter;
            unit.HoverEntered += HandleUnitHoverEnter;
            unit.HoverExited -= HandleUnitHoverExit;
            unit.HoverExited += HandleUnitHoverExit;
        }

        private void HandleUnitHoverEnter(Unit unit)
        {
            if (isAnimating || IsMenuOpen() || IsBattleEnded() || !CanPlayerAct() || selectedUnit == null || !selectedUnit.IsPlayerControlled || unit == null || unit.IsDead || unit.Faction == selectedUnit.Faction)
            {
                return;
            }

            BattleUI.Instance?.ShowAttackPreview(selectedUnit, unit, selectedUnit.CanAttack(unit));
        }

        private void HandleUnitHoverExit(Unit unit)
        {
            if (IsMenuOpen() || selectedUnit == null || !selectedUnit.IsPlayerControlled || unit == null || unit.Faction == selectedUnit.Faction)
            {
                return;
            }

            BattleUI.Instance?.ClearAttackPreview();
        }

        private bool CanPlayerAct()
        {
            var manager = GetTurnManager();
            return manager != null && manager.IsPlayerTurn && !IsMenuOpen();
        }

        private bool IsMenuOpen()
        {
            return StageManager.Instance != null && StageManager.Instance.IsMenuOpen;
        }

        private bool IsBattleEnded()
        {
            var manager = GetTurnManager();
            return manager != null && manager.IsBattleEnded;
        }

        private TurnManager GetTurnManager()
        {
            if (turnManager == null)
            {
                turnManager = FindAnyObjectByType<TurnManager>();
            }

            return turnManager;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapPrototypeScene()
        {
            if (FindAnyObjectByType<GridManager>() == null)
            {
                var gridObject = new GameObject("GridManager");
                gridObject.AddComponent<GridManager>();
            }

            if (FindAnyObjectByType<TurnManager>() == null)
            {
                var turnObject = new GameObject("TurnManager");
                turnObject.AddComponent<TurnManager>();
            }

            if (FindAnyObjectByType<PlayerController>() == null)
            {
                var controllerObject = new GameObject("PlayerController");
                controllerObject.AddComponent<PlayerController>();
            }

            if (FindAnyObjectByType<StageLoader>() == null)
            {
                var stageLoaderObject = new GameObject("StageLoader");
                stageLoaderObject.AddComponent<StageLoader>();
            }

            if (FindAnyObjectByType<StageManager>() == null)
            {
                var stageManagerObject = new GameObject("StageManager");
                stageManagerObject.AddComponent<StageManager>();
            }

            if (FindAnyObjectByType<AudioManager>() == null)
            {
                var audioManagerObject = new GameObject("AudioManager");
                audioManagerObject.AddComponent<AudioManager>();
            }

            if (FindAnyObjectByType<BattleUI>() == null)
            {
                var uiObject = new GameObject("BattleUI");
                uiObject.AddComponent<Canvas>();
                uiObject.AddComponent<BattleUI>();
            }

            SetupCamera();
        }

        private static void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            var grid = FindAnyObjectByType<GridManager>();
            var width = grid != null ? grid.Width : 8;
            var height = grid != null ? grid.Height : 8;
            var cellSize = grid != null ? grid.CellSize : 1f;
            var boardWidth = BoardProjection.GetBoardWidth(width, height, cellSize);
            var boardHeight = BoardProjection.GetBoardHeight(width, height, cellSize);
            var screenAspect = Screen.height > 0 ? (float)Screen.width / Screen.height : 16f / 9f;
            var verticalSize = boardHeight * 0.5f + 0.75f;
            var horizontalSize = boardWidth * 0.5f / screenAspect + 0.75f;
            camera.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
            camera.transform.position = BoardProjection.GetBoardCenter(width, height, cellSize) + new Vector3(0f, -0.08f, -10f);
            var clearFlagsProperty = typeof(Camera).GetProperty("clearFlags");
            if (clearFlagsProperty != null)
            {
                clearFlagsProperty.SetValue(camera, System.Enum.Parse(clearFlagsProperty.PropertyType, "SolidColor"), null);
            }

            camera.backgroundColor = new Color(0.015f, 0.025f, 0.04f, 1f);
        }
    }
}
