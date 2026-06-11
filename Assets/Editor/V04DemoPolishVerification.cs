#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using SRPG.Battle;
using SRPG.Grid;
using SRPG.Stage;
using SRPG.UI;
using SRPG.Units;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRPG.EditorTools
{
    public static class V04DemoPolishVerification
    {
        private const string PassLog = "V04_DEMO_POLISH_TRANSITION_VERIFICATION_PASS";

        public static void Run()
        {
            try
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                var ui = CreateComponent<BattleUI>("BattleUI", typeof(Canvas));
                var gridManager = CreateComponent<GridManager>("GridManager");
                CreateComponent<PlayerController>("PlayerController");
                var turnManager = CreateComponent<TurnManager>("TurnManager");
                CreateComponent<StageLoader>("StageLoader");
                var stageManager = CreateComponent<StageManager>("StageManager");

                InvokePrivate(ui, "Awake");
                InvokePrivate(turnManager, "Awake");
                InvokePrivate(stageManager, "Awake");

                var stages = StageData.CreateDefaultStages();
                Assert(stages.Count == 6, $"Expected 6 stages, got {stages.Count}.");

                InvokePrivate(stageManager, "ShowTitleScreen");
                Assert(stageManager.IsTitleScreenOpen, "Title screen did not open.");
                Assert(!stageManager.IsStageSelectOpen, "Stage Select should be closed on Title.");
                Debug.Log("v0.4 transition check: Title screen opened.");

                InvokePrivate(stageManager, "ShowStageSelect");
                Assert(stageManager.IsStageSelectOpen, "Stage Select did not open.");
                Assert(!stageManager.IsTitleScreenOpen, "Title should be closed on Stage Select.");
                Debug.Log("v0.4 transition check: Stage Select opened.");

                InvokePrivate(stageManager, "ShowTitleScreen");
                Assert(stageManager.IsTitleScreenOpen, "Title screen did not reopen from Stage Select.");
                Debug.Log("v0.4 transition check: Stage Select -> Title passed.");

                InvokePrivate(stageManager, "ShowStageSelect");
                VerifyAllStageLoads(stageManager, gridManager, turnManager, stages);

                stageManager.RetryCurrentStage();
                Assert(stageManager.CurrentStageNumber == 6, "Retry should keep the current stage index.");
                VerifyLoadedStage(gridManager, turnManager, stages[5], 6);
                Debug.Log("v0.4 transition check: Retry current stage passed.");

                InvokePrivate(stageManager, "ShowStageSelect");
                Assert(stageManager.IsStageSelectOpen, "Battle -> Stage Select did not open.");
                Debug.Log("v0.4 transition check: Battle -> Stage Select passed.");

                InvokePrivate(stageManager, "ShowTitleScreen");
                Assert(stageManager.IsTitleScreenOpen, "Stage Select -> Title did not open after battle.");
                Debug.Log("v0.4 transition check: Stage Select -> Title after battle passed.");

                Debug.Log(PassLog);
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError($"V04_DEMO_POLISH_TRANSITION_VERIFICATION_FAIL: {exception.Message}\n{exception}");
                EditorApplication.Exit(1);
            }
        }

        private static void VerifyAllStageLoads(
            StageManager stageManager,
            GridManager gridManager,
            TurnManager turnManager,
            IReadOnlyList<StageData> stages)
        {
            for (var i = 0; i < stages.Count; i++)
            {
                stageManager.LoadStageAt(i);
                Assert(!stageManager.IsMenuOpen, $"Menu should be closed after loading Stage {i + 1}.");
                Assert(stageManager.CurrentStageNumber == i + 1, $"Current stage number mismatch after loading Stage {i + 1}.");
                VerifyLoadedStage(gridManager, turnManager, stages[i], i + 1);
                Debug.Log($"v0.4 transition check: Stage {i + 1}/6 loaded - {stages[i].DisplayName}.");
            }
        }

        private static void VerifyLoadedStage(GridManager gridManager, TurnManager turnManager, StageData stage, int stageNumber)
        {
            Assert(gridManager.Width == stage.Width, $"Stage {stageNumber} width mismatch.");
            Assert(gridManager.Height == stage.Height, $"Stage {stageNumber} height mismatch.");
            Assert(gridManager.Tiles.Count == stage.Width * stage.Height, $"Stage {stageNumber} tile count mismatch.");

            var expectedPlayers = 0;
            var expectedEnemies = 0;
            foreach (var unit in stage.Units)
            {
                if (unit.Faction == Faction.Player)
                {
                    expectedPlayers++;
                }
                else
                {
                    expectedEnemies++;
                }
            }

            Assert(gridManager.GetUnitsByFaction(Faction.Player).Count == expectedPlayers, $"Stage {stageNumber} player count mismatch.");
            Assert(gridManager.GetUnitsByFaction(Faction.Enemy).Count == expectedEnemies, $"Stage {stageNumber} enemy count mismatch.");
            Assert(turnManager.CurrentPhase == TurnPhase.PlayerTurn, $"Stage {stageNumber} should start on PlayerTurn.");
            Assert(turnManager.TurnNumber == 1, $"Stage {stageNumber} should start on Turn 1.");
            Assert(!turnManager.IsBattleEnded, $"Stage {stageNumber} should not start ended.");
            Assert(turnManager.Result == BattleResult.None, $"Stage {stageNumber} should start with no result.");
        }

        private static T CreateComponent<T>(string objectName, params Type[] extraComponents) where T : Component
        {
            var types = new List<Type>();
            if (extraComponents != null)
            {
                types.AddRange(extraComponents);
            }

            types.Add(typeof(T));
            var gameObject = new GameObject(objectName, types.ToArray());
            return gameObject.GetComponent<T>();
        }

        private static void InvokePrivate(object target, string methodName)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert(method != null, $"Private method not found: {target.GetType().Name}.{methodName}");
            method.Invoke(target, null);
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
#endif
