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
        private static readonly Color MoveOutlineColor = new Color(0.015f, 0.08f, 0.09f, 0.84f);
        private static readonly Color MoveMarkerColor = new Color(0.22f, 0.92f, 0.98f, 0.82f);
        private static readonly Color MoveArrowColor = new Color(0.46f, 1f, 0.92f, 0.9f);
        private static readonly Color MoveDestinationOutlineColor = new Color(0.01f, 0.09f, 0.1f, 0.86f);
        private static readonly Color MoveDestinationMarkerColor = new Color(0.26f, 1f, 0.9f, 0.78f);
        private static readonly Color GuardOutlineColor = new Color(0.055f, 0.06f, 0.025f, 0.78f);
        private static readonly Color GuardMarkerColor = new Color(0.82f, 0.78f, 0.36f, 0.66f);

        private readonly List<GameObject> visualObjects = new List<GameObject>();
        private static Sprite lineSprite;
        private static Sprite arrowSprite;
        private static Sprite targetMarkerSprite;
        private string currentSignature = string.Empty;

        public void Refresh(GridManager gridManager, Unit selectedEnemy)
        {
            var turnManager = TurnManager.Instance;
            if (gridManager == null || turnManager == null || turnManager.IsBattleEnded || !turnManager.IsPlayerTurn)
            {
                Clear();
                return;
            }

            var intents = new List<EnemyIntentData>();
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
                switch (intent.Type)
                {
                    case EnemyIntentType.AttackNow:
                        CreateAttackIntentVisual(intent.Enemy, intent.Target, gridManager.CellSize);
                        CreateOriginMarker(intent.Enemy, gridManager.CellSize);
                        if (intent.Target != null && markedTargets.Add(intent.Target))
                        {
                            CreateTargetMarker(intent.Target, gridManager.CellSize);
                        }
                        break;
                    case EnemyIntentType.MoveToward:
                        CreateMoveTowardVisual(intent, gridManager.CellSize);
                        break;
                    case EnemyIntentType.Guard:
                        CreateGuardMarker(intent.Enemy, gridManager.CellSize);
                        break;
                }
            }
        }

        public void Clear()
        {
            currentSignature = string.Empty;
            ClearVisualObjects();
        }

        private static void AddIntent(List<EnemyIntentData> intents, TurnManager turnManager, Unit enemy)
        {
            if (enemy == null || enemy.IsDead || enemy.Faction != Faction.Enemy)
            {
                return;
            }

            var intent = turnManager.GetEnemyIntentPreview(enemy);
            if (intent.Type != EnemyIntentType.None)
            {
                intents.Add(intent);
            }
        }

        private static string BuildSignature(List<EnemyIntentData> intents)
        {
            var builder = new StringBuilder();
            foreach (var intent in intents)
            {
                builder.Append(intent.Enemy.name)
                    .Append('@').Append(intent.Enemy.GridPosition.x).Append(',').Append(intent.Enemy.GridPosition.y)
                    .Append(':').Append((int)intent.Type);

                if (intent.Target != null)
                {
                    builder.Append('>').Append(intent.Target.name)
                        .Append('@').Append(intent.Target.GridPosition.x).Append(',').Append(intent.Target.GridPosition.y);
                }

                if (intent.HasMoveDestination)
                {
                    builder.Append("->").Append(intent.MoveDestination.x).Append(',').Append(intent.MoveDestination.y);
                }

                builder.Append(';');
            }

            return builder.ToString();
        }

        private void CreateAttackIntentVisual(Unit enemy, Unit target, float cellSize)
        {
            if (enemy == null || target == null)
            {
                return;
            }

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
            var attackArrowAngle = angle + 180f;
            CreateDirectionalVisual("EnemyIntentArrowOutline", GetArrowSprite(), IntentOutlineColor, IntentSortingOrder + 2, arrowPosition, new Vector3(0.4f, 0.3f, 1f), attackArrowAngle);
            CreateDirectionalVisual("EnemyIntentArrow", GetArrowSprite(), IntentArrowColor, IntentSortingOrder + 3, arrowPosition, new Vector3(0.31f, 0.22f, 1f), attackArrowAngle);
        }

        private void CreateMoveTowardVisual(EnemyIntentData intent, float cellSize)
        {
            if (intent.Enemy == null || intent.Target == null)
            {
                return;
            }

            var directionCoordinates = intent.HasMoveDestination ? intent.MoveDestination : intent.Target.GridPosition;
            var direction = BoardProjection.GetIsoDirection(intent.Enemy.GridPosition, directionCoordinates);

            if (direction.x == 0f && direction.y == 0f)
            {
                return;
            }

            var from = BoardProjection.GridToIsoWorld(intent.Enemy.GridPosition, cellSize) + direction * 0.3f;
            var to = intent.HasMoveDestination
                ? BoardProjection.GridToIsoWorld(intent.MoveDestination, cellSize) - direction * 0.18f
                : from + direction * 1.05f * cellSize;
            var delta = to - from;
            var angle = GetAngle(direction);
            const int segmentCount = 5;

            for (var index = 1; index <= segmentCount; index++)
            {
                var progress = index / (segmentCount + 1f);
                var position = from + delta * progress + new Vector3(0f, 0.035f, -0.17f);
                CreateDirectionalVisual("EnemyMoveIntentOutline", GetLineSprite(), MoveOutlineColor, IntentSortingOrder, position, new Vector3(0.2f, 0.085f, 1f), angle);
                CreateDirectionalVisual("EnemyMoveIntentStep", GetLineSprite(), MoveMarkerColor, IntentSortingOrder + 1, position, new Vector3(0.145f, 0.045f, 1f), angle);
            }

            var arrowPosition = to + new Vector3(0f, 0.035f, -0.18f);
            var arrowAngle = angle + 180f;
            CreateMoveDestinationMarker(to, direction, angle, cellSize);
            CreateDirectionalVisual("EnemyMoveIntentArrowOutline", GetArrowSprite(), MoveOutlineColor, IntentSortingOrder + 2, arrowPosition, new Vector3(0.34f, 0.23f, 1f), arrowAngle);
            CreateDirectionalVisual("EnemyMoveIntentArrow", GetArrowSprite(), MoveArrowColor, IntentSortingOrder + 3, arrowPosition, new Vector3(0.25f, 0.17f, 1f), arrowAngle);
        }

        private void CreateMoveDestinationMarker(Vector3 destination, Vector3 direction, float angle, float cellSize)
        {
            var perpendicular = new Vector3(-direction.y, direction.x, 0f);
            var markerCenter = destination - direction * 0.24f * cellSize + new Vector3(0f, 0.035f, -0.18f);
            var firstStep = markerCenter - direction * 0.065f * cellSize + perpendicular * 0.055f * cellSize;
            var secondStep = markerCenter + direction * 0.065f * cellSize - perpendicular * 0.055f * cellSize;

            var ringPosition = destination + new Vector3(0f, 0.03f, -0.19f);
            var ringOutline = CreateVisualObject("EnemyMoveIntentDestinationOutline", GetTargetMarkerSprite(), MoveDestinationOutlineColor, IntentSortingOrder + 2);
            ringOutline.transform.position = ringPosition;
            ringOutline.transform.localScale = new Vector3(0.78f * cellSize, 0.78f * cellSize, 1f);

            var ring = CreateVisualObject("EnemyMoveIntentDestination", GetTargetMarkerSprite(), MoveDestinationMarkerColor, IntentSortingOrder + 3);
            ring.transform.position = ringPosition;
            ring.transform.localScale = new Vector3(0.64f * cellSize, 0.64f * cellSize, 1f);

            CreateDirectionalVisual("EnemyMoveIntentFootprintOutline", GetLineSprite(), MoveOutlineColor, IntentSortingOrder + 2, firstStep, new Vector3(0.13f, 0.075f, 1f), angle);
            CreateDirectionalVisual("EnemyMoveIntentFootprint", GetLineSprite(), MoveMarkerColor, IntentSortingOrder + 3, firstStep, new Vector3(0.085f, 0.042f, 1f), angle);
            CreateDirectionalVisual("EnemyMoveIntentFootprintOutline", GetLineSprite(), MoveOutlineColor, IntentSortingOrder + 2, secondStep, new Vector3(0.13f, 0.075f, 1f), angle);
            CreateDirectionalVisual("EnemyMoveIntentFootprint", GetLineSprite(), MoveMarkerColor, IntentSortingOrder + 3, secondStep, new Vector3(0.085f, 0.042f, 1f), angle);
        }

        private void CreateGuardMarker(Unit enemy, float cellSize)
        {
            if (enemy == null)
            {
                return;
            }

            var position = BoardProjection.GridToIsoWorld(enemy.GridPosition, cellSize) + new Vector3(0f, 0.03f, -0.19f);
            var outline = CreateVisualObject("EnemyGuardIntentOutline", GetTargetMarkerSprite(), GuardOutlineColor, IntentSortingOrder + 2);
            outline.transform.position = position;
            outline.transform.localScale = new Vector3(0.62f * cellSize, 0.62f * cellSize, 1f);

            var marker = CreateVisualObject("EnemyGuardIntent", GetTargetMarkerSprite(), GuardMarkerColor, IntentSortingOrder + 3);
            marker.transform.position = position;
            marker.transform.localScale = new Vector3(0.5f * cellSize, 0.5f * cellSize, 1f);
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
