using System;
using System.Collections.Generic;
using System.Text;
using SRPG.Battle;
using SRPG.Grid;
using SRPG.Units;
using UnityEngine;

namespace SRPG.Visual
{
    public sealed class EnemyIntentPreview : MonoBehaviour
    {
        private const int IntentSortingOrder = -505;
        private static readonly Color IntentOutlineColor = new Color(0.07f, 0.015f, 0.008f, 0.82f);
        private static readonly Color IntentLineColor = new Color(1f, 0.42f, 0.1f, 0.9f);
        private static readonly Color IntentArrowColor = new Color(1f, 0.72f, 0.18f, 1f);
        private static readonly Color TargetOutlineColor = new Color(0.08f, 0.025f, 0.008f, 0.9f);
        private static readonly Color TargetMarkerColor = new Color(1f, 0.76f, 0.16f, 0.98f);
        private static readonly Color OriginMarkerColor = new Color(1f, 0.5f, 0.1f, 0.92f);

        private readonly List<GameObject> visualObjects = new List<GameObject>();
        private static Sprite lineSprite;
        private static Sprite arrowSprite;
        private static Sprite targetMarkerSprite;
        private string currentSignature = string.Empty;

        public void Refresh(GridManager gridManager, Unit selectedEnemy)
        {
            var turnManager = TurnManager.Instance;
            if (gridManager == null || turnManager == null || turnManager.IsBattleEnded)
            {
                Clear();
                return;
            }

            var intents = new List<KeyValuePair<Unit, Unit>>();
            if (selectedEnemy != null)
            {
                AddIntent(intents, turnManager, selectedEnemy);
            }
            else
            {
                foreach (var enemy in gridManager.GetUnitsByFaction(Faction.Enemy))
                {
                    AddIntent(intents, turnManager, enemy);
                }
            }

            var signature = BuildSignature(intents);
            if (signature == currentSignature)
            {
                return;
            }

            ClearVisualObjects();
            currentSignature = signature;

            var markedTargets = new HashSet<Unit>();
            foreach (var intent in intents)
            {
                CreateIntentVisual(intent.Key, intent.Value, gridManager.CellSize);
                CreateOriginMarker(intent.Key, gridManager.CellSize);
                if (markedTargets.Add(intent.Value))
                {
                    CreateTargetMarker(intent.Value, gridManager.CellSize);
                }
            }
        }

        public void Clear()
        {
            currentSignature = string.Empty;
            ClearVisualObjects();
        }

        private static void AddIntent(List<KeyValuePair<Unit, Unit>> intents, TurnManager turnManager, Unit enemy)
        {
            if (enemy == null || enemy.IsDead || enemy.Faction != Faction.Enemy)
            {
                return;
            }

            var target = turnManager.GetEnemyCurrentAttackTarget(enemy);
            if (target != null && !target.IsDead)
            {
                intents.Add(new KeyValuePair<Unit, Unit>(enemy, target));
            }
        }

        private static string BuildSignature(List<KeyValuePair<Unit, Unit>> intents)
        {
            var builder = new StringBuilder();
            foreach (var intent in intents)
            {
                builder.Append(intent.Key.name)
                    .Append('@').Append(intent.Key.GridPosition.x).Append(',').Append(intent.Key.GridPosition.y)
                    .Append('>').Append(intent.Value.name)
                    .Append('@').Append(intent.Value.GridPosition.x).Append(',').Append(intent.Value.GridPosition.y)
                    .Append(';');
            }

            return builder.ToString();
        }

        private void CreateIntentVisual(Unit enemy, Unit target, float cellSize)
        {
            var direction = BoardProjection.GetIsoDirection(enemy.GridPosition, target.GridPosition);
            if (direction.x == 0f && direction.y == 0f)
            {
                return;
            }

            var from = BoardProjection.GridToIsoWorld(enemy.GridPosition, cellSize) + direction * 0.34f;
            var to = BoardProjection.GridToIsoWorld(target.GridPosition, cellSize) - direction * 0.38f;
            var delta = to - from;
            var length = (float)Math.Sqrt(delta.x * delta.x + delta.y * delta.y);
            var angle = GetAngle(direction);

            var center = (from + to) * 0.5f + new Vector3(0f, 0.04f, -0.18f);
            CreateDirectionalVisual("EnemyIntentLineOutline", GetLineSprite(), IntentOutlineColor, IntentSortingOrder, center, new Vector3(length, 0.11f, 1f), angle);
            CreateDirectionalVisual("EnemyIntentLine", GetLineSprite(), IntentLineColor, IntentSortingOrder + 1, center, new Vector3(length, 0.065f, 1f), angle);

            var arrowPosition = to + new Vector3(0f, 0.04f, -0.19f);
            CreateDirectionalVisual("EnemyIntentArrowOutline", GetArrowSprite(), IntentOutlineColor, IntentSortingOrder + 2, arrowPosition, new Vector3(0.4f, 0.3f, 1f), angle);
            CreateDirectionalVisual("EnemyIntentArrow", GetArrowSprite(), IntentArrowColor, IntentSortingOrder + 3, arrowPosition, new Vector3(0.31f, 0.22f, 1f), angle);
        }

        private void CreateTargetMarker(Unit target, float cellSize)
        {
            var position = BoardProjection.GridToIsoWorld(target.GridPosition, cellSize) + new Vector3(0f, 0.03f, -0.2f);
            var outline = CreateVisualObject("EnemyIntentTargetOutline", GetTargetMarkerSprite(), TargetOutlineColor, IntentSortingOrder + 4);
            outline.transform.position = position;
            outline.transform.localScale = new Vector3(1f * cellSize, 1f * cellSize, 1f);

            var marker = CreateVisualObject("EnemyIntentTarget", GetTargetMarkerSprite(), TargetMarkerColor, IntentSortingOrder + 5);
            marker.transform.position = position;
            marker.transform.localScale = new Vector3(0.86f * cellSize, 0.86f * cellSize, 1f);
        }

        private void CreateOriginMarker(Unit enemy, float cellSize)
        {
            var position = BoardProjection.GridToIsoWorld(enemy.GridPosition, cellSize) + new Vector3(0f, 0.03f, -0.2f);
            var outline = CreateVisualObject("EnemyIntentOriginOutline", GetTargetMarkerSprite(), TargetOutlineColor, IntentSortingOrder + 4);
            outline.transform.position = position;
            outline.transform.localScale = new Vector3(0.42f * cellSize, 0.42f * cellSize, 1f);

            var marker = CreateVisualObject("EnemyIntentOrigin", GetTargetMarkerSprite(), OriginMarkerColor, IntentSortingOrder + 5);
            marker.transform.position = position;
            marker.transform.localScale = new Vector3(0.31f * cellSize, 0.31f * cellSize, 1f);
        }

        private void CreateDirectionalVisual(string objectName, Sprite sprite, Color color, int sortingOrder, Vector3 position, Vector3 scale, float angle)
        {
            var visual = CreateVisualObject(objectName, sprite, color, sortingOrder);
            visual.transform.position = position;
            visual.transform.localScale = scale;
            visual.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        }

        private GameObject CreateVisualObject(string objectName, Sprite sprite, Color color, int sortingOrder)
        {
            var visual = new GameObject(objectName);
            visual.transform.SetParent(transform, false);
            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            visualObjects.Add(visual);
            return visual;
        }

        private void ClearVisualObjects()
        {
            foreach (var visual in visualObjects)
            {
                if (visual != null)
                {
                    Destroy(visual);
                }
            }

            visualObjects.Clear();
        }

        private static float GetAngle(Vector3 direction)
        {
            return (float)(Math.Atan2(direction.y, direction.x) * 180.0 / Math.PI);
        }

        private static Sprite GetLineSprite()
        {
            if (lineSprite == null)
            {
                var texture = new Texture2D(1, 1);
                texture.filterMode = FilterMode.Point;
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                lineSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            }

            return lineSprite;
        }

        private static Sprite GetArrowSprite()
        {
            if (arrowSprite == null)
            {
                const int width = 12;
                const int height = 8;
                var texture = CreateTransparentTexture(width, height);
                for (var x = 0; x < width; x++)
                {
                    var halfHeight = x * (height * 0.5f) / (width - 1);
                    for (var y = 0; y < height; y++)
                    {
                        if (Math.Abs(y - (height - 1) * 0.5f) <= halfHeight)
                        {
                            texture.SetPixel(x, y, Color.white);
                        }
                    }
                }

                texture.Apply();
                arrowSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), width);
            }

            return arrowSprite;
        }

        private static Sprite GetTargetMarkerSprite()
        {
            if (targetMarkerSprite == null)
            {
                const int width = 32;
                const int height = 16;
                var texture = CreateTransparentTexture(width, height);
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var distance = GetDiamondDistance(x, y, width, height);
                        if (distance <= 1f && distance >= 0.82f)
                        {
                            texture.SetPixel(x, y, Color.white);
                        }
                    }
                }

                texture.Apply();
                targetMarkerSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), width);
            }

            return targetMarkerSprite;
        }

        private static Texture2D CreateTransparentTexture(int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
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

        private static float GetDiamondDistance(int x, int y, int width, int height)
        {
            var centerX = (width - 1) * 0.5f;
            var centerY = (height - 1) * 0.5f;
            return (float)Math.Abs(x - centerX) / (width * 0.5f) +
                   (float)Math.Abs(y - centerY) / (height * 0.5f);
        }
    }
}
