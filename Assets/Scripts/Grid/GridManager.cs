using System;
using System.Collections.Generic;
using SRPG.Stage;
using SRPG.Units;
using SRPG.Visual;
using UnityEngine;

namespace SRPG.Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Color lightTileColor = new Color(0.42f, 0.5f, 0.58f, 1f);
        [SerializeField] private Color darkTileColor = new Color(0.35f, 0.43f, 0.51f, 1f);
        [SerializeField] private Color boardFrameColor = new Color(0.68f, 0.46f, 0.18f, 0.48f);
        [SerializeField] private Color boardBaseColor = new Color(0.018f, 0.04f, 0.065f, 1f);
        [SerializeField] private Color boardBackdropColor = new Color(0.005f, 0.012f, 0.026f, 1f);
        [SerializeField] private Color boardShadowColor = new Color(0f, 0f, 0f, 0.45f);
        [SerializeField] private Color battleMistColor = new Color(0.16f, 0.22f, 0.3f, 0.18f);
        [SerializeField] private Color battleLightColor = new Color(0.72f, 0.55f, 0.28f, 0.16f);

        [Header("Prototype")]
        [SerializeField] private bool generateOnStart = true;

        private const int GuardianReactionRange = 3;
        private readonly Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
        private readonly List<Unit> units = new List<Unit>();
        private Sprite squareSprite;

        public event Action<Tile> TileClicked;
        public event Action<Unit> UnitRegistered;

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;
        public IReadOnlyDictionary<Vector2Int, Tile> Tiles => tiles;
        public IReadOnlyList<Unit> Units => units;

        public void GenerateGrid()
        {
            ClearStage();
            squareSprite = squareSprite == null ? CreateSquareSprite() : squareSprite;
            CreateBoardFrame();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var coordinates = new Vector2Int(x, y);
                    var tileObject = new GameObject($"Tile_{x}_{y}");
                    tileObject.transform.SetParent(transform);
                    tileObject.transform.position = GridToWorld(coordinates);
                    tileObject.transform.localScale = BoardProjection.GetTileScale(cellSize);

                    var renderer = tileObject.AddComponent<SpriteRenderer>();
                    renderer.sprite = squareSprite;

                    var tile = tileObject.AddComponent<Tile>();
                    tile.Initialize(coordinates, GetTileColor(x, y));
                    tile.SetVisualSortingOrder(BoardProjection.GetTileSortingOrder(coordinates));
                    tile.Clicked += HandleTileClicked;

                    tiles.Add(coordinates, tile);
                }
            }
        }

        public void GenerateGrid(int newWidth, int newHeight)
        {
            width = Mathf.Max(1, newWidth);
            height = Mathf.Max(1, newHeight);
            GenerateGrid();
        }

        public Tile GetTile(Vector2Int coordinates)
        {
            tiles.TryGetValue(coordinates, out var tile);
            return tile;
        }

        public bool IsInsideGrid(Vector2Int coordinates)
        {
            return coordinates.x >= 0 && coordinates.x < width && coordinates.y >= 0 && coordinates.y < height;
        }

        public List<Unit> GetUnitsByFaction(Faction faction)
        {
            var matchingUnits = new List<Unit>();

            foreach (var unit in units)
            {
                if (unit != null && !unit.IsDead && unit.Faction == faction)
                {
                    matchingUnits.Add(unit);
                }
            }

            return matchingUnits;
        }

        public int GetManhattanDistance(Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }

        public Vector3 GridToWorld(Vector2Int coordinates)
        {
            return BoardProjection.GridToIsoWorld(coordinates, cellSize);
        }

        public bool TryPlaceUnit(Unit unit, Vector2Int destination)
        {
            if (unit == null || unit.IsDead || !IsInsideGrid(destination))
            {
                return false;
            }

            var destinationTile = GetTile(destination);
            if (destinationTile == null || !destinationTile.IsWalkable || (destinationTile.IsOccupied && destinationTile.Occupant != unit))
            {
                return false;
            }

            var currentTile = GetTile(unit.GridPosition);
            if (currentTile != null && currentTile.Occupant == unit)
            {
                currentTile.SetOccupant(null);
            }

            destinationTile.SetOccupant(unit);
            unit.SetGridPositionInternal(destination);
            unit.transform.position = BoardProjection.GetUnitWorldPosition(destination, cellSize);
            unit.SetVisualSortingOrder(BoardProjection.GetUnitSortingOrder(destination));

            if (!units.Contains(unit))
            {
                units.Add(unit);
                UnitRegistered?.Invoke(unit);
            }

            return true;
        }

        public Unit SpawnUnit(UnitSpawnData unitData)
        {
            if (unitData == null)
            {
                return null;
            }

            return SpawnUnit(
                unitData.UnitName,
                unitData.Position,
                unitData.Faction,
                unitData.MoveRange,
                unitData.MaxHp,
                unitData.AttackPower,
                unitData.AttackRange,
                unitData.UnitType,
                unitData.EnemyAIType);
        }

        public Unit SpawnUnit(
            string unitName,
            Vector2Int coordinates,
            Faction faction,
            int moveRange,
            int maxHp,
            int attackPower,
            int attackRange,
            UnitType unitType = UnitType.Soldier,
            EnemyAIType enemyAIType = EnemyAIType.Aggressive)
        {
            if (!IsInsideGrid(coordinates))
            {
                Debug.LogWarning($"Cannot spawn {unitName} outside the grid at ({coordinates.x}, {coordinates.y}).");
                return null;
            }

            var spawnTile = GetTile(coordinates);
            if (spawnTile == null || !spawnTile.IsWalkable)
            {
                Debug.LogWarning($"Cannot spawn {unitName} on obstacle tile at ({coordinates.x}, {coordinates.y}).");
                return null;
            }

            squareSprite = squareSprite == null ? CreateSquareSprite() : squareSprite;

            var unitObject = new GameObject(unitName);
            unitObject.transform.SetParent(transform);

            var renderer = unitObject.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.sortingOrder = 5;

            var unit = unitObject.AddComponent<Unit>();
            var color = GetUnitColor(faction);
            unit.InitializeForStage(this, unitName, faction, coordinates, maxHp, attackPower, moveRange, attackRange, unitType, enemyAIType, color);

            return unit;
        }

        public void ApplyTerrains(List<TileTerrainData> terrains)
        {
            if (terrains == null)
            {
                return;
            }

            foreach (var terrainData in terrains)
            {
                if (terrainData == null)
                {
                    continue;
                }

                if (!IsInsideGrid(terrainData.Position))
                {
                    Debug.LogWarning($"Terrain outside grid skipped at ({terrainData.Position.x}, {terrainData.Position.y}).");
                    continue;
                }

                var tile = GetTile(terrainData.Position);
                if (tile != null)
                {
                    tile.SetTerrain(terrainData.TerrainType);
                }
            }
        }

        public void RemoveUnit(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            var currentTile = GetTile(unit.GridPosition);
            if (currentTile != null && currentTile.Occupant == unit)
            {
                currentTile.SetOccupant(null);
            }

            units.Remove(unit);
        }

        public List<Tile> GetReachableTiles(Vector2Int start, int movePower)
        {
            var reachableTiles = new List<Tile>();
            if (!IsInsideGrid(start) || movePower <= 0)
            {
                return reachableTiles;
            }

            var visitedCosts = new Dictionary<Vector2Int, int> { [start] = 0 };
            var frontier = new Queue<Vector2Int>();
            frontier.Enqueue(start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                var currentCost = visitedCosts[current];

                foreach (var next in GetNeighbors(current))
                {
                    var nextCost = currentCost + 1;
                    if (nextCost > movePower || visitedCosts.ContainsKey(next))
                    {
                        continue;
                    }

                    var tile = GetTile(next);
                    if (tile == null || !tile.IsWalkable || tile.IsOccupied)
                    {
                        continue;
                    }

                    visitedCosts[next] = nextCost;
                    frontier.Enqueue(next);
                    reachableTiles.Add(tile);
                }
            }

            return reachableTiles;
        }

        public List<Tile> GetAttackRangeTiles(Unit unit)
        {
            var attackRangeTiles = new List<Tile>();
            if (unit == null || unit.IsDead)
            {
                return attackRangeTiles;
            }

            return GetAttackRangeTiles(unit.GridPosition, unit.AttackRange);
        }

        public List<Tile> GetAttackRangeTiles(Vector2Int origin, int attackRange)
        {
            var attackRangeTiles = new List<Tile>();
            attackRange = Mathf.Max(1, attackRange);

            for (var y = origin.y - attackRange; y <= origin.y + attackRange; y++)
            {
                for (var x = origin.x - attackRange; x <= origin.x + attackRange; x++)
                {
                    var coordinates = new Vector2Int(x, y);
                    if (!IsInsideGrid(coordinates) || coordinates.x == origin.x && coordinates.y == origin.y)
                    {
                        continue;
                    }

                    if (GetManhattanDistance(origin, coordinates) > attackRange)
                    {
                        continue;
                    }

                    var tile = GetTile(coordinates);
                    if (tile != null)
                    {
                        attackRangeTiles.Add(tile);
                    }
                }
            }

            return attackRangeTiles;
        }

        public HashSet<Tile> GetEnemyThreatTiles()
        {
            var enemyThreatTiles = new HashSet<Tile>();
            var enemies = GetUnitsByFaction(Faction.Enemy);

            foreach (var enemy in enemies)
            {
                foreach (var tile in GetEnemyThreatTiles(enemy))
                {
                    enemyThreatTiles.Add(tile);
                }
            }

            return enemyThreatTiles;
        }

        public HashSet<Tile> GetEnemyMoveThreatTiles()
        {
            var enemyMoveThreatTiles = new HashSet<Tile>();
            var enemies = GetUnitsByFaction(Faction.Enemy);

            foreach (var enemy in enemies)
            {
                foreach (var tile in GetEnemyMoveThreatTiles(enemy))
                {
                    enemyMoveThreatTiles.Add(tile);
                }
            }

            return enemyMoveThreatTiles;
        }

        public HashSet<Tile> GetGuardianReactionTiles()
        {
            var guardianReactionTiles = new HashSet<Tile>();
            var enemies = GetUnitsByFaction(Faction.Enemy);

            foreach (var enemy in enemies)
            {
                foreach (var tile in GetGuardianReactionTiles(enemy))
                {
                    guardianReactionTiles.Add(tile);
                }
            }

            return guardianReactionTiles;
        }

        public HashSet<Tile> GetEnemyThreatTiles(Unit enemy)
        {
            var enemyThreatTiles = new HashSet<Tile>();
            if (enemy == null || enemy.IsDead || enemy.Faction != Faction.Enemy)
            {
                return enemyThreatTiles;
            }

            if (enemy.EnemyAIType == EnemyAIType.Guardian && !IsPlayerInsideGuardianReactionRange(enemy))
            {
                return enemyThreatTiles;
            }

            foreach (var tile in GetAttackRangeTiles(enemy))
            {
                enemyThreatTiles.Add(tile);
            }

            if (!CanEnemyMoveForThreat(enemy))
            {
                return enemyThreatTiles;
            }

            foreach (var moveTile in GetReachableTiles(enemy.GridPosition, enemy.MovePower))
            {
                foreach (var attackTile in GetAttackRangeTiles(moveTile.Coordinates, enemy.AttackRange))
                {
                    enemyThreatTiles.Add(attackTile);
                }
            }

            return enemyThreatTiles;
        }

        public HashSet<Tile> GetEnemyMoveThreatTiles(Unit enemy)
        {
            var enemyMoveThreatTiles = new HashSet<Tile>();
            if (enemy == null || enemy.IsDead || enemy.Faction != Faction.Enemy || !CanEnemyMoveForThreat(enemy))
            {
                return enemyMoveThreatTiles;
            }

            foreach (var tile in GetReachableTiles(enemy.GridPosition, enemy.MovePower))
            {
                enemyMoveThreatTiles.Add(tile);
            }

            return enemyMoveThreatTiles;
        }

        public HashSet<Tile> GetGuardianReactionTiles(Unit enemy)
        {
            var guardianReactionTiles = new HashSet<Tile>();
            if (enemy == null || enemy.IsDead || enemy.Faction != Faction.Enemy || enemy.EnemyAIType != EnemyAIType.Guardian)
            {
                return guardianReactionTiles;
            }

            var origin = enemy.InitialGridPosition;
            for (var y = origin.y - GuardianReactionRange; y <= origin.y + GuardianReactionRange; y++)
            {
                for (var x = origin.x - GuardianReactionRange; x <= origin.x + GuardianReactionRange; x++)
                {
                    var coordinates = new Vector2Int(x, y);
                    if (!IsInsideGrid(coordinates) || GetManhattanDistance(origin, coordinates) > GuardianReactionRange)
                    {
                        continue;
                    }

                    var tile = GetTile(coordinates);
                    if (tile == null || !tile.IsWalkable)
                    {
                        continue;
                    }

                    guardianReactionTiles.Add(tile);
                }
            }

            return guardianReactionTiles;
        }

        public void ClearMoveHighlights()
        {
            foreach (var tile in tiles.Values)
            {
                tile.SetMoveHighlight(false);
            }
        }

        public void ClearPlayerHighlights()
        {
            foreach (var tile in tiles.Values)
            {
                tile.ClearPlayerHighlights();
            }
        }

        public void ClearEnemyThreatHighlights()
        {
            foreach (var tile in tiles.Values)
            {
                tile.SetEnemyThreatHighlight(false);
                tile.SetEnemyMoveThreatHighlight(false);
                tile.SetGuardianReactionHighlight(false);
            }
        }

        public void ClearAllHighlights()
        {
            foreach (var tile in tiles.Values)
            {
                tile.ClearAllHighlights();
            }
        }

        public void ClearStage()
        {
            foreach (var tile in tiles.Values)
            {
                if (tile != null)
                {
                    tile.Clicked -= HandleTileClicked;
                }
            }

            tiles.Clear();
            units.Clear();

            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void Start()
        {
            if (!generateOnStart)
            {
                return;
            }

            GenerateGrid();
        }

        private IEnumerable<Vector2Int> GetNeighbors(Vector2Int coordinates)
        {
            var directions = new[]
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            };

            foreach (var direction in directions)
            {
                var next = coordinates + direction;
                if (IsInsideGrid(next))
                {
                    yield return next;
                }
            }
        }

        private bool CanEnemyMoveForThreat(Unit enemy)
        {
            switch (enemy.EnemyAIType)
            {
                case EnemyAIType.Stationary:
                    return false;
                case EnemyAIType.Guardian:
                    return IsPlayerInsideGuardianReactionRange(enemy);
                default:
                    return true;
            }
        }

        private bool IsPlayerInsideGuardianReactionRange(Unit guardian)
        {
            foreach (var player in GetUnitsByFaction(Faction.Player))
            {
                if (player == null || player.IsDead)
                {
                    continue;
                }

                if (GetManhattanDistance(guardian.InitialGridPosition, player.GridPosition) <= GuardianReactionRange)
                {
                    return true;
                }
            }

            return false;
        }

        private Color GetUnitColor(Faction faction)
        {
            return faction == Faction.Player
                ? new Color(0.15f, 0.48f, 1f, 1f)
                : new Color(0.9f, 0.12f, 0.16f, 1f);
        }

        private Color GetTileColor(int x, int y)
        {
            var variant = Mathf.Abs(x * 17 + y * 31 + x * y * 7) % 4;
            if ((x + y) % 2 == 0)
            {
                switch (variant)
                {
                    case 0:
                        return new Color(0.44f, 0.52f, 0.6f, 1f);
                    case 1:
                        return new Color(0.4f, 0.48f, 0.56f, 1f);
                    case 2:
                        return new Color(0.43f, 0.5f, 0.59f, 1f);
                    default:
                        return lightTileColor;
                }
            }

            switch (variant)
            {
                case 0:
                    return new Color(0.34f, 0.42f, 0.5f, 1f);
                case 1:
                    return new Color(0.37f, 0.45f, 0.53f, 1f);
                case 2:
                    return new Color(0.35f, 0.43f, 0.51f, 1f);
                default:
                    return darkTileColor;
            }
        }

        private void HandleTileClicked(Tile tile)
        {
            TileClicked?.Invoke(tile);
        }

        private static Sprite CreateSquareSprite()
        {
            var texture = new Texture2D(1, 1);
            texture.filterMode = FilterMode.Point;
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private void CreateBoardFrame()
        {
            var center = BoardProjection.GetBoardCenter(width, height, cellSize) + new Vector3(0f, 0f, 0.18f);
            var projectedWidth = BoardProjection.GetBoardWidth(width, height, cellSize);
            var projectedHeight = BoardProjection.GetBoardHeight(width, height, cellSize);

            CreateWorldPanel("BattleWorldBackdrop", center + new Vector3(0f, 0f, 0.08f), new Vector3(projectedWidth + 8.4f, projectedHeight + 4.2f, 1f), boardBackdropColor, -1000);
            CreateWorldPanel("BattleDistantWall", center + new Vector3(0f, projectedHeight * 0.22f, 0.07f), new Vector3(projectedWidth + 6.8f, 1.8f, 1f), new Color(0.02f, 0.055f, 0.08f, 0.72f), -999);
            CreateWorldPanel("BattleLowerFog", center + new Vector3(0f, -projectedHeight * 0.44f, 0.06f), new Vector3(projectedWidth + 8f, 1.45f, 1f), battleMistColor, -998);
            CreateWorldPanel("BattleUpperLight", center + new Vector3(-1.2f, projectedHeight * 0.43f, 0.05f), new Vector3(projectedWidth + 3f, 0.72f, 1f), battleLightColor, -997);
            CreateWorldPanel("BattleLeftPillar", center + new Vector3(-projectedWidth * 0.5f - 1.2f, 0.45f, 0.04f), new Vector3(0.44f, projectedHeight + 2.4f, 1f), new Color(0f, 0f, 0f, 0.28f), -996);
            CreateWorldPanel("BattleRightPillar", center + new Vector3(projectedWidth * 0.5f + 1.2f, 0.05f, 0.04f), new Vector3(0.52f, projectedHeight + 2.1f, 1f), new Color(0f, 0f, 0f, 0.24f), -996);
            CreateWorldPanel("BoardShadow", center + new Vector3(0.18f, -0.22f, 0.08f), new Vector3(projectedWidth + 1.18f, projectedHeight + 1.12f, 1f), boardShadowColor, -995);
            CreateWorldPanel("BoardFrontLip", center + new Vector3(0f, -projectedHeight * 0.5f - 0.14f, 0.05f), new Vector3(projectedWidth + 0.36f, 0.16f, 1f), new Color(0.33f, 0.22f, 0.09f, 0.42f), -994);

            var frameObject = new GameObject("BoardFrame");
            frameObject.transform.SetParent(transform);
            frameObject.transform.position = center;
            frameObject.transform.localScale = new Vector3(projectedWidth + 0.56f, projectedHeight + 0.56f, 1f);
            var frameRenderer = frameObject.AddComponent<SpriteRenderer>();
            frameRenderer.sprite = squareSprite;
            frameRenderer.color = boardFrameColor;
            frameRenderer.sortingOrder = -994;

            var baseObject = new GameObject("BoardBase");
            baseObject.transform.SetParent(transform);
            baseObject.transform.position = center + new Vector3(0f, 0f, -0.01f);
            baseObject.transform.localScale = new Vector3(projectedWidth + 0.42f, projectedHeight + 0.42f, 1f);
            var baseRenderer = baseObject.AddComponent<SpriteRenderer>();
            baseRenderer.sprite = squareSprite;
            baseRenderer.color = boardBaseColor;
            baseRenderer.sortingOrder = -993;

            CreateWorldPanel("BoardInnerLight", center + new Vector3(-0.7f, 0.8f, -0.02f), new Vector3(projectedWidth - 0.6f, projectedHeight - 0.6f, 1f), new Color(0.55f, 0.62f, 0.72f, 0.08f), -992);
            CreateWorldPanel("BoardStageLight", center + new Vector3(-0.28f, 0.36f, -0.08f), new Vector3(projectedWidth - 1.2f, projectedHeight - 1.35f, 1f), new Color(0.9f, 0.72f, 0.38f, 0.08f), -991);
        }

        private void CreateWorldPanel(string objectName, Vector3 position, Vector3 scale, Color color, int sortingOrder)
        {
            var panelObject = new GameObject(objectName);
            panelObject.transform.SetParent(transform);
            panelObject.transform.position = position;
            panelObject.transform.localScale = scale;

            var renderer = panelObject.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
        }
    }
}
