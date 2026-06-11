using SRPG.Grid;
using UnityEngine;

namespace SRPG.Stage
{
    public class StageLoader : MonoBehaviour
    {
        private GridManager gridManager;

        public static StageLoader Instance { get; private set; }

        public void LoadStage(StageData data)
        {
            if (data == null)
            {
                Debug.Log("Stage load failed: StageData is null.");
                return;
            }

            if (!EnsureGridManager())
            {
                Debug.Log("Stage load failed: GridManager not found.");
                return;
            }

            gridManager.GenerateGrid(data.Width, data.Height);
            gridManager.ApplyTerrains(data.Terrains);

            foreach (var unitData in data.Units)
            {
                if (unitData == null)
                {
                    continue;
                }

                gridManager.SpawnUnit(unitData);
            }

            Debug.Log($"Stage loaded: {data.Width}x{data.Height}, Units: {data.Units.Count}, Terrains: {data.Terrains.Count}");
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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
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
        private static void BootstrapStageLoader()
        {
            if (FindAnyObjectByType<StageLoader>() == null)
            {
                var loaderObject = new GameObject("StageLoader");
                loaderObject.AddComponent<StageLoader>();
            }
        }
    }
}
