using System;
using System.Collections;
using System.Collections.Generic;
using SRPG.Audio;
using SRPG.Grid;
using SRPG.UI;
using UnityEngine;

namespace SRPG.Units
{
    public enum Faction
    {
        Player,
        Enemy
    }

    public enum UnitType
    {
        Soldier,
        Knight,
        Archer,
        Rogue
    }

    public enum EnemyAIType
    {
        Aggressive,
        WeakTarget,
        Stationary,
        Guardian
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private Vector2Int initialGridPosition;
        [SerializeField] private int movePower = 3;
        [SerializeField] private Faction faction = Faction.Player;
        [SerializeField] private UnitType unitType = UnitType.Soldier;
        [SerializeField] private EnemyAIType enemyAIType = EnemyAIType.Aggressive;
        [SerializeField] private Color baseColor = Color.blue;
        [SerializeField] private Color selectedColor = new Color(1f, 0.88f, 0.28f, 1f);
        [SerializeField] private Color actedPlayerColor = new Color(0.07f, 0.16f, 0.28f, 1f);
        [SerializeField] private bool hasActed;
        [SerializeField] private int maxHp = 10;
        [SerializeField] private int currentHp = 10;
        [SerializeField] private int attackPower = 5;
        [SerializeField] private int attackRange = 1;
        [SerializeField] private Vector3 hpTextOffset = new Vector3(0f, -0.5f, -0.1f);

        private static readonly Dictionary<UnitType, Sprite> PixelSprites = new Dictionary<UnitType, Sprite>();
        private static readonly Dictionary<string, Sprite> ResourceUnitSprites = new Dictionary<string, Sprite>();
        private static Sprite shadowSprite;
        private static Sprite selectionRingSprite;
        private static Sprite hpBackSprite;
        private static Sprite unitTopLightSprite;
        private GridManager gridManager;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer shadowRenderer;
        private SpriteRenderer selectionRingRenderer;
        private SpriteRenderer topLightRenderer;
        private SpriteRenderer hpBackRenderer;
        private TextMesh hpText;
        private TextMesh hpShadowText;
        private bool isSelected;
        private bool isDead;
        private bool usesResourceUnitSprite;
        private Coroutine scaleEffectCoroutine;
        private Coroutine colorEffectCoroutine;

        public event Action<Unit> Clicked;
        public event Action<Unit> HoverEntered;
        public event Action<Unit> HoverExited;

        public Vector2Int GridPosition => gridPosition;
        public Vector2Int InitialGridPosition => initialGridPosition;
        public int MovePower => movePower;
        public Faction Faction => faction;
        public UnitType UnitType => unitType;
        public EnemyAIType EnemyAIType => enemyAIType;
        public bool IsPlayerControlled => faction == Faction.Player;
        public bool HasActed => hasActed;
        public int MaxHp => maxHp;
        public int CurrentHp => currentHp;
        public int AttackPower => attackPower;
        public int AttackRange => attackRange;
        public bool IsDead => isDead;

        public void Initialize(
            GridManager manager,
            Vector2Int startPosition,
            int movement,
            Faction ownerFaction,
            Color color,
            int startingMaxHp = 10,
            int startingAttackPower = 5,
            int startingAttackRange = 1,
            UnitType newUnitType = UnitType.Soldier,
            EnemyAIType newEnemyAIType = EnemyAIType.Aggressive)
        {
            gridManager = manager;
            movePower = movement;
            faction = ownerFaction;
            unitType = newUnitType;
            enemyAIType = newEnemyAIType;
            initialGridPosition = startPosition;
            baseColor = color;
            hasActed = false;
            isDead = false;
            maxHp = Mathf.Max(1, startingMaxHp);
            currentHp = maxHp;
            attackPower = Mathf.Max(0, startingAttackPower);
            attackRange = Mathf.Max(1, startingAttackRange);

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetUnitSprite(unitType, faction, out usesResourceUnitSprite);
            spriteRenderer.color = GetNormalVisualColor();
            spriteRenderer.sortingOrder = 8;

            transform.localScale = Vector3.one * gridManager.CellSize * 1.06f;
            EnsureUnitVisualLayers();

            var circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.radius = 0.46f;

            gridManager.TryPlaceUnit(this, startPosition);
            CreateHpText();
            UpdateHpDisplay();
        }

        public void InitializeForStage(
            GridManager manager,
            string unitName,
            Faction ownerFaction,
            Vector2Int startPosition,
            int startingMaxHp,
            int startingAttackPower,
            int movement,
            int startingAttackRange,
            UnitType newUnitType,
            EnemyAIType newEnemyAIType,
            Color color)
        {
            name = unitName;
            Initialize(
                manager,
                startPosition,
                movement,
                ownerFaction,
                color,
                startingMaxHp,
                startingAttackPower,
                startingAttackRange,
                newUnitType,
                newEnemyAIType);
        }

        public bool MoveTo(Vector2Int destination)
        {
            return !isDead && gridManager != null && gridManager.TryPlaceUnit(this, destination);
        }

        public bool CanAttack(Unit target)
        {
            if (isDead || target == null || target.IsDead || target.Faction == faction || gridManager == null)
            {
                return false;
            }

            return gridManager.GetManhattanDistance(gridPosition, target.GridPosition) <= attackRange;
        }

        public bool Attack(Unit target)
        {
            if (!CanAttack(target))
            {
                return false;
            }

            PlayAttackEffect();
            AudioManager.Instance?.PlayAttackSe();
            BattleUI.Instance?.AddBattleLog($"{name} attacked {target.name} for {attackPower} damage");
            target.TakeDamage(attackPower);
            return true;
        }

        public void PlayWaitEffect()
        {
            if (scaleEffectCoroutine != null)
            {
                StopCoroutine(scaleEffectCoroutine);
            }

            scaleEffectCoroutine = StartCoroutine(ScalePulseRoutine(1.08f, 0.06f));
        }

        public void TakeDamage(int damage)
        {
            if (isDead)
            {
                return;
            }

            int appliedDamage = Mathf.Max(0, damage);
            bool willDie = currentHp - appliedDamage <= 0;
            DamagePopup.Show(transform.position, appliedDamage, willDie);
            PlayHitEffect(willDie);
            AudioManager.Instance?.PlayHitSe();

            currentHp -= appliedDamage;
            UpdateHpDisplay();
            Debug.Log($"{name} took {damage} damage. HP: {Mathf.Max(0, currentHp)}/{maxHp}");

            if (currentHp <= 0)
            {
                Die();
            }
        }

        public void SetSelected(bool value)
        {
            if (isDead)
            {
                isSelected = false;
                return;
            }

            isSelected = value;
            RefreshColor();
        }

        public void SetHasActed(bool value)
        {
            hasActed = value;
            RefreshColor();
        }

        internal void SetGridPositionInternal(Vector2Int coordinates)
        {
            gridPosition = coordinates;
        }

        public void UpdateHpDisplay()
        {
            if (hpText == null || isDead)
            {
                return;
            }

            hpText.text = $"{Mathf.Max(0, currentHp)}/{maxHp}";
            if (hpShadowText != null)
            {
                hpShadowText.text = hpText.text;
            }
        }

        public void HideHpDisplay()
        {
            if (hpText != null)
            {
                hpText.gameObject.SetActive(false);
            }

            if (hpShadowText != null)
            {
                hpShadowText.gameObject.SetActive(false);
            }

            if (hpBackRenderer != null)
            {
                hpBackRenderer.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseDown()
        {
            if (isDead)
            {
                return;
            }

            Clicked?.Invoke(this);
        }

        private void OnMouseEnter()
        {
            if (isDead)
            {
                return;
            }

            HoverEntered?.Invoke(this);
        }

        private void OnMouseExit()
        {
            HoverExited?.Invoke(this);
        }

        private void Die()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            isSelected = false;
            hasActed = true;

            if (gridManager != null)
            {
                gridManager.RemoveUnit(this);
            }

            Debug.Log($"{name} defeated.");
            BattleUI.Instance?.AddBattleLog($"{name} defeated");
            AudioManager.Instance?.PlayKoSe();
            DamagePopup.ShowText(transform.position + new Vector3(0f, 0.28f, -0.25f), "KO", new Color(1f, 0.86f, 0.22f, 1f), 0.24f, 48);
            HideHpDisplay();
            gameObject.SetActive(false);
        }

        private void CreateHpText()
        {
            if (hpText != null)
            {
                return;
            }

            var hpTextObject = new GameObject("HPText");
            hpTextObject.transform.SetParent(transform, false);
            hpTextObject.transform.localPosition = hpTextOffset;

            var hpBackObject = new GameObject("HPTextBack");
            hpBackObject.transform.SetParent(transform, false);
            hpBackObject.transform.localPosition = hpTextOffset + new Vector3(0f, 0f, 0.02f);
            hpBackObject.transform.localScale = new Vector3(0.62f, 0.2f, 1f);
            hpBackRenderer = hpBackObject.AddComponent<SpriteRenderer>();
            hpBackRenderer.sprite = GetHpBackSprite();
            hpBackRenderer.color = new Color(0f, 0f, 0f, 0.72f);
            hpBackRenderer.sortingOrder = 6;

            var hpTextRenderer = hpTextObject.AddComponent<MeshRenderer>();
            hpText = hpTextObject.AddComponent<TextMesh>();
            hpText.anchor = TextAnchor.MiddleCenter;
            hpText.alignment = TextAlignment.Center;
            hpText.fontSize = 28;
            hpText.characterSize = 0.072f;
            hpText.color = Color.white;
            hpTextRenderer.sortingOrder = 7;

            var shadowObject = new GameObject("HPTextShadow");
            shadowObject.transform.SetParent(transform, false);
            shadowObject.transform.localPosition = hpTextOffset + new Vector3(0.018f, -0.018f, 0.01f);

            var shadowRenderer = shadowObject.AddComponent<MeshRenderer>();
            hpShadowText = shadowObject.AddComponent<TextMesh>();
            hpShadowText.anchor = TextAnchor.MiddleCenter;
            hpShadowText.alignment = TextAlignment.Center;
            hpShadowText.fontSize = 28;
            hpShadowText.characterSize = 0.072f;
            hpShadowText.color = new Color(0f, 0f, 0f, 0.9f);
            shadowRenderer.sortingOrder = 7;
        }

        private void EnsureUnitVisualLayers()
        {
            if (shadowRenderer == null)
            {
                shadowRenderer = CreateChildRenderer("UnitShadow", GetShadowSprite(), 6, new Vector3(1.08f, 0.36f, 1f), new Vector3(0f, -0.24f, 0.04f));
                shadowRenderer.color = new Color(0f, 0f, 0f, 0.38f);
            }

            if (selectionRingRenderer == null)
            {
                selectionRingRenderer = CreateChildRenderer("SelectionRing", GetSelectionRingSprite(), 7, new Vector3(1.2f, 0.44f, 1f), new Vector3(0f, -0.25f, 0.03f));
                selectionRingRenderer.color = new Color(0f, 0f, 0f, 0f);
            }

            if (topLightRenderer == null)
            {
                topLightRenderer = CreateChildRenderer("UnitTopLight", GetUnitTopLightSprite(), 9, new Vector3(0.78f, 0.2f, 1f), new Vector3(0f, 0.24f, -0.02f));
                topLightRenderer.color = new Color(1f, 0.92f, 0.62f, 0.18f);
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

        private void RefreshColor()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (isSelected)
            {
                spriteRenderer.color = GetSelectedTint();
                if (selectionRingRenderer != null)
                {
                    selectionRingRenderer.color = IsPlayerControlled
                        ? new Color(0.46f, 0.86f, 1f, 0.9f)
                        : new Color(1f, 0.3f, 0.8f, 0.9f);
                }
                if (topLightRenderer != null)
                {
                    topLightRenderer.color = new Color(1f, 0.95f, 0.68f, 0.32f);
                }
                return;
            }

            if (selectionRingRenderer != null)
            {
                selectionRingRenderer.color = new Color(0f, 0f, 0f, 0f);
            }
            if (topLightRenderer != null)
            {
                topLightRenderer.color = new Color(1f, 0.92f, 0.62f, 0.16f);
            }

            spriteRenderer.color = IsPlayerControlled && hasActed ? GetActedVisualColor() : GetNormalVisualColor();
        }

        private Color GetSelectedTint()
        {
            if (usesResourceUnitSprite)
            {
                return Color.white;
            }

            return IsPlayerControlled
                ? new Color(0.54f, 0.84f, 1f, 1f)
                : new Color(1f, 0.38f, 0.58f, 1f);
        }

        private Color GetNormalVisualColor()
        {
            return usesResourceUnitSprite ? Color.white : baseColor;
        }

        private Color GetActedVisualColor()
        {
            return usesResourceUnitSprite ? new Color(0.62f, 0.66f, 0.72f, 1f) : actedPlayerColor;
        }

        private void PlayAttackEffect()
        {
            if (scaleEffectCoroutine != null)
            {
                StopCoroutine(scaleEffectCoroutine);
            }

            scaleEffectCoroutine = StartCoroutine(ScalePulseRoutine(1.22f, 0.09f));
        }

        private void PlayHitEffect(bool defeated)
        {
            if (colorEffectCoroutine != null)
            {
                StopCoroutine(colorEffectCoroutine);
            }

            colorEffectCoroutine = StartCoroutine(ColorFlashRoutine(defeated ? new Color(1f, 0.28f, 0.08f, 1f) : Color.white, 0.11f));
        }

        private IEnumerator ScalePulseRoutine(float scaleMultiplier, float duration)
        {
            var originalScale = transform.localScale;
            transform.localScale = originalScale * scaleMultiplier;
            yield return new WaitForSeconds(duration);
            transform.localScale = originalScale;
            scaleEffectCoroutine = null;
        }

        private IEnumerator ColorFlashRoutine(Color flashColor, float duration)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(duration);
            RefreshColor();
            colorEffectCoroutine = null;
        }

        private static Sprite GetUnitSprite(UnitType type, Faction ownerFaction, out bool loadedFromResource)
        {
            var resourceSprite = GetResourceUnitSprite(type, ownerFaction);
            if (resourceSprite != null)
            {
                loadedFromResource = true;
                return resourceSprite;
            }

            loadedFromResource = false;
            return GetPixelSprite(type);
        }

        private static Sprite GetResourceUnitSprite(UnitType type, Faction ownerFaction)
        {
            var factionPrefix = ownerFaction == Faction.Player ? "Player" : "Enemy";
            var key = $"{factionPrefix}_{type}";
            if (ResourceUnitSprites.TryGetValue(key, out var cachedSprite))
            {
                return cachedSprite;
            }

            var texture = Resources.Load<Texture2D>($"Units/{key}");
            if (texture == null)
            {
                return null;
            }

            texture.filterMode = FilterMode.Point;
            var pixelsPerUnit = texture.height > 0 ? texture.height * GetResourcePixelsPerUnitScale(type) : 1f;
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.22f), pixelsPerUnit);
            ResourceUnitSprites[key] = sprite;
            return sprite;
        }

        private static float GetResourcePixelsPerUnitScale(UnitType type)
        {
            switch (type)
            {
                case UnitType.Archer:
                    return 1.04f;
                case UnitType.Rogue:
                    return 1.02f;
                default:
                    return 1.08f;
            }
        }

        private static Sprite GetPixelSprite(UnitType type)
        {
            if (PixelSprites.TryGetValue(type, out var sprite))
            {
                return sprite;
            }

            sprite = CreatePixelSprite(type);
            PixelSprites[type] = sprite;
            return sprite;
        }

        private static Sprite CreatePixelSprite(UnitType type)
        {
            const int size = 16;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;

            var transparent = new Color(0f, 0f, 0f, 0f);
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            var palette = CreateUnitPixelPalette();
            switch (type)
            {
                case UnitType.Knight:
                    DrawPixelPattern(texture, palette, new[]
                    {
                        "................",
                        ".....OOOOO......",
                        "....OLLLLO......",
                        "...OOSSSOO......",
                        "...OMMMMMO......",
                        "..OOMLMLMOO.....",
                        ".OMMMMMMMMO.....",
                        ".OMMGMMGMMOB....",
                        ".OMMDDDDMMOB....",
                        "..OMDMMMDOB.....",
                        "...ODDDDOB......",
                        "..OO.DD.OO......",
                        "..O..DD..O......",
                        ".....OO.........",
                        "................",
                        "................"
                    });
                    break;
                case UnitType.Archer:
                    DrawPixelPattern(texture, palette, new[]
                    {
                        "................",
                        "......OOOO......",
                        ".....OLLLLO.....",
                        ".....OSSSO......",
                        "....OMMMMO..B...",
                        "....OMLMMO.B....",
                        "...OOMMDDOB.....",
                        "..BOOMMGOB......",
                        ".B..OMMMO.......",
                        ".B..ODDDO.......",
                        "..B..DD.O.......",
                        "...B.DD.O.......",
                        "....BO.O........",
                        ".....B..........",
                        "................",
                        "................"
                    });
                    break;
                case UnitType.Rogue:
                    DrawPixelPattern(texture, palette, new[]
                    {
                        "................",
                        "......OOOO......",
                        ".....OMLLMO.....",
                        "....OMSSSMO.....",
                        "....OMMMMO......",
                        "...OOMDMMOO.....",
                        "..BOOMMMOOB.....",
                        ".B..OMMMO..B....",
                        "....ODDDO.B.....",
                        "...O.DDD.O......",
                        "..O..DD..O......",
                        ".B...O...B......",
                        "B....O....B.....",
                        "................",
                        "................",
                        "................"
                    });
                    break;
                default:
                    DrawPixelPattern(texture, palette, new[]
                    {
                        "................",
                        "......OOO.......",
                        ".....OLLO.......",
                        ".....OSSO.......",
                        "....OMMMO..B....",
                        "...OMLMMMO.B....",
                        "..OOMMMMDOB.....",
                        "...OGMMMGOB.....",
                        "....OMMMO.......",
                        "....ODDDO.......",
                        "...O.DD.O.......",
                        "...O.DD.O.......",
                        "..OO....OO......",
                        "..O......O......",
                        "................",
                        "................"
                    });
                    break;
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.42f), size);
        }

        private static Dictionary<char, Color> CreateUnitPixelPalette()
        {
            return new Dictionary<char, Color>
            {
                { 'O', new Color(0.06f, 0.07f, 0.1f, 1f) },
                { 'L', new Color(1f, 0.98f, 0.88f, 1f) },
                { 'M', new Color(0.74f, 0.78f, 0.86f, 1f) },
                { 'D', new Color(0.34f, 0.38f, 0.45f, 1f) },
                { 'S', new Color(0.86f, 0.66f, 0.48f, 1f) },
                { 'G', new Color(0.98f, 0.76f, 0.32f, 1f) },
                { 'B', new Color(0.4f, 0.25f, 0.12f, 1f) }
            };
        }

        private static void DrawPixelPattern(Texture2D texture, Dictionary<char, Color> palette, string[] rows)
        {
            for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                var y = rows.Length - 1 - rowIndex;
                var row = rows[rowIndex];
                for (var x = 0; x < row.Length; x++)
                {
                    if (palette.TryGetValue(row[x], out var color))
                    {
                        SetPixel(texture, x, y, color);
                    }
                }
            }
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
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
            {
                return;
            }

            texture.SetPixel(x, y, color);
        }

        private static void ApplyOutline(Texture2D texture, int size)
        {
            for (var x = 1; x < size - 1; x++)
            {
                SetPixel(texture, x, 0, new Color(0.12f, 0.12f, 0.16f, 1f));
            }

            SetPixel(texture, 1, 1, new Color(0.12f, 0.12f, 0.16f, 1f));
            SetPixel(texture, size - 2, 1, new Color(0.12f, 0.12f, 0.16f, 1f));
            SetPixel(texture, 1, 5, new Color(0.12f, 0.12f, 0.16f, 1f));
            SetPixel(texture, size - 2, 5, new Color(0.12f, 0.12f, 0.16f, 1f));
        }

        private static Sprite GetShadowSprite()
        {
            if (shadowSprite != null)
            {
                return shadowSprite;
            }

            shadowSprite = CreateDiscSprite(16, new Color(1f, 1f, 1f, 0.9f), true);
            return shadowSprite;
        }

        private static Sprite GetSelectionRingSprite()
        {
            if (selectionRingSprite != null)
            {
                return selectionRingSprite;
            }

            selectionRingSprite = CreateDiscSprite(18, new Color(1f, 1f, 1f, 0.9f), false);
            return selectionRingSprite;
        }

        private static Sprite GetHpBackSprite()
        {
            if (hpBackSprite != null)
            {
                return hpBackSprite;
            }

            var texture = new Texture2D(1, 1);
            texture.filterMode = FilterMode.Point;
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            hpBackSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            return hpBackSprite;
        }

        private static Sprite CreateDiscSprite(int size, Color color, bool filled)
        {
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            var transparent = new Color(0f, 0f, 0f, 0f);
            var center = (size - 1) * 0.5f;
            var outer = center * 0.86f;
            var inner = center * 0.58f;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                    var visible = filled ? distance <= outer : distance <= outer && distance >= inner;
                    texture.SetPixel(x, y, visible ? color : transparent);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static Sprite GetUnitTopLightSprite()
        {
            if (unitTopLightSprite != null)
            {
                return unitTopLightSprite;
            }

            var texture = new Texture2D(8, 2);
            texture.filterMode = FilterMode.Point;
            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var alpha = x == 0 || x == 7 ? 0.25f : 0.9f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            unitTopLightSprite = Sprite.Create(texture, new Rect(0, 0, 8, 2), new Vector2(0.5f, 0.5f), 8f);
            return unitTopLightSprite;
        }
    }
}
