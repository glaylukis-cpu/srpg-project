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
        [SerializeField] private Color lightTileColor = new Color(0.4f, 0.47f, 0.52f, 1f);
        [SerializeField] private Color darkTileColor = new Color(0.33f, 0.4f, 0.46f, 1f);
        [SerializeField] private Color boardSideColor = new Color(0.062f, 0.082f, 0.096f, 1f);
        [SerializeField] private Color boardEdgeHighlightColor = new Color(0.58f, 0.69f, 0.76f, 0.36f);

        [Header("Prototype")]
        [SerializeField] private bool generateOnStart = true;

        private const int GuardianReactionRange = 3;
        private const float IsoHorizontalStep = 0.6f;
        private const float IsoVerticalStep = 0.32f;
        private const float IsoTileWidth = 1.2f;
        private const float IsoTileHeight = 0.64f;
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

        internal void ClearOccupancyForTurnStartRestore()
        {
            foreach (var tile in tiles.Values)
            {
                tile.SetOccupant(null);
            }

            units.Clear();
        }

        internal bool RegisterRestoredUnit(Unit unit)
        {
            if (unit == null || unit.IsDead || !IsInsideGrid(unit.GridPosition))
            {
                return false;
            }

            var tile = GetTile(unit.GridPosition);
            if (tile == null || !tile.IsWalkable || tile.IsOccupied)
            {
                return false;
            }

            tile.SetOccupant(unit);
            units.Add(unit);
            UnitRegistered?.Invoke(unit);
            return true;
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
                        return new Color(0.41f, 0.48f, 0.53f, 1f);
                    case 1:
                        return new Color(0.38f, 0.45f, 0.51f, 1f);
                    case 2:
                        return new Color(0.39f, 0.46f, 0.52f, 1f);
                    default:
                        return lightTileColor;
                }
            }

            switch (variant)
            {
                case 0:
                    return new Color(0.32f, 0.39f, 0.45f, 1f);
                case 1:
                    return new Color(0.35f, 0.42f, 0.48f, 1f);
                case 2:
                    return new Color(0.34f, 0.4f, 0.47f, 1f);
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
            var tileHalfWidth = IsoTileWidth * cellSize * 0.5f;
            var tileHalfHeight = IsoTileHeight * cellSize * 0.5f;
            var frontX = 0f;
            var frontY = -tileHalfHeight;
            var rightX = (width - 1) * IsoHorizontalStep * cellSize + tileHalfWidth;
            var rightY = (width - 1) * IsoVerticalStep * cellSize;
            var backY = (width + height - 2) * IsoVerticalStep * cellSize + tileHalfHeight;
            var leftX = -(height - 1) * IsoHorizontalStep * cellSize - tileHalfWidth;
            var leftY = (height - 1) * IsoVerticalStep * cellSize;

            var leftVertex = new Vector3(leftX, leftY, 0.05f);
            var frontVertex = new Vector3(frontX, frontY, 0.05f);
            var rightVertex = new Vector3(rightX, rightY, 0.05f);
            var leftSideDrop = 0.86f;
            var rightSideDrop = 0.82f;

            CreateBoardFace("BoardFrontLeftDropShadow", leftVertex + new Vector3(0.16f, -leftSideDrop - 0.08f, 0.03f), frontVertex + new Vector3(0.16f, -leftSideDrop - 0.08f, 0.03f), 0.16f, new Color(0f, 0.006f, 0.012f, 0.2f), -995);
            CreateBoardFace("BoardFrontRightDropShadow", frontVertex + new Vector3(0.16f, -rightSideDrop - 0.08f, 0.03f), rightVertex + new Vector3(0.16f, -rightSideDrop - 0.08f, 0.03f), 0.15f, new Color(0f, 0.006f, 0.012f, 0.18f), -995);

            CreateBoardFace("BoardFrontLeftSide", leftVertex, frontVertex, leftSideDrop, boardSideColor, -994);
            CreateBoardFace("BoardFrontRightSide", frontVertex, rightVertex, rightSideDrop, new Color(0.074f, 0.096f, 0.112f, 1f), -994);
            CreateBoardStrip("BoardFrontLeftTopSeam", leftX, leftY - 0.035f, frontX, frontY - 0.035f, 0.045f, 0.075f, new Color(0.018f, 0.026f, 0.032f, 0.82f), -993);
            CreateBoardStrip("BoardFrontRightTopSeam", frontX, frontY - 0.035f, rightX, rightY - 0.035f, 0.045f, 0.075f, new Color(0.02f, 0.028f, 0.034f, 0.78f), -993);
            CreateBoardStrip("BoardFrontLeftBottomEdge", leftX, leftY - leftSideDrop, frontX, frontY - leftSideDrop, 0.045f, 0.14f, new Color(0.01f, 0.014f, 0.018f, 0.96f), -993);
            CreateBoardStrip("BoardFrontRightBottomEdge", frontX, frontY - rightSideDrop, rightX, rightY - rightSideDrop, 0.045f, 0.13f, new Color(0.012f, 0.016f, 0.02f, 0.92f), -993);

            CreateBoardStrip("BoardFrontLeftEdgeHighlight", leftX, leftY + 0.015f, frontX, frontY + 0.015f, 0.03f, 0.055f, boardEdgeHighlightColor, -993);
            CreateBoardStrip("BoardFrontRightEdgeHighlight", frontX, frontY + 0.015f, rightX, rightY + 0.015f, 0.03f, 0.055f, boardEdgeHighlightColor, -993);
        }

        private void CreateBoardFace(string objectName, Vector3 topStart, Vector3 topEnd, float dropY, Color color, int sortingOrder)
        {
            var faceObject = new GameObject(objectName);
            faceObject.transform.SetParent(transform);

            var mesh = new Mesh();
            mesh.vertices = new[]
            {
                topStart,
                topEnd,
                topEnd + new Vector3(0f, -dropY, 0f),
                topStart + new Vector3(0f, -dropY, 0f)
            };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateBounds();

            var filter = faceObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            var renderer = faceObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.material.color = color;
            renderer.sortingOrder = sortingOrder;
        }

        private void CreateBoardStrip(string objectName, float startX, float startY, float endX, float endY, float z, float thickness, Color color, int sortingOrder)
        {
            var deltaX = endX - startX;
            var deltaY = endY - startY;
            var length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            var angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI);

            var stripObject = new GameObject(objectName);
            stripObject.transform.SetParent(transform);
            stripObject.transform.position = new Vector3((startX + endX) * 0.5f, (startY + endY) * 0.5f, z);
            stripObject.transform.localScale = new Vector3(length, thickness, 1f);
            stripObject.transform.localEulerAngles = new Vector3(0f, 0f, angle);

            var renderer = stripObject.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
        }

    }
}
