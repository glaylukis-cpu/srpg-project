using System;
using SRPG.Units;
using SRPG.Visual;
using UnityEngine;

namespace SRPG.Grid
{
    public enum TileTerrainType
    {
        Normal,
        Obstacle,
        Goal
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Tile : MonoBehaviour
    {
        private static readonly Vector2[] DiamondColliderPoints =
        {
            new Vector2(0f, 0.25f),
            new Vector2(0.5f, 0f),
            new Vector2(0f, -0.25f),
            new Vector2(-0.5f, 0f)
        };

        [SerializeField] private Color baseColor = new Color(0.36f, 0.43f, 0.49f, 1f);
        [SerializeField] private Color normalTerrainColor = new Color(0.36f, 0.43f, 0.49f, 1f);
        [SerializeField] private Color obstacleTerrainColor = new Color(0.27f, 0.25f, 0.21f, 1f);
        [SerializeField] private Color goalTerrainColor = new Color(0.04f, 0.74f, 0.6f, 1f);
        [SerializeField] private Color moveHighlightedColor = new Color(0.16f, 0.62f, 1f, 0.42f);
        [SerializeField] private Color attackHighlightedColor = new Color(1f, 0.18f, 0.12f, 0.42f);
        [SerializeField] private Color enemyMoveThreatHighlightedColor = new Color(0.36f, 0.13f, 0.72f, 0.48f);
        [SerializeField] private Color enemyAttackThreatHighlightedColor = new Color(1f, 0.28f, 0.12f, 0.72f);
        [SerializeField] private Color guardianReactionHighlightedColor = new Color(1f, 0.72f, 0.18f, 0.36f);

        private SpriteRenderer spriteRenderer;
        private SpriteRenderer tileSideRenderer;
        private SpriteRenderer highlightRenderer;
        private SpriteRenderer terrainDetailRenderer;
        private SpriteRenderer goalRuneRenderer;
        private SpriteRenderer goalHaloRenderer;
        private TileTerrainType terrainType = TileTerrainType.Normal;
        private bool isMoveHighlighted;
        private bool isAttackHighlighted;
        private bool isEnemyMoveThreatHighlighted;
        private bool isEnemyAttackThreatHighlighted;
        private bool isGuardianReactionHighlighted;
        private float goalPulseTimer;

        public event Action<Tile> Clicked;

        public Vector2Int Coordinates { get; private set; }
        public Unit Occupant { get; private set; }
        public TileTerrainType TerrainType => terrainType;
        public bool IsWalkable => terrainType != TileTerrainType.Obstacle;
        public bool IsOccupied => Occupant != null;
        public bool IsMoveHighlighted => isMoveHighlighted;
        public bool IsAttackHighlighted => isAttackHighlighted;
        public bool IsEnemyThreatHighlighted => isEnemyMoveThreatHighlighted || isEnemyAttackThreatHighlighted || isGuardianReactionHighlighted;
        public bool IsEnemyMoveThreatHighlighted => isEnemyMoveThreatHighlighted;
        public bool IsEnemyAttackThreatHighlighted => isEnemyAttackThreatHighlighted;
        public bool IsGuardianReactionHighlighted => isGuardianReactionHighlighted;

        public void Initialize(Vector2Int coordinates, Color color)
        {
            Coordinates = coordinates;
            terrainType = TileTerrainType.Normal;
            baseColor = color;
            normalTerrainColor = color;

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = TileVisualFactory.GetStoneTileSprite();
            spriteRenderer.color = baseColor;
            EnsureVisualLayers();
            SetVisualSortingOrder(BoardProjection.GetTileSortingOrder(coordinates));

            var polygonCollider = GetComponent<PolygonCollider2D>();
            polygonCollider.pathCount = 1;
            polygonCollider.SetPath(0, DiamondColliderPoints);
        }

        public void SetVisualSortingOrder(int tileSortingOrder)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            EnsureVisualLayers();
            tileSideRenderer.sortingOrder = tileSortingOrder - 1;
            spriteRenderer.sortingOrder = tileSortingOrder;
            terrainDetailRenderer.sortingOrder = tileSortingOrder + 1;
            goalHaloRenderer.sortingOrder = tileSortingOrder + 1;
            goalRuneRenderer.sortingOrder = tileSortingOrder + 2;
            highlightRenderer.sortingOrder = tileSortingOrder + 3;
        }

        public void SetOccupant(Unit unit)
        {
            Occupant = unit;
            RefreshColor();
        }

        public void SetMoveHighlight(bool value)
        {
            isMoveHighlighted = value;
            RefreshColor();
        }

        public void SetTerrain(TileTerrainType newTerrainType)
        {
            terrainType = newTerrainType;
            baseColor = GetTerrainColor(newTerrainType);
            RefreshColor();
        }

        public void SetAttackHighlight(bool value)
        {
            isAttackHighlighted = value;
            RefreshColor();
        }

        public void SetEnemyThreatHighlight(bool value)
        {
            isEnemyAttackThreatHighlighted = value;
            RefreshColor();
        }

        public void SetEnemyMoveThreatHighlight(bool value)
        {
            isEnemyMoveThreatHighlighted = value;
            RefreshColor();
        }

        public void SetGuardianReactionHighlight(bool value)
        {
            isGuardianReactionHighlighted = value;
            RefreshColor();
        }

        public void ClearPlayerHighlights()
        {
            isMoveHighlighted = false;
            isAttackHighlighted = false;
            RefreshColor();
        }

        public void ClearAllHighlights()
        {
            isMoveHighlighted = false;
            isAttackHighlighted = false;
            isEnemyMoveThreatHighlighted = false;
            isEnemyAttackThreatHighlighted = false;
            isGuardianReactionHighlighted = false;
            RefreshColor();
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (terrainType != TileTerrainType.Goal || goalRuneRenderer == null)
            {
                return;
            }

            goalPulseTimer += Time.deltaTime;
            var pulse = ((float)System.Math.Sin(goalPulseTimer * 2.4f) + 1f) * 0.5f;
            goalRuneRenderer.color = new Color(0.78f, 1f, 0.94f, 0.62f + 0.24f * pulse);
            if (goalHaloRenderer != null)
            {
                goalHaloRenderer.color = new Color(0.08f, 0.95f, 0.76f, 0.18f + 0.12f * pulse);
            }
        }

        private void OnMouseDown()
        {
            Clicked?.Invoke(this);
        }

        private void RefreshColor()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            EnsureVisualLayers();
            spriteRenderer.color = baseColor;
            RefreshTerrainDetail();

            if (isAttackHighlighted)
            {
                SetHighlight(attackHighlightedColor);
                return;
            }

            if (isMoveHighlighted && isEnemyAttackThreatHighlighted)
            {
                SetHighlight(Color.white, TileVisualFactory.GetMoveThreatOverlapSprite());
                return;
            }

            if (isMoveHighlighted)
            {
                SetHighlight(moveHighlightedColor, TileVisualFactory.GetMoveHighlightSprite());
                return;
            }

            if (isEnemyAttackThreatHighlighted)
            {
                SetHighlight(enemyAttackThreatHighlightedColor, TileVisualFactory.GetEnemyAttackThreatSprite());
                return;
            }

            if (isEnemyMoveThreatHighlighted)
            {
                SetHighlight(enemyMoveThreatHighlightedColor);
                return;
            }

            if (isGuardianReactionHighlighted)
            {
                SetHighlight(guardianReactionHighlightedColor);
                return;
            }

            SetHighlight(new Color(0f, 0f, 0f, 0f));
        }

        private Color GetTerrainColor(TileTerrainType value)
        {
            switch (value)
            {
                case TileTerrainType.Obstacle:
                    return obstacleTerrainColor;
                case TileTerrainType.Goal:
                    return goalTerrainColor;
                default:
                    return normalTerrainColor;
            }
        }

        private void EnsureVisualLayers()
        {
            if (tileSideRenderer == null)
            {
                tileSideRenderer = CreateChildRenderer("TileSide", TileVisualFactory.GetTileSideSprite(), -1, Vector3.one, new Vector3(0f, -0.06f, 0.02f));
                tileSideRenderer.color = new Color(0.13f, 0.18f, 0.24f, 0.96f);
            }

            if (highlightRenderer == null)
            {
                highlightRenderer = CreateChildRenderer("HighlightOverlay", TileVisualFactory.GetHighlightSprite(), 3, Vector3.one, new Vector3(0f, 0f, -0.02f));
                highlightRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (terrainDetailRenderer == null)
            {
                terrainDetailRenderer = CreateChildRenderer("TerrainDetail", TileVisualFactory.GetObstacleDetailSprite(), 1, Vector3.one * 0.9f, new Vector3(0f, 0.06f, -0.03f));
                terrainDetailRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (goalRuneRenderer == null)
            {
                goalRuneRenderer = CreateChildRenderer("GoalRune", TileVisualFactory.GetGoalRuneSprite(), 2, Vector3.one * 0.82f, new Vector3(0f, 0.02f, -0.03f));
                goalRuneRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (goalHaloRenderer == null)
            {
                goalHaloRenderer = CreateChildRenderer("GoalHalo", TileVisualFactory.GetGoalHaloSprite(), 1, Vector3.one, new Vector3(0f, 0.02f, -0.02f));
                goalHaloRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        private SpriteRenderer CreateChildRenderer(string objectName, Sprite sprite, int sortingOrder, Vector3 localScale, Vector3 localPosition)
        {
            var child = new GameObject(objectName);
            child.transform.SetParent(transform, false);
            child.transform.localPosition = localPosition;
            child.transform.localScale = localScale;

            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void RefreshTerrainDetail()
        {
            tileSideRenderer.color = GetTerrainSideColor(terrainType);
            tileSideRenderer.transform.localPosition = terrainType == TileTerrainType.Obstacle
                ? new Vector3(0f, -0.13f, 0.02f)
                : new Vector3(0f, -0.06f, 0.02f);
            tileSideRenderer.transform.localScale = terrainType == TileTerrainType.Obstacle
                ? new Vector3(1.04f, 1.18f, 1f)
                : Vector3.one;
            terrainDetailRenderer.transform.localPosition = terrainType == TileTerrainType.Obstacle
                ? new Vector3(0f, 0.11f, -0.03f)
                : new Vector3(0f, 0.06f, -0.03f);
            terrainDetailRenderer.transform.localScale = terrainType == TileTerrainType.Obstacle
                ? new Vector3(0.86f, 0.92f, 1f)
                : Vector3.one * 0.9f;
            terrainDetailRenderer.color = new Color(0f, 0f, 0f, 0f);
            goalRuneRenderer.color = new Color(0f, 0f, 0f, 0f);
            goalHaloRenderer.color = new Color(0f, 0f, 0f, 0f);

            if (terrainType == TileTerrainType.Obstacle)
            {
                terrainDetailRenderer.sprite = TileVisualFactory.GetObstacleDetailSprite(GetObstacleDetailVariant());
                terrainDetailRenderer.color = new Color(1f, 1f, 1f, 0.98f);
            }
            else if (terrainType == TileTerrainType.Normal)
            {
                terrainDetailRenderer.sprite = TileVisualFactory.GetGroundDetailSprite();
                terrainDetailRenderer.color = new Color(0.48f, 0.56f, 0.62f, 0.08f);
            }

            if (terrainType == TileTerrainType.Goal)
            {
                goalRuneRenderer.color = new Color(0.78f, 1f, 0.94f, 0.82f);
                goalHaloRenderer.color = new Color(0.08f, 0.95f, 0.76f, 0.22f);
            }
        }

        private static Color GetTerrainSideColor(TileTerrainType value)
        {
            switch (value)
            {
                case TileTerrainType.Obstacle:
                    return new Color(0.075f, 0.058f, 0.04f, 1f);
                case TileTerrainType.Goal:
                    return new Color(0.015f, 0.18f, 0.16f, 0.98f);
                default:
                    return new Color(0.038f, 0.052f, 0.064f, 1f);
            }
        }

        private int GetObstacleDetailVariant()
        {
            return Math.Abs(Coordinates.x * 17 + Coordinates.y * 31) % TileVisualFactory.ObstacleDetailVariantCount;
        }

        private void SetHighlight(Color color, Sprite sprite = null)
        {
            if (highlightRenderer != null)
            {
                highlightRenderer.sprite = sprite != null ? sprite : TileVisualFactory.GetHighlightSprite();
                highlightRenderer.color = color;
            }
        }

    }
}
