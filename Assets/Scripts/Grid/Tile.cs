using System;
using SRPG.Units;
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
    [RequireComponent(typeof(BoxCollider2D))]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color baseColor = new Color(0.42f, 0.5f, 0.58f, 1f);
        [SerializeField] private Color normalTerrainColor = new Color(0.42f, 0.5f, 0.58f, 1f);
        [SerializeField] private Color obstacleTerrainColor = new Color(0.28f, 0.27f, 0.27f, 1f);
        [SerializeField] private Color goalTerrainColor = new Color(0.04f, 0.74f, 0.6f, 1f);
        [SerializeField] private Color moveHighlightedColor = new Color(0.12f, 0.52f, 1f, 0.46f);
        [SerializeField] private Color attackHighlightedColor = new Color(1f, 0.18f, 0.12f, 0.42f);
        [SerializeField] private Color enemyMoveThreatHighlightedColor = new Color(0.36f, 0.13f, 0.72f, 0.48f);
        [SerializeField] private Color enemyAttackThreatHighlightedColor = new Color(0.95f, 0.28f, 0.9f, 0.4f);
        [SerializeField] private Color guardianReactionHighlightedColor = new Color(1f, 0.72f, 0.18f, 0.36f);

        private static Sprite stoneTileSprite;
        private static Sprite highlightSprite;
        private static Sprite obstacleDetailSprite;
        private static Sprite goalRuneSprite;
        private static Sprite goalHaloSprite;
        private SpriteRenderer spriteRenderer;
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
            spriteRenderer.sprite = GetStoneTileSprite();
            spriteRenderer.color = baseColor;
            spriteRenderer.sortingOrder = 0;
            EnsureVisualLayers();

            var boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = Vector2.one;
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

            if (isMoveHighlighted)
            {
                SetHighlight(moveHighlightedColor);
                return;
            }

            if (isEnemyAttackThreatHighlighted)
            {
                SetHighlight(enemyAttackThreatHighlightedColor);
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
            if (highlightRenderer == null)
            {
                highlightRenderer = CreateChildRenderer("HighlightOverlay", GetHighlightSprite(), 3, Vector3.one);
                highlightRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (terrainDetailRenderer == null)
            {
                terrainDetailRenderer = CreateChildRenderer("TerrainDetail", GetObstacleDetailSprite(), 1, Vector3.one * 0.82f);
                terrainDetailRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (goalRuneRenderer == null)
            {
                goalRuneRenderer = CreateChildRenderer("GoalRune", GetGoalRuneSprite(), 2, Vector3.one * 0.76f);
                goalRuneRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (goalHaloRenderer == null)
            {
                goalHaloRenderer = CreateChildRenderer("GoalHalo", GetGoalHaloSprite(), 1, Vector3.one * 1.04f);
                goalHaloRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        private SpriteRenderer CreateChildRenderer(string objectName, Sprite sprite, int sortingOrder, Vector3 localScale)
        {
            var child = new GameObject(objectName);
            child.transform.SetParent(transform, false);
            child.transform.localPosition = new Vector3(0f, 0f, -0.02f);
            child.transform.localScale = localScale;

            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void RefreshTerrainDetail()
        {
            terrainDetailRenderer.color = new Color(0f, 0f, 0f, 0f);
            goalRuneRenderer.color = new Color(0f, 0f, 0f, 0f);
            goalHaloRenderer.color = new Color(0f, 0f, 0f, 0f);

            if (terrainType == TileTerrainType.Obstacle)
            {
                terrainDetailRenderer.sprite = GetObstacleDetailSprite();
                terrainDetailRenderer.color = new Color(0.74f, 0.69f, 0.56f, 0.94f);
            }

            if (terrainType == TileTerrainType.Goal)
            {
                goalRuneRenderer.color = new Color(0.78f, 1f, 0.94f, 0.82f);
                goalHaloRenderer.color = new Color(0.08f, 0.95f, 0.76f, 0.22f);
            }
        }

        private void SetHighlight(Color color)
        {
            if (highlightRenderer != null)
            {
                highlightRenderer.color = color;
            }
        }

        private static Sprite GetStoneTileSprite()
        {
            if (stoneTileSprite != null)
            {
                return stoneTileSprite;
            }

            const int size = 16;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var shade = 0.82f + y * 0.01f;
                    if (x == 0 || y == 0 || x == size - 1 || y == size - 1)
                    {
                        shade = 0.5f;
                    }
                    else if (x == 1 || y == 1)
                    {
                        shade = 0.62f;
                    }
                    else if (x == 2 || y == size - 3 || y >= size - 4 && x > 2 && x < size - 3)
                    {
                        shade = 0.98f;
                    }
                    else if ((x + y * 3) % 7 == 0)
                    {
                        shade = 0.78f;
                    }
                    else if (x > 4 && x < 11 && y > 4 && y < 11)
                    {
                        shade += 0.06f;
                    }

                    texture.SetPixel(x, y, new Color(shade, shade, shade, 1f));
                }
            }

            texture.Apply();
            stoneTileSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return stoneTileSprite;
        }

        private static Sprite GetHighlightSprite()
        {
            if (highlightSprite != null)
            {
                return highlightSprite;
            }

            const int size = 16;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            var transparent = new Color(1f, 1f, 1f, 0.22f);
            var edge = new Color(1f, 1f, 1f, 0.78f);

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, x <= 1 || y <= 1 || x >= size - 2 || y >= size - 2 ? edge : transparent);
                }
            }

            texture.Apply();
            highlightSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return highlightSprite;
        }

        private static Sprite GetObstacleDetailSprite()
        {
            if (obstacleDetailSprite != null)
            {
                return obstacleDetailSprite;
            }

            const int size = 16;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            var transparent = new Color(0f, 0f, 0f, 0f);
            var rubble = new Color(1f, 1f, 1f, 0.8f);

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            FillRect(texture, 3, 2, 6, 6, rubble);
            FillRect(texture, 8, 3, 12, 8, rubble);
            FillRect(texture, 5, 8, 10, 13, rubble);
            FillRect(texture, 2, 10, 4, 12, new Color(0.75f, 0.72f, 0.64f, 0.72f));
            SetPixel(texture, 12, 11, rubble);
            SetPixel(texture, 13, 12, rubble);
            SetPixel(texture, 4, 6, new Color(1f, 0.95f, 0.78f, 0.92f));
            SetPixel(texture, 10, 4, new Color(1f, 0.95f, 0.78f, 0.92f));
            SetPixel(texture, 7, 11, new Color(1f, 0.95f, 0.78f, 0.92f));
            texture.Apply();

            obstacleDetailSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return obstacleDetailSprite;
        }

        private static Sprite GetGoalRuneSprite()
        {
            if (goalRuneSprite != null)
            {
                return goalRuneSprite;
            }

            const int size = 16;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            var transparent = new Color(0f, 0f, 0f, 0f);
            var rune = new Color(1f, 1f, 1f, 0.95f);

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            for (var i = 0; i < 6; i++)
            {
                SetPixel(texture, 8, 3 + i, rune);
                SetPixel(texture, 8, 13 - i, rune);
                SetPixel(texture, 3 + i, 8, rune);
                SetPixel(texture, 13 - i, 8, rune);
            }

            SetPixel(texture, 7, 7, rune);
            SetPixel(texture, 8, 7, rune);
            SetPixel(texture, 7, 8, rune);
            SetPixel(texture, 8, 8, rune);
            texture.Apply();

            goalRuneSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return goalRuneSprite;
        }

        private static Sprite GetGoalHaloSprite()
        {
            if (goalHaloSprite != null)
            {
                return goalHaloSprite;
            }

            const int size = 18;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            var transparent = new Color(0f, 0f, 0f, 0f);
            var halo = new Color(1f, 1f, 1f, 0.72f);
            var center = (size - 1) * 0.5f;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                    texture.SetPixel(x, y, distance < 7.5f ? halo : transparent);
                }
            }

            texture.Apply();
            goalHaloSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return goalHaloSprite;
        }

        private static void FillRect(Texture2D texture, int minX, int minY, int maxX, int maxY, Color color)
        {
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    SetPixel(texture, x, y, color);
                }
            }
        }

        private static void SetPixel(Texture2D texture, int x, int y, Color color)
        {
            if (x < 0 || x >= 16 || y < 0 || y >= 16)
            {
                return;
            }

            texture.SetPixel(x, y, color);
        }
    }
}
