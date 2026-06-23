using SRPG.Units;
using UnityEngine;

namespace SRPG.Visual
{
    public static class UnitTokenVisual
    {
        private const int TokenWidth = 20;
        private const int TokenHeight = 12;

        public const float UnitVisualScale = 0.94f;
        public const int HpFontSize = 24;
        public const float HpCharacterSize = 0.064f;
        public const int HpBackgroundSortingOffset = 5;
        public const int HpTextSortingOffset = 6;

        public static readonly Vector3 UnitShadowScale = new Vector3(0.9f, 0.25f, 1f);
        public static readonly Vector3 TokenBaseScale = new Vector3(0.94f, 0.44f, 1f);
        public static readonly Vector3 SelectionRingScale = new Vector3(1f, 0.5f, 1f);
        public static readonly Vector3 UnitTopLightScale = new Vector3(0.72f, 0.18f, 1f);
        public static readonly Vector3 HpBackgroundScale = new Vector3(0.54f, 0.17f, 1f);
        public static readonly Color HpBackgroundColor = new Color(0f, 0f, 0f, 0.54f);
        public static readonly Vector3 HpShadowOffset = new Vector3(0.014f, -0.014f, 0.01f);
        public static readonly Color HpShadowColor = new Color(0f, 0f, 0f, 0.82f);

        private static Sprite tokenBaseSprite;
        private static Sprite tokenRingSprite;

        public static Sprite GetTokenBaseSprite()
        {
            return tokenBaseSprite ?? (tokenBaseSprite = CreateEllipseSprite(false));
        }

        public static Sprite GetTokenRingSprite()
        {
            return tokenRingSprite ?? (tokenRingSprite = CreateEllipseSprite(true));
        }

        public static Color GetTokenBaseColor(bool acted)
        {
            return acted
                ? new Color(0.025f, 0.035f, 0.045f, 0.28f)
                : new Color(0.035f, 0.045f, 0.055f, 0.46f);
        }

        public static Color GetTokenRingColor(Faction faction, bool selected, bool acted)
        {
            if (selected)
            {
                return new Color(0.42f, 0.92f, 1f, 0.96f);
            }

            if (acted)
            {
                return faction == Faction.Player
                    ? new Color(0.12f, 0.34f, 0.58f, 0.44f)
                    : new Color(0.5f, 0.1f, 0.14f, 0.44f);
            }

            return faction == Faction.Player
                ? new Color(0.18f, 0.62f, 1f, 0.68f)
                : new Color(0.95f, 0.22f, 0.24f, 0.68f);
        }

        public static Vector3 GetHpTextOffset(Faction faction)
        {
            var horizontalOffset = faction == Faction.Player ? -0.06f : 0.06f;
            return new Vector3(horizontalOffset, -0.72f, -0.1f);
        }

        private static Sprite CreateEllipseSprite(bool ringOnly)
        {
            var texture = new Texture2D(TokenWidth, TokenHeight);
            texture.filterMode = FilterMode.Point;
            var centerX = (TokenWidth - 1) * 0.5f;
            var centerY = (TokenHeight - 1) * 0.5f;

            for (var y = 0; y < TokenHeight; y++)
            {
                for (var x = 0; x < TokenWidth; x++)
                {
                    var nx = (x - centerX) / (centerX * 0.94f);
                    var ny = (y - centerY) / (centerY * 0.9f);
                    var distance = nx * nx + ny * ny;
                    var visible = distance <= 1f && (!ringOnly || distance >= 0.72f);
                    texture.SetPixel(x, y, visible ? Color.white : new Color(0f, 0f, 0f, 0f));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, TokenWidth, TokenHeight), new Vector2(0.5f, 0.5f), TokenWidth);
        }
    }
}
