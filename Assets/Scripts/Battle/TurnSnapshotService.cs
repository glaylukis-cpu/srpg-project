using System.Collections.Generic;
using SRPG.Grid;
using SRPG.Stage;
using SRPG.UI;
using SRPG.Units;
using UnityEngine;

namespace SRPG.Battle
{
    internal sealed class TurnSnapshotService
    {
        private TurnStartSnapshot snapshot;
        private bool isRestoring;

        public bool CanRestore(StageData stageData, GridManager gridManager, int turnNumber)
        {
            return !isRestoring
                && snapshot != null
                && ReferenceEquals(snapshot.StageData, stageData)
                && ReferenceEquals(snapshot.GridManager, gridManager)
                && snapshot.TurnNumber == turnNumber;
        }

        public void Capture(StageData stageData, GridManager gridManager, int turnNumber)
        {
            if (gridManager == null)
            {
                Invalidate();
                return;
            }

            var unitSnapshots = new List<UnitSnapshot>();
            foreach (var unit in gridManager.Units)
            {
                if (unit != null)
                {
                    unitSnapshots.Add(new UnitSnapshot(unit));
                }
            }

            var battleLogEntries = new List<string>();
            if (BattleUI.Instance != null)
            {
                battleLogEntries = BattleUI.Instance.CaptureBattleLog();
            }
            else
            {
                Debug.LogWarning("Battle Log was unavailable when the turn start snapshot was captured.");
            }

            var enemyThreatVisible = PlayerController.Instance != null && PlayerController.Instance.IsEnemyThreatVisible;
            snapshot = new TurnStartSnapshot(
                stageData,
                gridManager,
                turnNumber,
                unitSnapshots,
                battleLogEntries,
                enemyThreatVisible);
        }

        public bool Restore(StageData stageData, GridManager gridManager, int turnNumber, out bool enemyThreatVisible)
        {
            enemyThreatVisible = false;
            if (!CanRestore(stageData, gridManager, turnNumber) || !ValidateSnapshot(gridManager))
            {
                return false;
            }

            isRestoring = true;
            try
            {
                gridManager.ClearOccupancyForTurnStartRestore();

                foreach (var unitSnapshot in snapshot.Units)
                {
                    if (!unitSnapshot.Unit.RestoreTurnStartState(
                            gridManager,
                            unitSnapshot.GridPosition,
                            unitSnapshot.CurrentHp,
                            unitSnapshot.MaxHp,
                            unitSnapshot.IsDead,
                            unitSnapshot.IsActive,
                            unitSnapshot.HasActed,
                            unitSnapshot.Faction,
                            unitSnapshot.UnitType,
                            unitSnapshot.EnemyAIType))
                    {
                        Debug.LogError($"Turn start undo failed while restoring {unitSnapshot.Unit.name}.");
                        return false;
                    }
                }

                foreach (var unitSnapshot in snapshot.Units)
                {
                    if (!unitSnapshot.IsDead && !gridManager.RegisterRestoredUnit(unitSnapshot.Unit))
                    {
                        Debug.LogError($"Turn start undo failed while registering {unitSnapshot.Unit.name}.");
                        return false;
                    }
                }

                if (BattleUI.Instance != null)
                {
                    BattleUI.Instance.RestoreBattleLog(snapshot.BattleLogEntries);
                    BattleUI.Instance.AddBattleLog("Player turn restored.");
                }
                else
                {
                    Debug.LogWarning("Battle Log could not be restored because BattleUI is unavailable.");
                }

                enemyThreatVisible = snapshot.EnemyThreatVisible;
                return true;
            }
            finally
            {
                isRestoring = false;
            }
        }

        public void Invalidate()
        {
            snapshot = null;
            isRestoring = false;
        }

        private bool ValidateSnapshot(GridManager gridManager)
        {
            var unitReferences = new HashSet<Unit>();
            var occupiedCoordinates = new HashSet<Vector2Int>();

            foreach (var unitSnapshot in snapshot.Units)
            {
                var unit = unitSnapshot.Unit;
                if (unit == null || !unitReferences.Add(unit))
                {
                    Debug.LogError("Turn start undo snapshot contains a missing or duplicate unit reference.");
                    return false;
                }

                if (unit.MaxHp != unitSnapshot.MaxHp
                    || unit.Faction != unitSnapshot.Faction
                    || unit.UnitType != unitSnapshot.UnitType
                    || unit.EnemyAIType != unitSnapshot.EnemyAIType)
                {
                    Debug.LogError($"Turn start undo snapshot no longer matches {unit.name}.");
                    return false;
                }

                if (unitSnapshot.IsDead)
                {
                    continue;
                }

                var tile = gridManager.GetTile(unitSnapshot.GridPosition);
                if (tile == null || !tile.IsWalkable || !occupiedCoordinates.Add(unitSnapshot.GridPosition))
                {
                    Debug.LogError($"Turn start undo snapshot has an invalid position for {unit.name}.");
                    return false;
                }
            }

            return true;
        }
    }
}
