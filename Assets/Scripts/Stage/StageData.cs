using System.Collections.Generic;
using SRPG.Grid;
using SRPG.Units;
using UnityEngine;

namespace SRPG.Stage
{
    public enum VictoryConditionType
    {
        DefeatAllEnemies,
        ReachGoal
    }

    public enum DefeatConditionType
    {
        AllPlayersDefeated,
        TurnLimitExceeded
    }

    [System.Serializable]
    public class StageData
    {
        public int Width = 8;
        public int Height = 8;
        public List<UnitSpawnData> Units = new List<UnitSpawnData>();
        public List<TileTerrainData> Terrains = new List<TileTerrainData>();
        public VictoryConditionType VictoryCondition = VictoryConditionType.DefeatAllEnemies;
        public List<DefeatConditionType> DefeatConditions = new List<DefeatConditionType>();
        public string DisplayName = "Untitled Stage";
        public string ThemeName = "Prototype";
        public string DifficultyLabel = "Normal";
        public string Description = "Clear the stage objective.";
        public int TurnLimit;

        public StageData()
        {
        }

        public static StageData CreateDefaultStage()
        {
            var stages = CreateDefaultStages();
            return stages.Count > 0 ? stages[0] : new StageData();
        }

        public static List<StageData> CreateDefaultStages()
        {
            return new List<StageData>
            {
                CreateStageOne(),
                CreateStageFour(),
                CreateStageSix(),
                CreateStageFive(),
                CreateStageTwo(),
                CreateStageThree()
            };
        }

        private static StageData CreateStageOne()
        {
            var stage = new StageData
            {
                Width = 8,
                Height = 8,
                VictoryCondition = VictoryConditionType.DefeatAllEnemies,
                DisplayName = "Opening Formation",
                ThemeName = "Basic Movement and Attacks",
                DifficultyLabel = "Intro",
                Description = "Learn the core flow: select allies, move, attack, wait, and read simple enemy positions.",
                TurnLimit = 0
            };
            stage.DefeatConditions.Add(DefeatConditionType.AllPlayersDefeated);

            stage.Units.Add(new UnitSpawnData("Stage1_Player_A", Faction.Player, new Vector2Int(1, 1), 10, 5, 3, 1, UnitType.Soldier));
            stage.Units.Add(new UnitSpawnData("Stage1_Player_B", Faction.Player, new Vector2Int(1, 2), 8, 4, 3, 3, UnitType.Archer));
            stage.Units.Add(new UnitSpawnData("Stage1_Enemy_A", Faction.Enemy, new Vector2Int(5, 5), 10, 5, 3, 1, UnitType.Soldier, EnemyAIType.Aggressive));
            stage.Units.Add(new UnitSpawnData("Stage1_Enemy_B", Faction.Enemy, new Vector2Int(6, 5), 8, 4, 3, 3, UnitType.Archer, EnemyAIType.Stationary));

            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 3), TileTerrainType.Obstacle));

            return stage;
        }

        private static StageData CreateStageTwo()
        {
            var stage = new StageData
            {
                Width = 8,
                Height = 8,
                VictoryCondition = VictoryConditionType.DefeatAllEnemies,
                DisplayName = "Last Turn Breakthrough",
                ThemeName = "Route Optimization",
                DifficultyLabel = "Hard+",
                Description = "Every action matters. Use both allies efficiently to break the guardian before the limit expires.",
                TurnLimit = 6
            };
            stage.DefeatConditions.Add(DefeatConditionType.AllPlayersDefeated);
            stage.DefeatConditions.Add(DefeatConditionType.TurnLimitExceeded);

            stage.Units.Add(new UnitSpawnData("Stage5_Player_A", Faction.Player, new Vector2Int(1, 1), 10, 5, 3, 1, UnitType.Soldier));
            stage.Units.Add(new UnitSpawnData("Stage5_Player_B", Faction.Player, new Vector2Int(1, 2), 7, 4, 5, 1, UnitType.Rogue));
            stage.Units.Add(new UnitSpawnData("Stage5_Enemy_A", Faction.Enemy, new Vector2Int(6, 5), 10, 5, 3, 1, UnitType.Soldier, EnemyAIType.Aggressive));
            stage.Units.Add(new UnitSpawnData("Stage5_Enemy_B", Faction.Enemy, new Vector2Int(6, 3), 12, 6, 2, 1, UnitType.Knight, EnemyAIType.Guardian));
            stage.Units.Add(new UnitSpawnData("Stage5_Enemy_C", Faction.Enemy, new Vector2Int(5, 6), 8, 4, 3, 3, UnitType.Archer, EnemyAIType.Stationary));

            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 1), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 5), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 5), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(5, 5), TileTerrainType.Obstacle));

            return stage;
        }

        private static StageData CreateStageThree()
        {
            var stage = new StageData
            {
                Width = 8,
                Height = 8,
                VictoryCondition = VictoryConditionType.ReachGoal,
                DisplayName = "Final Escape",
                ThemeName = "Goal Route Under Fire",
                DifficultyLabel = "Final",
                Description = "Do not chase every enemy. Create one escape route and reach the goal before the line collapses.",
                TurnLimit = 7
            };
            stage.DefeatConditions.Add(DefeatConditionType.AllPlayersDefeated);
            stage.DefeatConditions.Add(DefeatConditionType.TurnLimitExceeded);

            stage.Units.Add(new UnitSpawnData("Stage6_Player_A", Faction.Player, new Vector2Int(1, 1), 16, 6, 2, 1, UnitType.Knight));
            stage.Units.Add(new UnitSpawnData("Stage6_Player_B", Faction.Player, new Vector2Int(2, 1), 8, 5, 3, 3, UnitType.Archer));
            stage.Units.Add(new UnitSpawnData("Stage6_Enemy_A", Faction.Enemy, new Vector2Int(5, 5), 10, 5, 3, 1, UnitType.Soldier, EnemyAIType.Aggressive));
            stage.Units.Add(new UnitSpawnData("Stage6_Enemy_B", Faction.Enemy, new Vector2Int(6, 5), 16, 6, 2, 1, UnitType.Knight, EnemyAIType.Guardian));
            stage.Units.Add(new UnitSpawnData("Stage6_Enemy_C", Faction.Enemy, new Vector2Int(5, 6), 8, 4, 3, 3, UnitType.Archer, EnemyAIType.WeakTarget));
            stage.Units.Add(new UnitSpawnData("Stage6_Enemy_D", Faction.Enemy, new Vector2Int(6, 6), 7, 4, 5, 1, UnitType.Rogue, EnemyAIType.Aggressive));

            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(5, 1), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(5, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(6, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(7, 6), TileTerrainType.Goal));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(7, 7), TileTerrainType.Goal));

            return stage;
        }

        private static StageData CreateStageFour()
        {
            var stage = new StageData
            {
                Width = 8,
                Height = 8,
                VictoryCondition = VictoryConditionType.DefeatAllEnemies,
                DisplayName = "Crossfire Lanes",
                ThemeName = "Threat Display and Safe Lanes",
                DifficultyLabel = "Easy+",
                Description = "Use threat display to find safe lanes through fixed archer coverage.",
                TurnLimit = 7
            };
            stage.DefeatConditions.Add(DefeatConditionType.AllPlayersDefeated);
            stage.DefeatConditions.Add(DefeatConditionType.TurnLimitExceeded);

            // Fixed archer batteries cover the center; use threat display to find the lower and upper approach lanes.
            stage.Units.Add(new UnitSpawnData("Stage2_Player_A", Faction.Player, new Vector2Int(1, 1), 10, 5, 3, 1, UnitType.Soldier));
            stage.Units.Add(new UnitSpawnData("Stage2_Player_B", Faction.Player, new Vector2Int(1, 2), 8, 4, 3, 3, UnitType.Archer));
            stage.Units.Add(new UnitSpawnData("Stage2_Player_C", Faction.Player, new Vector2Int(0, 1), 7, 4, 5, 1, UnitType.Rogue));
            stage.Units.Add(new UnitSpawnData("Stage2_Enemy_A", Faction.Enemy, new Vector2Int(5, 5), 8, 3, 0, 3, UnitType.Archer, EnemyAIType.Stationary));
            stage.Units.Add(new UnitSpawnData("Stage2_Enemy_B", Faction.Enemy, new Vector2Int(6, 2), 8, 3, 0, 3, UnitType.Archer, EnemyAIType.Stationary));
            stage.Units.Add(new UnitSpawnData("Stage2_Enemy_C", Faction.Enemy, new Vector2Int(5, 3), 10, 5, 3, 1, UnitType.Soldier, EnemyAIType.Aggressive));
            stage.Units.Add(new UnitSpawnData("Stage2_Enemy_D", Faction.Enemy, new Vector2Int(7, 4), 6, 4, 5, 1, UnitType.Rogue, EnemyAIType.Aggressive));

            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 0), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 1), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(5, 1), TileTerrainType.Obstacle));

            return stage;
        }

        private static StageData CreateStageFive()
        {
            var stage = new StageData
            {
                Width = 8,
                Height = 8,
                VictoryCondition = VictoryConditionType.DefeatAllEnemies,
                DisplayName = "Guardian Split",
                ThemeName = "Pulling Guardians Separately",
                DifficultyLabel = "Hard",
                Description = "Pull the guardians apart, soften them safely, then finish before they regroup.",
                TurnLimit = 8
            };
            stage.DefeatConditions.Add(DefeatConditionType.AllPlayersDefeated);
            stage.DefeatConditions.Add(DefeatConditionType.TurnLimitExceeded);

            // Two guardians can be pulled separately if the player checks each reaction range before entering.
            stage.Units.Add(new UnitSpawnData("Stage4_Player_A", Faction.Player, new Vector2Int(1, 1), 16, 6, 2, 1, UnitType.Knight));
            stage.Units.Add(new UnitSpawnData("Stage4_Player_B", Faction.Player, new Vector2Int(1, 2), 9, 5, 3, 3, UnitType.Archer));
            stage.Units.Add(new UnitSpawnData("Stage4_Player_C", Faction.Player, new Vector2Int(0, 1), 8, 4, 5, 1, UnitType.Rogue));
            stage.Units.Add(new UnitSpawnData("Stage4_Enemy_A", Faction.Enemy, new Vector2Int(5, 1), 10, 5, 3, 1, UnitType.Soldier, EnemyAIType.Aggressive));
            stage.Units.Add(new UnitSpawnData("Stage4_Enemy_B", Faction.Enemy, new Vector2Int(5, 6), 8, 3, 0, 3, UnitType.Archer, EnemyAIType.Stationary));
            stage.Units.Add(new UnitSpawnData("Stage4_Enemy_C", Faction.Enemy, new Vector2Int(6, 3), 13, 4, 2, 1, UnitType.Knight, EnemyAIType.Guardian));
            stage.Units.Add(new UnitSpawnData("Stage4_Enemy_D", Faction.Enemy, new Vector2Int(6, 6), 13, 4, 2, 1, UnitType.Knight, EnemyAIType.Guardian));

            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 0), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 5), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 3), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 5), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(5, 3), TileTerrainType.Obstacle));

            return stage;
        }

        private static StageData CreateStageSix()
        {
            var stage = new StageData
            {
                Width = 8,
                Height = 8,
                VictoryCondition = VictoryConditionType.ReachGoal,
                DisplayName = "Goal Under Pressure",
                ThemeName = "Weak Target Chase",
                DifficultyLabel = "Medium",
                Description = "WeakTarget enemies punish exposed low-HP allies. Advance behind the knight and choose the safer goal route.",
                TurnLimit = 8
            };
            stage.DefeatConditions.Add(DefeatConditionType.AllPlayersDefeated);
            stage.DefeatConditions.Add(DefeatConditionType.TurnLimitExceeded);

            // WeakTarget enemies pressure the low-HP units; advance behind the knight and choose the safer route to the goal.
            stage.Units.Add(new UnitSpawnData("Stage3_Player_A", Faction.Player, new Vector2Int(1, 1), 16, 6, 2, 1, UnitType.Knight));
            stage.Units.Add(new UnitSpawnData("Stage3_Player_B", Faction.Player, new Vector2Int(1, 2), 8, 5, 3, 3, UnitType.Archer));
            stage.Units.Add(new UnitSpawnData("Stage3_Player_C", Faction.Player, new Vector2Int(0, 1), 7, 4, 4, 1, UnitType.Rogue));
            stage.Units.Add(new UnitSpawnData("Stage3_Enemy_A", Faction.Enemy, new Vector2Int(5, 5), 8, 4, 3, 3, UnitType.Archer, EnemyAIType.WeakTarget));
            stage.Units.Add(new UnitSpawnData("Stage3_Enemy_B", Faction.Enemy, new Vector2Int(6, 3), 7, 4, 5, 1, UnitType.Rogue, EnemyAIType.WeakTarget));
            stage.Units.Add(new UnitSpawnData("Stage3_Enemy_C", Faction.Enemy, new Vector2Int(5, 6), 13, 6, 2, 1, UnitType.Knight, EnemyAIType.Guardian));
            stage.Units.Add(new UnitSpawnData("Stage3_Enemy_D", Faction.Enemy, new Vector2Int(6, 6), 8, 3, 0, 3, UnitType.Archer, EnemyAIType.Stationary));

            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 1), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(3, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(4, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(5, 2), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(6, 4), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(7, 6), TileTerrainType.Obstacle));
            stage.Terrains.Add(new TileTerrainData(new Vector2Int(7, 7), TileTerrainType.Goal));

            return stage;
        }
    }

    [System.Serializable]
    public class UnitSpawnData
    {
        public string UnitName;
        public Faction Faction;
        public Vector2Int Position;
        public int MaxHp;
        public int AttackPower;
        public int MoveRange;
        public int AttackRange;
        public UnitType UnitType = UnitType.Soldier;
        public EnemyAIType EnemyAIType = EnemyAIType.Aggressive;

        public UnitSpawnData()
        {
        }

        public UnitSpawnData(
            string unitName,
            Faction faction,
            Vector2Int position,
            int maxHp,
            int attackPower,
            int moveRange,
            int attackRange,
            UnitType unitType = UnitType.Soldier,
            EnemyAIType enemyAIType = EnemyAIType.Aggressive)
        {
            UnitName = unitName;
            Faction = faction;
            Position = position;
            MaxHp = maxHp;
            AttackPower = attackPower;
            MoveRange = moveRange;
            AttackRange = attackRange;
            UnitType = unitType;
            EnemyAIType = enemyAIType;
        }
    }

    [System.Serializable]
    public class TileTerrainData
    {
        public Vector2Int Position;
        public TileTerrainType TerrainType;

        public TileTerrainData()
        {
        }

        public TileTerrainData(Vector2Int position, TileTerrainType terrainType)
        {
            Position = position;
            TerrainType = terrainType;
        }
    }
}
