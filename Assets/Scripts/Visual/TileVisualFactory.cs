using System;
using UnityEngine;

namespace SRPG.Visual
{
    public static class TileVisualFactory
    {
        private const int TextureWidth = 32;
        private const int TextureHeight = 16;
        private const float PixelsPerUnit = TextureWidth;

        private static Sprite stoneTileSprite;
        private static Sprite tileSideSprite;
        private static Sprite highlightSprite;
        private static Sprite moveHighlightSprite;
        private static Sprite enemyAttackThreatSprite;
        private static Sprite moveThreatOverlapSprite;
        private static Sprite obstacleDetailSprite;
        private static Sprite goalRuneSprite;
        private static Sprite goalHaloSprite;

        public static Sprite GetStoneTileSprite()
        {
            if (stoneTileSprite != null)
            {
                return stoneTileSprite;
            }

            var texture = CreateTexture();
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance > 1f)
                    {
                        continue;
                    }

                    var shade = 0.76f + y * 0.012f;
                    if (distance > 0.82f)
                    {
                        shade = y >= TextureHeight * 0.5f ? 0.5f : 0.3f;
                    }
                    else if (distance > 0.7f)
                    {
                        shade = y >= TextureHeight * 0.5f ? 0.74f : 0.52f;
                    }
                    else if ((x * 3 + y * 5) % 13 == 0)
                    {
                        shade -= 0.07f;
                    }
                    else if ((x + y * 2) % 17 == 0)
                    {
                        shade += 0.06f;
                    }

                    texture.SetPixel(x, y, new Color(shade, shade, shade, 1f));
                }
            }

            texture.Apply();
            stoneTileSprite = CreateSprite(texture);
            return stoneTileSprite;
        }

        public static Sprite GetTileSideSprite()
        {
            if (tileSideSprite != null)
            {
                return tileSideSprite;
            }

            var texture = CreateTexture();
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    if (GetDiamondDistance(x, y) <= 1f)
                    {
                        var shade = y < TextureHeight * 0.5f ? 0.58f : 0.78f;
                        texture.SetPixel(x, y, new Color(shade, shade, shade, 0.96f));
                    }
                }
            }

            texture.Apply();
            tileSideSprite = CreateSprite(texture);
            return tileSideSprite;
        }

        public static Sprite GetHighlightSprite()
        {
            if (highlightSprite != null)
            {
                return highlightSprite;
            }

            var texture = CreateTexture();
            var fill = new Color(1f, 1f, 1f, 0.26f);
            var edge = new Color(1f, 1f, 1f, 0.9f);
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance <= 1f)
                    {
                        texture.SetPixel(x, y, distance > 0.76f ? edge : fill);
                    }
                }
            }

            texture.Apply();
            highlightSprite = CreateSprite(texture);
            return highlightSprite;
        }

        public static Sprite GetMoveHighlightSprite()
        {
            if (moveHighlightSprite != null)
            {
                return moveHighlightSprite;
            }

            var texture = CreateTexture();
            var fill = new Color(1f, 1f, 1f, 0.12f);
            var innerEdge = new Color(1f, 1f, 1f, 0.48f);
            var outerEdge = new Color(1f, 1f, 1f, 1f);
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance <= 1f)
                    {
                        var color = distance > 0.82f
                            ? outerEdge
                            : distance > 0.68f ? innerEdge : fill;
                        texture.SetPixel(x, y, color);
                    }
                }
            }

            texture.Apply();
            moveHighlightSprite = CreateSprite(texture);
            return moveHighlightSprite;
        }

        public static Sprite GetEnemyAttackThreatSprite()
        {
            if (enemyAttackThreatSprite != null)
            {
                return enemyAttackThreatSprite;
            }

            var texture = CreateTexture();
            var fill = new Color(1f, 1f, 1f, 0.12f);
            var innerEdge = new Color(1f, 1f, 1f, 0.46f);
            var outerEdge = new Color(1f, 1f, 1f, 1f);
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance <= 1f)
                    {
                        var color = distance > 0.82f
                            ? outerEdge
                            : distance > 0.68f ? innerEdge : fill;
                        texture.SetPixel(x, y, color);
                    }
                }
            }

            texture.Apply();
            enemyAttackThreatSprite = CreateSprite(texture);
            return enemyAttackThreatSprite;
        }

        public static Sprite GetMoveThreatOverlapSprite()
        {
            if (moveThreatOverlapSprite != null)
            {
                return moveThreatOverlapSprite;
            }

            var texture = CreateTexture();
            var moveFill = new Color(0.16f, 0.62f, 1f, 0.14f);
            var moveEdge = new Color(0.42f, 0.78f, 1f, 0.44f);
            var dangerEdge = new Color(1f, 0.22f, 0.1f, 0.94f);
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance <= 1f)
                    {
                        var color = distance > 0.82f
                            ? dangerEdge
                            : distance > 0.68f ? moveEdge : moveFill;
                        texture.SetPixel(x, y, color);
                    }
                }
            }

            texture.Apply();
            moveThreatOverlapSprite = CreateSprite(texture);
            return moveThreatOverlapSprite;
        }

        public static Sprite GetObstacleDetailSprite()
        {
            if (obstacleDetailSprite != null)
            {
                return obstacleDetailSprite;
            }

            var texture = CreateTexture();
            var rubble = new Color(1f, 1f, 1f, 0.88f);
            var shadow = new Color(0.28f, 0.24f, 0.18f, 0.96f);
            var light = new Color(1f, 0.94f, 0.72f, 0.96f);

            FillRect(texture, 8, 3, 23, 5, shadow);
            FillRect(texture, 8, 5, 14, 9, rubble);
            FillRect(texture, 15, 4, 22, 10, rubble);
            FillRect(texture, 12, 9, 19, 12, rubble);
            FillRect(texture, 10, 4, 13, 5, shadow);
            FillRect(texture, 17, 3, 21, 4, shadow);
            SetPixel(texture, 10, 9, light);
            SetPixel(texture, 17, 10, light);
            SetPixel(texture, 21, 7, light);

            texture.Apply();
            obstacleDetailSprite = CreateSprite(texture);
            return obstacleDetailSprite;
        }

        public static Sprite GetGoalRuneSprite()
        {
            if (goalRuneSprite != null)
            {
                return goalRuneSprite;
            }

            var texture = CreateTexture();
            var rune = new Color(1f, 1f, 1f, 0.96f);
            for (var i = 0; i < 6; i++)
            {
                SetPixel(texture, 16, 4 + i, rune);
                SetPixel(texture, 16, 11 - i, rune);
                SetPixel(texture, 10 + i, 8, rune);
                SetPixel(texture, 22 - i, 8, rune);
            }

            SetPixel(texture, 15, 7, rune);
            SetPixel(texture, 16, 7, rune);
            SetPixel(texture, 15, 8, rune);
            SetPixel(texture, 16, 8, rune);
            texture.Apply();

            goalRuneSprite = CreateSprite(texture);
            return goalRuneSprite;
        }

        public static Sprite GetGoalHaloSprite()
        {
            if (goalHaloSprite != null)
            {
                return goalHaloSprite;
            }

            var texture = CreateTexture();
            var halo = new Color(1f, 1f, 1f, 0.66f);
            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance <= 0.72f && distance >= 0.42f)
                    {
                        texture.SetPixel(x, y, halo);
                    }
                }
            }

            texture.Apply();
            goalHaloSprite = CreateSprite(texture);
            return goalHaloSprite;
        }

        private static Texture2D CreateTexture()
        {
            var texture = new Texture2D(TextureWidth, TextureHeight);
            texture.filterMode = FilterMode.Point;
            var transparent = new Color(0f, 0f, 0f, 0f);

            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            return texture;
        }

        private static Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, TextureWidth, TextureHeight),
                new Vector2(0.5f, 0.5f),
                PixelsPerUnit);
        }

        private static float GetDiamondDistance(int x, int y)
        {
            var centerX = (TextureWidth - 1) * 0.5f;
            var centerY = (TextureHeight - 1) * 0.5f;
            var normalizedX = (float)Math.Abs(x - centerX) / (TextureWidth * 0.5f);
            var normalizedY = (float)Math.Abs(y - centerY) / (TextureHeight * 0.5f);
            return normalizedX + normalizedY;
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
            if (x < 0 || x >= TextureWidth || y < 0 || y >= TextureHeight)
            {
                return;
            }

            texture.SetPixel(x, y, color);
        }
    }
}
