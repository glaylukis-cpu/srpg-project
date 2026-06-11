using System.Collections;
using SRPG.Audio;
using SRPG.Grid;
using SRPG.Stage;
using SRPG.UI;
using SRPG.Units;
using UnityEngine;

namespace SRPG.Battle
{
    public enum BattleResult
    {
        None,
        Victory,
        Defeat
    }

    public enum TurnPhase
    {
        PlayerTurn,
        EnemyTurn
    }

    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private int turnNumber = 1;
        [SerializeField] private TurnPhase currentPhase = TurnPhase.PlayerTurn;

        private GridManager gridManager;
        private Coroutine enemyTurnCoroutine;
        private bool battleEnded;
        private BattleResult result = BattleResult.None;
        private StageData currentStageData;

        public static TurnManager Instance { get; private set; }

        public int TurnNumber => turnNumber;
        public TurnPhase CurrentPhase => currentPhase;
        public bool IsPlayerTurn => currentPhase == TurnPhase.PlayerTurn;
        public bool IsBattleEnded => battleEnded;
        public BattleResult Result => result;

        public void SetStageData(StageData data)
        {
            currentStageData = data;
            BattleUI.Instance?.SetObjectiveInfo(data);
        }

        public bool CanSelectUnit(Unit unit)
        {
            return !battleEnded && IsPlayerTurn && unit != null && !unit.IsDead && unit.IsPlayerControlled && !unit.HasActed;
        }

        public string GetEnemyActionPrediction(Unit enemy)
        {
            if (enemy == null || enemy.IsDead || enemy.Faction != Faction.Enemy || !EnsureGridManager())
            {
                return "Prediction:\nNone";
            }

            switch (enemy.EnemyAIType)
            {
                case EnemyAIType.WeakTarget:
                    return GetMoveOrAttackPrediction(enemy, FindWeakestPlayerUnit(enemy));
                case EnemyAIType.Stationary:
                    return GetStationaryPrediction(enemy);
                case EnemyAIType.Guardian:
                    return GetGuardianPrediction(enemy);
                default:
                    return GetMoveOrAttackPrediction(enemy, FindNearestPlayerUnit(enemy));
            }
        }

        public void NotifyUnitMoved(Unit unit)
        {
            NotifyPlayerUnitActed(unit);
        }

        public bool CheckVictoryConditions()
        {
            return CheckVictory();
        }

        public void NotifyPlayerUnitActed(Unit unit)
        {
            if (battleEnded || !IsPlayerTurn || unit == null || !unit.IsPlayerControlled)
            {
                return;
            }

            unit.SetHasActed(true);
            Debug.Log($"{unit.name} acted.");

            if (CheckVictory())
            {
                return;
            }

            if (HaveAllPlayerUnitsActed())
            {
                BeginEnemyTurn();
            }
        }

        public void ResetBattleState()
        {
            if (enemyTurnCoroutine != null)
            {
                StopCoroutine(enemyTurnCoroutine);
                enemyTurnCoroutine = null;
            }

            battleEnded = false;
            result = BattleResult.None;
            turnNumber = 1;
            currentPhase = TurnPhase.PlayerTurn;
            gridManager = FindAnyObjectByType<GridManager>();
            ResetPlayerUnitActions();
            BattleUI.Instance?.ClearResult();
            BattleUI.Instance?.SetTurnInfo(turnNumber, currentPhase.ToString());
        }

        public void StopActiveEnemyTurn()
        {
            if (enemyTurnCoroutine == null)
            {
                return;
            }

            StopCoroutine(enemyTurnCoroutine);
            enemyTurnCoroutine = null;
            Debug.Log("Enemy turn stopped for stage select.");
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private IEnumerator Start()
        {
            yield return null;

            gridManager = FindAnyObjectByType<GridManager>();
            BeginPlayerTurn(true);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void BeginPlayerTurn(bool resetPlayerUnits)
        {
            if (battleEnded)
            {
                return;
            }

            currentPhase = TurnPhase.PlayerTurn;

            if (resetPlayerUnits)
            {
                ResetPlayerUnitActions();
            }

            Debug.Log($"Turn {turnNumber}: {currentPhase}");
            BattleUI.Instance?.SetTurnInfo(turnNumber, currentPhase.ToString());

            CheckDefeat();
        }

        private void BeginEnemyTurn()
        {
            if (battleEnded)
            {
                return;
            }

            if (enemyTurnCoroutine != null)
            {
                StopCoroutine(enemyTurnCoroutine);
            }

            currentPhase = TurnPhase.EnemyTurn;
            Debug.Log($"Turn {turnNumber}: {currentPhase}");
            BattleUI.Instance?.SetTurnInfo(turnNumber, currentPhase.ToString());
            enemyTurnCoroutine = StartCoroutine(EnemyTurnRoutine());
        }

        private IEnumerator EnemyTurnRoutine()
        {
            if (!EnsureGridManager())
            {
                enemyTurnCoroutine = null;
                yield break;
            }

            ResetEnemyUnitActions();

            var enemies = gridManager.GetUnitsByFaction(Faction.Enemy);
            foreach (var enemy in enemies)
            {
                if (battleEnded || enemy == null || enemy.IsDead || enemy.HasActed)
                {
                    continue;
                }

                yield return ActEnemyUnit(enemy);
                enemy.SetHasActed(true);
                Debug.Log($"{enemy.name} acted.");

                if (CheckDefeat())
                {
                    enemyTurnCoroutine = null;
                    yield break;
                }

                yield return null;
            }

            enemyTurnCoroutine = null;
            EndEnemyTurn();
        }

        private IEnumerator ActEnemyUnit(Unit enemy)
        {
            switch (enemy.EnemyAIType)
            {
                case EnemyAIType.WeakTarget:
                    yield return ActAggressiveEnemy(enemy, FindWeakestPlayerUnit(enemy));
                    break;
                case EnemyAIType.Stationary:
                    yield return ActStationaryEnemy(enemy);
                    break;
                case EnemyAIType.Guardian:
                    yield return ActGuardianEnemy(enemy);
                    break;
                default:
                    yield return ActAggressiveEnemy(enemy, FindNearestPlayerUnit(enemy));
                    break;
            }
        }

        private IEnumerator ActAggressiveEnemy(Unit enemy, Unit target)
        {
            if (target == null)
            {
                Debug.Log($"{enemy.name} has no target.");
                BattleUI.Instance?.AddBattleLog($"{enemy.name} has no target");
                yield break;
            }

            var attackResult = new AttackAttemptResult();
            yield return TryEnemyAttack(enemy, target, attackResult);
            if (attackResult.Succeeded)
            {
                yield break;
            }

            var currentDistance = gridManager.GetManhattanDistance(enemy.GridPosition, target.GridPosition);
            var bestTile = FindBestApproachTile(enemy, target, currentDistance);
            if (bestTile != null && enemy.MoveTo(bestTile.Coordinates))
            {
                Debug.Log($"{enemy.name} moved to ({bestTile.Coordinates.x}, {bestTile.Coordinates.y})");
                BattleUI.Instance?.AddBattleLog($"{enemy.name} moved to ({bestTile.Coordinates.x}, {bestTile.Coordinates.y})");
            }

            if (target != null && !target.IsDead)
            {
                yield return TryEnemyAttack(enemy, target, attackResult);
            }
        }

        private IEnumerator ActStationaryEnemy(Unit enemy)
        {
            var target = FindAttackableWeakestPlayerUnit(enemy);
            if (target == null)
            {
                Debug.Log($"{enemy.name} waited.");
                BattleUI.Instance?.AddBattleLog($"{enemy.name} waited");
                enemy.PlayWaitEffect();
                yield break;
            }

            yield return TryEnemyAttack(enemy, target, new AttackAttemptResult());
        }

        private IEnumerator ActGuardianEnemy(Unit enemy)
        {
            var target = FindNearestPlayerUnitInsideGuardianRange(enemy, 3);
            if (target == null)
            {
                Debug.Log($"{enemy.name} guarded.");
                BattleUI.Instance?.AddBattleLog($"{enemy.name} guarded");
                enemy.PlayWaitEffect();
                yield break;
            }

            yield return ActAggressiveEnemy(enemy, target);
        }

        private string GetMoveOrAttackPrediction(Unit enemy, Unit target)
        {
            if (target == null)
            {
                return "Prediction:\nWill wait";
            }

            return enemy.CanAttack(target)
                ? $"Prediction:\nWill attack {target.name}"
                : $"Prediction:\nWill move toward {target.name}";
        }

        private string GetStationaryPrediction(Unit enemy)
        {
            var target = FindAttackableWeakestPlayerUnit(enemy);
            return target == null
                ? "Prediction:\nWill wait"
                : $"Prediction:\nWill attack {target.name}";
        }

        private string GetGuardianPrediction(Unit enemy)
        {
            var target = FindNearestPlayerUnitInsideGuardianRange(enemy, 3);
            if (target == null)
            {
                return "Prediction:\nGuarding";
            }

            return GetMoveOrAttackPrediction(enemy, target);
        }

        private IEnumerator TryEnemyAttack(Unit enemy, Unit target, AttackAttemptResult result)
        {
            result.Succeeded = false;
            if (enemy == null || target == null || enemy.IsDead || target.IsDead || !enemy.CanAttack(target))
            {
                yield break;
            }

            var damage = enemy.AttackPower;
            yield return UnitAttackAnimator.PlayAttackAnimation(enemy, target);

            if (!enemy.Attack(target))
            {
                yield break;
            }

            Debug.Log($"{enemy.name} attacked {target.name} for {damage} damage");
            result.Succeeded = true;
        }

        private Unit FindNearestPlayerUnit(Unit enemy)
        {
            var players = gridManager.GetUnitsByFaction(Faction.Player);
            Unit nearestPlayer = null;
            var nearestDistance = int.MaxValue;

            foreach (var player in players)
            {
                if (player == null || player.IsDead)
                {
                    continue;
                }

                var distance = gridManager.GetManhattanDistance(enemy.GridPosition, player.GridPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        private Unit FindWeakestPlayerUnit(Unit enemy)
        {
            var players = gridManager.GetUnitsByFaction(Faction.Player);
            Unit weakestPlayer = null;
            var lowestHp = int.MaxValue;
            var nearestDistance = int.MaxValue;

            foreach (var player in players)
            {
                if (player == null || player.IsDead)
                {
                    continue;
                }

                var distance = gridManager.GetManhattanDistance(enemy.GridPosition, player.GridPosition);
                if (player.CurrentHp < lowestHp || player.CurrentHp == lowestHp && distance < nearestDistance)
                {
                    lowestHp = player.CurrentHp;
                    nearestDistance = distance;
                    weakestPlayer = player;
                }
            }

            return weakestPlayer;
        }

        private Unit FindAttackableWeakestPlayerUnit(Unit enemy)
        {
            var players = gridManager.GetUnitsByFaction(Faction.Player);
            Unit weakestPlayer = null;
            var lowestHp = int.MaxValue;
            var nearestDistance = int.MaxValue;

            foreach (var player in players)
            {
                if (player == null || player.IsDead || !enemy.CanAttack(player))
                {
                    continue;
                }

                var distance = gridManager.GetManhattanDistance(enemy.GridPosition, player.GridPosition);
                if (player.CurrentHp < lowestHp || player.CurrentHp == lowestHp && distance < nearestDistance)
                {
                    lowestHp = player.CurrentHp;
                    nearestDistance = distance;
                    weakestPlayer = player;
                }
            }

            return weakestPlayer;
        }

        private Unit FindNearestPlayerUnitInsideGuardianRange(Unit enemy, int guardianRange)
        {
            var players = gridManager.GetUnitsByFaction(Faction.Player);
            Unit nearestPlayer = null;
            var nearestDistance = int.MaxValue;

            foreach (var player in players)
            {
                if (player == null || player.IsDead)
                {
                    continue;
                }

                var distanceFromInitialPosition = gridManager.GetManhattanDistance(enemy.InitialGridPosition, player.GridPosition);
                if (distanceFromInitialPosition > guardianRange)
                {
                    continue;
                }

                var distanceFromEnemy = gridManager.GetManhattanDistance(enemy.GridPosition, player.GridPosition);
                if (distanceFromEnemy < nearestDistance)
                {
                    nearestDistance = distanceFromEnemy;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        private Tile FindBestApproachTile(Unit enemy, Unit target, int currentDistance)
        {
            var reachableTiles = gridManager.GetReachableTiles(enemy.GridPosition, enemy.MovePower);
            Tile bestTile = null;
            var bestDistance = currentDistance;

            foreach (var tile in reachableTiles)
            {
                var distance = gridManager.GetManhattanDistance(tile.Coordinates, target.GridPosition);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTile = tile;
                }
            }

            return bestTile;
        }

        private void EndEnemyTurn()
        {
            if (battleEnded)
            {
                return;
            }

            ResetPlayerUnitActions();
            turnNumber++;
            BeginPlayerTurn(false);
        }

        private bool CheckVictory()
        {
            if (battleEnded || !EnsureGridManager())
            {
                return false;
            }

            var victoryCondition = currentStageData != null ? currentStageData.VictoryCondition : VictoryConditionType.DefeatAllEnemies;
            switch (victoryCondition)
            {
                case VictoryConditionType.ReachGoal:
                    if (!HasPlayerReachedGoal())
                    {
                        return false;
                    }

                    Debug.Log("Victory: player reached goal");
                    BattleUI.Instance?.AddBattleLog("Victory: player reached goal");
                    EndBattle(BattleResult.Victory, "VICTORY\nPress Enter: Next Stage");
                    return true;
                default:
                    if (gridManager.GetUnitsByFaction(Faction.Enemy).Count > 0)
                    {
                        return false;
                    }

                    Debug.Log("Victory: all enemies defeated");
                    BattleUI.Instance?.AddBattleLog("Victory: all enemies defeated");
                    EndBattle(BattleResult.Victory, "VICTORY\nPress Enter: Next Stage");
                    return true;
            }
        }

        private bool CheckDefeat()
        {
            if (battleEnded || !EnsureGridManager())
            {
                return false;
            }

            if (HasDefeatCondition(DefeatConditionType.AllPlayersDefeated) && gridManager.GetUnitsByFaction(Faction.Player).Count == 0)
            {
                Debug.Log("Defeat: all player units defeated");
                BattleUI.Instance?.AddBattleLog("Defeat: all player units defeated");
                EndBattle(BattleResult.Defeat, "DEFEAT\nPress Enter: Retry");
                return true;
            }

            if (HasDefeatCondition(DefeatConditionType.TurnLimitExceeded) && currentStageData != null && currentStageData.TurnLimit > 0 && turnNumber > currentStageData.TurnLimit)
            {
                Debug.Log("Defeat: turn limit exceeded");
                BattleUI.Instance?.AddBattleLog("Defeat: turn limit exceeded");
                EndBattle(BattleResult.Defeat, "DEFEAT\nPress Enter: Retry");
                return true;
            }

            return false;
        }

        private void EndBattle(BattleResult battleResult, string resultText)
        {
            battleEnded = true;
            result = battleResult;
            AudioManager.Instance?.StopBgm();
            if (battleResult == BattleResult.Victory)
            {
                AudioManager.Instance?.PlayVictorySe();
            }
            else if (battleResult == BattleResult.Defeat)
            {
                AudioManager.Instance?.PlayDefeatSe();
            }

            BattleUI.Instance?.ClearAttackPreview();
            var evaluation = BuildClearEvaluation(battleResult);
            LogClearEvaluation(evaluation);
            BattleUI.Instance?.ShowResult(
                resultText,
                evaluation.Rating,
                evaluation.TurnNumber,
                evaluation.TurnLimitText,
                evaluation.PlayersAlive,
                evaluation.PlayerHpTotal,
                evaluation.EnemiesAlive,
                evaluation.EnemyHpTotal);
        }

        private ClearEvaluation BuildClearEvaluation(BattleResult battleResult)
        {
            if (!EnsureGridManager())
            {
                return new ClearEvaluation
                {
                    BattleResult = battleResult,
                    Rating = GetClearEvaluationRating(battleResult),
                    TurnNumber = turnNumber,
                    TurnLimitText = currentStageData != null && currentStageData.TurnLimit > 0 ? currentStageData.TurnLimit.ToString() : "None",
                    Objective = currentStageData != null ? currentStageData.VictoryCondition.ToString() : VictoryConditionType.DefeatAllEnemies.ToString(),
                    StageLabel = "Stage ?"
                };
            }

            var players = gridManager.GetUnitsByFaction(Faction.Player);
            var enemies = gridManager.GetUnitsByFaction(Faction.Enemy);
            var playerHpTotal = 0;
            var enemyHpTotal = 0;

            foreach (var player in players)
            {
                playerHpTotal += player.CurrentHp;
            }

            foreach (var enemy in enemies)
            {
                enemyHpTotal += enemy.CurrentHp;
            }

            var stageNumber = StageManager.Instance != null ? StageManager.Instance.CurrentStageNumber : 0;
            var totalStages = StageManager.Instance != null ? StageManager.Instance.TotalStages : 0;
            var stageLabel = stageNumber > 0 && totalStages > 0 ? $"Stage {stageNumber}/{totalStages}" : "Stage ?";
            var rating = GetClearEvaluationRating(battleResult);
            var objective = currentStageData != null ? currentStageData.VictoryCondition.ToString() : VictoryConditionType.DefeatAllEnemies.ToString();
            var turnLimit = currentStageData != null && currentStageData.TurnLimit > 0 ? currentStageData.TurnLimit.ToString() : "None";

            return new ClearEvaluation
            {
                BattleResult = battleResult,
                StageLabel = stageLabel,
                Rating = rating,
                TurnNumber = turnNumber,
                TurnLimitText = turnLimit,
                Objective = objective,
                PlayersAlive = players.Count,
                PlayerHpTotal = playerHpTotal,
                EnemiesAlive = enemies.Count,
                EnemyHpTotal = enemyHpTotal
            };
        }

        private void LogClearEvaluation(ClearEvaluation evaluation)
        {
            var evaluationLog =
                $"Clear Evaluation: {evaluation.StageLabel} | Result: {evaluation.BattleResult} | Rating: {evaluation.Rating} | Turn: {evaluation.TurnNumber} | Limit: {evaluation.TurnLimitText} | Objective: {evaluation.Objective} | Players Alive: {evaluation.PlayersAlive} | Player HP Total: {evaluation.PlayerHpTotal} | Enemies Alive: {evaluation.EnemiesAlive} | Enemy HP Total: {evaluation.EnemyHpTotal}";

            Debug.Log(evaluationLog);
            BattleUI.Instance?.AddBattleLog($"Evaluation: {evaluation.BattleResult} T{evaluation.TurnNumber} {evaluation.Rating}");
        }

        private string GetClearEvaluationRating(BattleResult battleResult)
        {
            if (battleResult != BattleResult.Victory)
            {
                return "FAILED";
            }

            if (currentStageData == null || currentStageData.TurnLimit <= 0)
            {
                return "CLEAR";
            }

            var remainingTurns = Mathf.Max(0, currentStageData.TurnLimit - turnNumber);
            if (remainingTurns >= 3)
            {
                return "S";
            }

            if (remainingTurns >= 1)
            {
                return "A";
            }

            return "B";
        }

        private bool HasPlayerReachedGoal()
        {
            var players = gridManager.GetUnitsByFaction(Faction.Player);
            foreach (var player in players)
            {
                if (player == null || player.IsDead)
                {
                    continue;
                }

                var tile = gridManager.GetTile(player.GridPosition);
                if (tile != null && tile.TerrainType == TileTerrainType.Goal)
                {
                    return true;
                }
            }

            return false;
        }

        private struct ClearEvaluation
        {
            public BattleResult BattleResult;
            public string StageLabel;
            public string Rating;
            public int TurnNumber;
            public string TurnLimitText;
            public string Objective;
            public int PlayersAlive;
            public int PlayerHpTotal;
            public int EnemiesAlive;
            public int EnemyHpTotal;
        }

        private sealed class AttackAttemptResult
        {
            public bool Succeeded;
        }

        private bool HasDefeatCondition(DefeatConditionType condition)
        {
            if (currentStageData == null)
            {
                return false;
            }

            if (currentStageData.DefeatConditions == null || currentStageData.DefeatConditions.Count == 0)
            {
                return condition == DefeatConditionType.AllPlayersDefeated;
            }

            return currentStageData.DefeatConditions.Contains(condition);
        }

        private bool HaveAllPlayerUnitsActed()
        {
            if (!EnsureGridManager())
            {
                return false;
            }

            var hasPlayerUnit = false;

            foreach (var unit in gridManager.Units)
            {
                if (unit == null || unit.IsDead || !unit.IsPlayerControlled)
                {
                    continue;
                }

                hasPlayerUnit = true;
                if (!unit.HasActed)
                {
                    return false;
                }
            }

            return hasPlayerUnit;
        }

        private void ResetPlayerUnitActions()
        {
            if (!EnsureGridManager())
            {
                return;
            }

            foreach (var unit in gridManager.Units)
            {
                if (unit != null && !unit.IsDead && unit.IsPlayerControlled)
                {
                    unit.SetHasActed(false);
                }
            }
        }

        private void ResetEnemyUnitActions()
        {
            if (!EnsureGridManager())
            {
                return;
            }

            foreach (var unit in gridManager.Units)
            {
                if (unit != null && !unit.IsDead && unit.Faction == Faction.Enemy)
                {
                    unit.SetHasActed(false);
                }
            }
        }

        private bool EnsureGridManager()
        {
            if (gridManager == null)
            {
                gridManager = FindAnyObjectByType<GridManager>();
            }

            return gridManager != null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapTurnManager()
        {
            if (FindAnyObjectByType<TurnManager>() == null)
            {
                var turnObject = new GameObject("TurnManager");
                turnObject.AddComponent<TurnManager>();
            }
        }
    }
}
