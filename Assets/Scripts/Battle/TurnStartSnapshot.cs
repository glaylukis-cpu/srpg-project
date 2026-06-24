using System.Collections.Generic;
using SRPG.Grid;
using SRPG.Stage;
using SRPG.Units;
using UnityEngine;

namespace SRPG.Battle
{
    internal sealed class UnitSnapshot
    {
        public UnitSnapshot(Unit unit)
        {
            Unit = unit;
            GridPosition = unit.GridPosition;
            WorldPosition = unit.transform.position;
            CurrentHp = unit.CurrentHp;
            MaxHp = unit.MaxHp;
            IsDead = unit.IsDead;
            IsActive = unit.gameObject.activeSelf;
            HasActed = unit.HasActed;
            Faction = unit.Faction;
            UnitType = unit.UnitType;
            EnemyAIType = unit.EnemyAIType;
        }

        public Unit Unit { get; }
        public Vector2Int GridPosition { get; }
        public Vector3 WorldPosition { get; }
        public int CurrentHp { get; }
        public int MaxHp { get; }
        public bool IsDead { get; }
        public bool IsActive { get; }
        public bool HasActed { get; }
        public Faction Faction { get; }
        public UnitType UnitType { get; }
        public EnemyAIType EnemyAIType { get; }
    }

    internal sealed class TurnStartSnapshot
    {
        public TurnStartSnapshot(
            StageData stageData,
            GridManager gridManager,
            int turnNumber,
            List<UnitSnapshot> units,
            List<string> battleLogEntries,
            bool enemyThreatVisible)
        {
            StageData = stageData;
            GridManager = gridManager;
            TurnNumber = turnNumber;
            Units = units;
            BattleLogEntries = battleLogEntries;
            EnemyThreatVisible = enemyThreatVisible;
        }

        public StageData StageData { get; }
        public GridManager GridManager { get; }
        public int TurnNumber { get; }
        public IReadOnlyList<UnitSnapshot> Units { get; }
        public IReadOnlyList<string> BattleLogEntries { get; }
        public bool EnemyThreatVisible { get; }
    }
}
