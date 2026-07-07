using System;
using UnityEngine;

namespace SRPG.Visual
{
    public static class TileVisualFactory
    {
        private const int TextureWidth = 32;
        private const int TextureHeight = 16;
        private const int SmoothTextureScale = 3;
        private const int SmoothTextureWidth = TextureWidth * SmoothTextureScale;
        private const int SmoothTextureHeight = TextureHeight * SmoothTextureScale;
        private const float PixelsPerUnit = TextureWidth;
        private const float SmoothPixelsPerUnit = SmoothTextureWidth;
        private const float ObstaclePixelsPerUnit = 78f;
        private const string ObstacleResourcePathPrefix = "Terrain/RockRubble_";
        public const int ObstacleDetailVariantCount = 4;

        private static Sprite stoneTileSprite;
        private static Sprite tileSideSprite;
        private static Sprite highlightSprite;
        private static Sprite moveHighlightSprite;
        private static Sprite enemyAttackThreatSprite;
        private static Sprite enemyMoveThreatSprite;
        private static Sprite enemyAttackOnlyThreatSprite;
        private static Sprite guardianReactionReachableHighlightSprite;
        private static Sprite guardianReactionBlockedHighlightSprite;
        private static Sprite moveThreatOverlapSprite;
        private static Sprite groundDetailSprite;
        private static Sprite[] obstacleDetailSprites;
        private static Sprite goalRuneSprite;
        private static Sprite goalHaloSprite;

        public static Sprite GetStoneTileSprite()
        {
            if (stoneTileSprite != null)
            {
                return stoneTileSprite;
            }

            var texture = CreateSmoothTexture();
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha <= 0f)
                    {
                        continue;
                    }

                    var sampleX = x / SmoothTextureScale;
                    var sampleY = y / SmoothTextureScale;
                    var shade = 0.82f + sampleY * 0.004f;
                    if (distance > 0.82f)
                    {
                        shade = y >= SmoothTextureHeight * 0.5f ? 0.62f : 0.5f;
                    }
                    else if (distance > 0.7f)
                    {
                        shade = y >= SmoothTextureHeight * 0.5f ? 0.74f : 0.62f;
                    }
                    else if ((sampleX * 11 + sampleY * 19) % 73 == 0)
                    {
                        shade -= 0.045f;
                    }
                    else if ((sampleX * 7 + sampleY * 3) % 59 == 0)
                    {
                        shade -= 0.025f;
                    }
                    else if ((sampleX * 3 + sampleY * 5) % 31 == 0)
                    {
                        shade -= 0.018f;
                    }
                    else if ((sampleX + sampleY * 2) % 43 == 0)
                    {
                        shade += 0.018f;
                    }

                    texture.SetPixel(x, y, new Color(shade, shade, shade, edgeAlpha));
                }
            }

            texture.Apply();
            stoneTileSprite = CreateSmoothSprite(texture);
            return stoneTileSprite;
        }

        public static Sprite GetTileSideSprite()
        {
            if (tileSideSprite != null)
            {
                return tileSideSprite;
            }

            var texture = CreateSmoothTexture();
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha > 0f)
                    {
                        var lowerSide = y < SmoothTextureHeight * 0.44f;
                        var edgeBand = distance > 0.7f;
                        var shade = lowerSide ? 0.24f : 0.38f;
                        if (edgeBand)
                        {
                            shade -= 0.025f;
                        }

                        texture.SetPixel(x, y, new Color(shade, shade, shade, edgeAlpha));
                    }
                }
            }

            texture.Apply();
            tileSideSprite = CreateSmoothSprite(texture);
            return tileSideSprite;
        }

        public static Sprite GetHighlightSprite()
        {
            if (highlightSprite != null)
            {
                return highlightSprite;
            }

            var texture = CreateSmoothTexture();
            var fill = new Color(1f, 1f, 1f, 0.26f);
            var edge = new Color(1f, 1f, 1f, 0.9f);
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha > 0f)
                    {
                        texture.SetPixel(x, y, WithAlphaScale(distance > 0.76f ? edge : fill, edgeAlpha));
                    }
                }
            }

            texture.Apply();
            highlightSprite = CreateSmoothSprite(texture);
            return highlightSprite;
        }

        public static Sprite GetMoveHighlightSprite()
        {
            if (moveHighlightSprite != null)
            {
                return moveHighlightSprite;
            }

            var texture = CreateSmoothTexture();
            var fill = new Color(1f, 1f, 1f, 0.12f);
            var innerEdge = new Color(1f, 1f, 1f, 0.48f);
            var outerEdge = new Color(1f, 1f, 1f, 1f);
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha > 0f)
                    {
                        var color = distance > 0.82f
                            ? outerEdge
                            : distance > 0.68f ? innerEdge : fill;
                        texture.SetPixel(x, y, WithAlphaScale(color, edgeAlpha));
                    }
                }
            }

            texture.Apply();
            moveHighlightSprite = CreateSmoothSprite(texture);
            return moveHighlightSprite;
        }

        public static Sprite GetEnemyAttackThreatSprite()
        {
            if (enemyAttackThreatSprite != null)
            {
                return enemyAttackThreatSprite;
            }

            enemyAttackThreatSprite = CreateThreatDiamondSprite(
                new Color(1f, 1f, 1f, 0.1f),
                new Color(1f, 1f, 1f, 0.38f),
                new Color(1f, 1f, 1f, 0.76f));
            return enemyAttackThreatSprite;
        }

        public static Sprite GetEnemyMoveThreatSprite()
        {
            if (enemyMoveThreatSprite != null)
            {
                return enemyMoveThreatSprite;
            }

            enemyMoveThreatSprite = CreateThreatDiamondSprite(
                new Color(1f, 1f, 1f, 0.18f),
                new Color(1f, 1f, 1f, 0.4f),
                new Color(1f, 1f, 1f, 0.64f));
            return enemyMoveThreatSprite;
        }

        public static Sprite GetEnemyAttackOnlyThreatSprite()
        {
            if (enemyAttackOnlyThreatSprite != null)
            {
                return enemyAttackOnlyThreatSprite;
            }

            enemyAttackOnlyThreatSprite = CreateThreatDiamondSprite(
                new Color(1f, 1f, 1f, 0.14f),
                new Color(1f, 1f, 1f, 0.34f),
                new Color(1f, 1f, 1f, 0.58f));
            return enemyAttackOnlyThreatSprite;
        }

        private static Sprite CreateThreatDiamondSprite(Color fill, Color innerEdge, Color outerEdge)
        {
            var texture = CreateSmoothTexture();
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha > 0f)
                    {
                        var color = distance > 0.82f
                            ? outerEdge
                            : distance > 0.68f ? innerEdge : fill;
                        texture.SetPixel(x, y, WithAlphaScale(color, edgeAlpha));
                    }
                }
            }

            texture.Apply();
            return CreateSmoothSprite(texture);
        }

        public static Sprite GetGuardianReactionReachableHighlightSprite()
        {
            if (guardianReactionReachableHighlightSprite != null)
            {
                return guardianReactionReachableHighlightSprite;
            }

            guardianReactionReachableHighlightSprite = CreateGuardianReactionSprite(
                new Color(1f, 1f, 1f, 0.22f),
                new Color(1f, 1f, 1f, 0.42f),
                new Color(1f, 1f, 1f, 0.82f),
                new Color(1f, 1f, 1f, 0.95f));
            return guardianReactionReachableHighlightSprite;
        }

        public static Sprite GetGuardianReactionBlockedHighlightSprite()
        {
            if (guardianReactionBlockedHighlightSprite != null)
            {
                return guardianReactionBlockedHighlightSprite;
            }

            guardianReactionBlockedHighlightSprite = CreateGuardianReactionSprite(
                new Color(1f, 1f, 1f, 0.22f),
                new Color(1f, 1f, 1f, 0.44f),
                new Color(1f, 1f, 1f, 0.7f),
                new Color(1f, 1f, 1f, 0.84f));
            return guardianReactionBlockedHighlightSprite;
        }

        private static Sprite CreateGuardianReactionSprite(Color centerWash, Color innerEdge, Color outerEdge, Color cornerTick)
        {
            var texture = CreateSmoothTexture();
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha <= 0f)
                    {
                        continue;
                    }

                    var color = distance > 0.88f
                        ? outerEdge
                        : distance > 0.76f ? innerEdge : centerWash;

                    var nearHorizontalCorner = x <= 2 * SmoothTextureScale || x >= SmoothTextureWidth - 3 * SmoothTextureScale;
                    var nearVerticalCorner = y <= SmoothTextureScale || y >= SmoothTextureHeight - 2 * SmoothTextureScale;
                    if (distance > 0.78f && (nearHorizontalCorner || nearVerticalCorner))
                    {
                        color = cornerTick;
                    }

                    texture.SetPixel(x, y, WithAlphaScale(color, edgeAlpha));
                }
            }

            texture.Apply();
            return CreateSmoothSprite(texture);
        }

        public static Sprite GetMoveThreatOverlapSprite()
        {
            if (moveThreatOverlapSprite != null)
            {
                return moveThreatOverlapSprite;
            }

            var texture = CreateSmoothTexture();
            var moveFill = new Color(0.16f, 0.62f, 1f, 0.14f);
            var moveEdge = new Color(0.42f, 0.78f, 1f, 0.44f);
            var dangerEdge = new Color(0.95f, 0.18f, 0.08f, 0.72f);
            for (var y = 0; y < SmoothTextureHeight; y++)
            {
                for (var x = 0; x < SmoothTextureWidth; x++)
                {
                    var distance = GetSmoothDiamondDistance(x, y);
                    var edgeAlpha = GetDiamondEdgeAlpha(distance);
                    if (edgeAlpha > 0f)
                    {
                        var color = distance > 0.82f
                            ? dangerEdge
                            : distance > 0.68f ? moveEdge : moveFill;
                        texture.SetPixel(x, y, WithAlphaScale(color, edgeAlpha));
                    }
                }
            }

            texture.Apply();
            moveThreatOverlapSprite = CreateSmoothSprite(texture);
            return moveThreatOverlapSprite;
        }

        public static Sprite GetGroundDetailSprite()
        {
            if (groundDetailSprite != null)
            {
                return groundDetailSprite;
            }

            var texture = CreateTexture();
            var grain = new Color(1f, 1f, 1f, 0.055f);
            var crack = new Color(0.1f, 0.12f, 0.14f, 0.065f);

            for (var y = 0; y < TextureHeight; y++)
            {
                for (var x = 0; x < TextureWidth; x++)
                {
                    var distance = GetDiamondDistance(x, y);
                    if (distance > 0.82f)
                    {
                        continue;
                    }

                    if ((x * 17 + y * 5) % 83 == 0)
                    {
                        texture.SetPixel(x, y, grain);
                    }
                    else if ((x * 7 + y * 13) % 97 == 0)
                    {
                        texture.SetPixel(x, y, crack);
                    }
                }
            }

            texture.Apply();
            groundDetailSprite = CreateSprite(texture);
            return groundDetailSprite;
        }

        public static Sprite GetObstacleDetailSprite()
        {
            return GetObstacleDetailSprite(0);
        }

        public static Sprite GetObstacleDetailSprite(int variant)
        {
            obstacleDetailSprites = obstacleDetailSprites ?? new Sprite[ObstacleDetailVariantCount];
            var normalizedVariant = Math.Abs(variant) % ObstacleDetailVariantCount;
            if (obstacleDetailSprites[normalizedVariant] != null)
            {
                return obstacleDetailSprites[normalizedVariant];
            }

            var loadedObstacleSprite = LoadObstacleDetailSprite(normalizedVariant);
            if (loadedObstacleSprite != null)
            {
                obstacleDetailSprites[normalizedVariant] = loadedObstacleSprite;
                return obstacleDetailSprites[normalizedVariant];
            }

            var texture = CreateTexture();
            DrawObstacleRubble(texture, normalizedVariant);

            texture.Apply();
            obstacleDetailSprites[normalizedVariant] = CreateSprite(texture);
            return obstacleDetailSprites[normalizedVariant];
        }

        private static Sprite LoadObstacleDetailSprite(int variant)
        {
            var texture = Resources.Load<Texture2D>(ObstacleResourcePathPrefix + variant);
            if (texture == null)
            {
                return null;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.28f),
                ObstaclePixelsPerUnit);
        }

        private static void DrawObstacleRubble(Texture2D texture, int variant)
        {
            var contactShadow = new Color(0.1f, 0.085f, 0.065f, 0.3f);
            var deepCrevice = new Color(0.12f, 0.11f, 0.09f, 0.78f);
            var lowerStone = new Color(0.46f, 0.42f, 0.35f, 0.88f);
            var midStone = new Color(0.66f, 0.6f, 0.5f, 0.9f);
            var upperStone = new Color(0.8f, 0.75f, 0.64f, 0.86f);
            var chippedStone = new Color(0.56f, 0.51f, 0.43f, 0.86f);
            var highlight = new Color(0.9f, 0.86f, 0.7f, 0.6f);

            DrawCommonRubbleBase(texture, contactShadow, deepCrevice, lowerStone, midStone, chippedStone);

            switch (variant)
            {
                case 1:
                    DrawLeaningSlabRubble(texture, deepCrevice, lowerStone, midStone, upperStone, chippedStone, highlight);
                    break;
                case 2:
                    DrawLowCollapsedRubble(texture, deepCrevice, lowerStone, midStone, upperStone, chippedStone, highlight);
                    break;
                case 3:
                    DrawSplitBlockRubble(texture, deepCrevice, lowerStone, midStone, upperStone, chippedStone, highlight);
                    break;
                default:
                    DrawBrokenWallRubble(texture, deepCrevice, lowerStone, midStone, upperStone, chippedStone, highlight);
                    break;
            }
        }

        private static void DrawCommonRubbleBase(Texture2D texture, Color contactShadow, Color deepCrevice, Color lowerStone, Color midStone, Color chippedStone)
        {
            FillRect(texture, 7, 2, 23, 3, contactShadow);
            FillRect(texture, 10, 3, 25, 4, contactShadow);
            FillRect(texture, 8, 4, 21, 5, deepCrevice);
            FillRect(texture, 11, 5, 24, 6, deepCrevice);
            FillRect(texture, 7, 5, 10, 7, chippedStone);
            FillRect(texture, 22, 5, 24, 7, chippedStone);
            FillRect(texture, 9, 6, 13, 8, lowerStone);
            FillRect(texture, 18, 6, 22, 8, lowerStone);
            SetPixel(texture, 6, 4, midStone);
            SetPixel(texture, 25, 5, lowerStone);
            SetPixel(texture, 8, 3, lowerStone);
            SetPixel(texture, 23, 3, chippedStone);
        }

        private static void DrawBrokenWallRubble(Texture2D texture, Color deepCrevice, Color lowerStone, Color midStone, Color upperStone, Color chippedStone, Color highlight)
        {
            FillRect(texture, 8, 7, 13, 10, lowerStone);
            FillRect(texture, 14, 6, 19, 11, midStone);
            FillRect(texture, 20, 8, 23, 10, chippedStone);
            FillRect(texture, 11, 10, 17, 13, upperStone);
            FillRect(texture, 18, 11, 21, 12, midStone);
            FillRect(texture, 10, 8, 11, 9, deepCrevice);
            FillRect(texture, 16, 7, 17, 9, deepCrevice);
            FillRect(texture, 19, 10, 20, 11, deepCrevice);
            SetPixel(texture, 12, 14, upperStone);
            SetPixel(texture, 15, 14, midStone);
            SetPixel(texture, 21, 13, chippedStone);
            SetPixel(texture, 12, 12, highlight);
            SetPixel(texture, 17, 11, highlight);
        }

        private static void DrawLeaningSlabRubble(Texture2D texture, Color deepCrevice, Color lowerStone, Color midStone, Color upperStone, Color chippedStone, Color highlight)
        {
            FillRect(texture, 8, 6, 12, 9, lowerStone);
            FillRect(texture, 13, 7, 17, 12, midStone);
            FillRect(texture, 16, 10, 20, 14, upperStone);
            FillRect(texture, 20, 7, 23, 10, chippedStone);
            FillRect(texture, 11, 11, 14, 13, chippedStone);
            FillRect(texture, 15, 8, 16, 10, deepCrevice);
            FillRect(texture, 18, 12, 19, 13, deepCrevice);
            FillRect(texture, 21, 8, 22, 9, deepCrevice);
            SetPixel(texture, 9, 10, midStone);
            SetPixel(texture, 18, 15, upperStone);
            SetPixel(texture, 22, 11, lowerStone);
            SetPixel(texture, 17, 13, highlight);
            SetPixel(texture, 14, 11, highlight);
        }

        private static void DrawLowCollapsedRubble(Texture2D texture, Color deepCrevice, Color lowerStone, Color midStone, Color upperStone, Color chippedStone, Color highlight)
        {
            FillRect(texture, 7, 7, 12, 9, lowerStone);
            FillRect(texture, 12, 8, 18, 11, midStone);
            FillRect(texture, 18, 7, 23, 10, lowerStone);
            FillRect(texture, 10, 11, 16, 13, upperStone);
            FillRect(texture, 17, 11, 21, 12, chippedStone);
            FillRect(texture, 9, 8, 10, 9, deepCrevice);
            FillRect(texture, 14, 9, 15, 10, deepCrevice);
            FillRect(texture, 20, 8, 21, 9, deepCrevice);
            SetPixel(texture, 6, 8, chippedStone);
            SetPixel(texture, 24, 8, midStone);
            SetPixel(texture, 11, 14, midStone);
            SetPixel(texture, 19, 13, upperStone);
            SetPixel(texture, 13, 12, highlight);
            SetPixel(texture, 18, 11, highlight);
        }

        private static void DrawSplitBlockRubble(Texture2D texture, Color deepCrevice, Color lowerStone, Color midStone, Color upperStone, Color chippedStone, Color highlight)
        {
            FillRect(texture, 8, 6, 13, 11, midStone);
            FillRect(texture, 15, 6, 21, 10, lowerStone);
            FillRect(texture, 12, 11, 18, 14, upperStone);
            FillRect(texture, 19, 10, 23, 12, chippedStone);
            FillRect(texture, 10, 12, 11, 13, chippedStone);
            FillRect(texture, 13, 7, 14, 12, deepCrevice);
            FillRect(texture, 18, 8, 19, 10, deepCrevice);
            FillRect(texture, 9, 9, 10, 10, deepCrevice);
            SetPixel(texture, 7, 11, lowerStone);
            SetPixel(texture, 22, 13, midStone);
            SetPixel(texture, 15, 15, upperStone);
            SetPixel(texture, 11, 10, highlight);
            SetPixel(texture, 16, 13, highlight);
            SetPixel(texture, 20, 11, highlight);
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
            return CreateTexture(TextureWidth, TextureHeight, FilterMode.Point);
        }

        private static Texture2D CreateSmoothTexture()
        {
            return CreateTexture(SmoothTextureWidth, SmoothTextureHeight, FilterMode.Bilinear);
        }

        private static Texture2D CreateTexture(int width, int height, FilterMode filterMode)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            var transparent = new Color(0f, 0f, 0f, 0f);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            return texture;
        }

        private static Sprite CreateSprite(Texture2D texture)
        {
            return CreateSprite(texture, PixelsPerUnit);
        }

        private static Sprite CreateSmoothSprite(Texture2D texture)
        {
            return CreateSprite(texture, SmoothPixelsPerUnit);
        }

        private static Sprite CreateSprite(Texture2D texture, float pixelsPerUnit)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit);
        }

        private static float GetDiamondDistance(int x, int y)
        {
            return GetDiamondDistance(x, y, TextureWidth, TextureHeight);
        }

        private static float GetSmoothDiamondDistance(int x, int y)
        {
            return GetDiamondDistance(x, y, SmoothTextureWidth, SmoothTextureHeight);
        }

        private static float GetDiamondDistance(int x, int y, int width, int height)
        {
            var centerX = (width - 1) * 0.5f;
            var centerY = (height - 1) * 0.5f;
            var normalizedX = (float)Math.Abs(x - centerX) / (width * 0.5f);
            var normalizedY = (float)Math.Abs(y - centerY) / (height * 0.5f);
            return normalizedX + normalizedY;
        }

        private static float GetDiamondEdgeAlpha(float distance)
        {
            return Mathf.Clamp01((1.035f - distance) / 0.055f);
        }

        private static Color WithAlphaScale(Color color, float alphaScale)
        {
            color.a *= alphaScale;
            return color;
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
