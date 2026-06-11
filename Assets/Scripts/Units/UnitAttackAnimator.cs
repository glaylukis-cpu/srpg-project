using System.Collections;
using UnityEngine;

namespace SRPG.Units
{
    public static class UnitAttackAnimator
    {
        public static bool EnableAttackAnimations = true;

        private const float SoldierLungeDistance = 0.15f;
        private const float KnightLungeDistance = 0.12f;
        private const float RogueLungeDistance = 0.22f;
        private const float ArcherDrawDistance = -0.05f;
        private const float SlashEffectDuration = 0.12f;
        private const float HeavyEffectDuration = 0.14f;
        private const float ArrowTravelDuration = 0.13f;

        private static Sprite slashSprite;
        private static Sprite heavyHitSprite;
        private static Sprite arrowSprite;

        public static IEnumerator PlayAttackAnimation(Unit attacker, Unit target)
        {
            if (!EnableAttackAnimations || attacker == null || target == null || attacker.IsDead || target.IsDead)
            {
                yield break;
            }

            switch (attacker.UnitType)
            {
                case UnitType.Knight:
                    yield return PlayKnightAttack(attacker, target);
                    break;
                case UnitType.Archer:
                    yield return PlayArcherAttack(attacker, target);
                    break;
                case UnitType.Rogue:
                    yield return PlayRogueAttack(attacker, target);
                    break;
                default:
                    yield return PlaySoldierAttack(attacker, target);
                    break;
            }
        }

        private static IEnumerator PlaySoldierAttack(Unit attacker, Unit target)
        {
            var originalPosition = attacker.transform.position;
            var offset = GetDirectionOffset(attacker, target, SoldierLungeDistance);
            yield return MoveUnit(attacker, originalPosition, offset, 0.08f);
            var slashEffect = ShowSlashEffect(attacker, target, 1f);
            yield return new WaitForSeconds(0.06f);
            Object.Destroy(slashEffect);
            yield return MoveUnit(attacker, attacker.transform.position, offset * -1f, 0.08f);
            attacker.transform.position = originalPosition;
        }

        private static IEnumerator PlayKnightAttack(Unit attacker, Unit target)
        {
            var originalPosition = attacker.transform.position;
            yield return ScaleUnit(attacker, 1.06f, 0.08f);
            var offset = GetDirectionOffset(attacker, target, KnightLungeDistance);
            yield return MoveUnit(attacker, originalPosition, offset, 0.10f);
            var hitEffect = ShowHeavyHitEffect(target);
            yield return new WaitForSeconds(0.08f);
            Object.Destroy(hitEffect);
            yield return MoveUnit(attacker, attacker.transform.position, offset * -1f, 0.10f);
            attacker.transform.position = originalPosition;
        }

        private static IEnumerator PlayArcherAttack(Unit attacker, Unit target)
        {
            var originalPosition = attacker.transform.position;
            var drawOffset = GetDirectionOffset(attacker, target, ArcherDrawDistance);
            yield return MoveUnit(attacker, originalPosition, drawOffset, 0.07f);
            yield return new WaitForSeconds(0.04f);
            yield return MoveArrow(attacker, target);
            yield return MoveUnit(attacker, attacker.transform.position, drawOffset * -1f, 0.05f);
            attacker.transform.position = originalPosition;
        }

        private static IEnumerator PlayRogueAttack(Unit attacker, Unit target)
        {
            var originalPosition = attacker.transform.position;
            var offset = GetDirectionOffset(attacker, target, RogueLungeDistance);
            var slashEffect = ShowSlashEffect(attacker, target, 0.82f);
            yield return MoveUnit(attacker, originalPosition, offset, 0.06f);
            yield return new WaitForSeconds(0.035f);
            Object.Destroy(slashEffect);
            yield return MoveUnit(attacker, attacker.transform.position, offset * -1f, 0.07f);
            attacker.transform.position = originalPosition;
        }

        private static IEnumerator MoveUnit(Unit unit, Vector3 startPosition, Vector3 offset, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = duration > 0f ? (float)System.Math.Min(1f, elapsed / duration) : 1f;
                unit.transform.position = startPosition + offset * EaseOut(t);
                yield return null;
            }
        }

        private static IEnumerator ScaleUnit(Unit unit, float scaleMultiplier, float duration)
        {
            var originalScale = unit.transform.localScale;
            unit.transform.localScale = originalScale * scaleMultiplier;
            yield return new WaitForSeconds(duration);
            unit.transform.localScale = originalScale;
        }

        private static IEnumerator MoveArrow(Unit attacker, Unit target)
        {
            var startPosition = attacker.transform.position + GetDirectionOffset(attacker, target, 0.18f);
            var travel = GetDirectionOffset(attacker, target, GetGridDistance(attacker, target));
            var arrow = CreateEffectObject("ArrowEffect", startPosition, GetArrowSprite(), new Color(1f, 0.86f, 0.42f, 0.95f), new Vector3(0.42f, 0.18f, 1f), 22);

            var elapsed = 0f;
            while (elapsed < ArrowTravelDuration)
            {
                elapsed += Time.deltaTime;
                var t = ArrowTravelDuration > 0f ? (float)System.Math.Min(1f, elapsed / ArrowTravelDuration) : 1f;
                arrow.transform.position = startPosition + travel * t;
                yield return null;
            }

            Object.Destroy(arrow);
        }

        private static Vector3 GetDirectionOffset(Unit attacker, Unit target, float distance)
        {
            var dx = target.GridPosition.x - attacker.GridPosition.x;
            var dy = target.GridPosition.y - attacker.GridPosition.y;
            var length = System.Math.Sqrt(dx * dx + dy * dy);
            if (length <= 0.001)
            {
                return new Vector3(0f, 0f, 0f);
            }

            return new Vector3((float)(dx / length) * distance, (float)(dy / length) * distance, -0.04f);
        }

        private static float GetGridDistance(Unit attacker, Unit target)
        {
            var dx = target.GridPosition.x - attacker.GridPosition.x;
            var dy = target.GridPosition.y - attacker.GridPosition.y;
            var length = (float)System.Math.Sqrt(dx * dx + dy * dy);
            return (float)System.Math.Max(0.24f, length);
        }

        private static float EaseOut(float t)
        {
            return 1f - (1f - t) * (1f - t);
        }

        private static GameObject ShowSlashEffect(Unit attacker, Unit target, float scale)
        {
            var position = attacker.transform.position + GetDirectionOffset(attacker, target, 0.42f);
            return CreateEffectObject("SlashEffect", position, GetSlashSprite(), new Color(1f, 0.92f, 0.62f, 0.92f), new Vector3(0.48f * scale, 0.48f * scale, 1f), 23);
        }

        private static GameObject ShowHeavyHitEffect(Unit target)
        {
            return CreateEffectObject("HeavyHitEffect", target.transform.position + new Vector3(0f, 0.08f, -0.1f), GetHeavyHitSprite(), new Color(1f, 0.84f, 0.36f, 0.88f), new Vector3(0.42f, 0.42f, 1f), 23);
        }

        private static GameObject CreateEffectObject(string objectName, Vector3 position, Sprite sprite, Color color, Vector3 scale, int sortingOrder)
        {
            var effectObject = new GameObject(objectName);
            effectObject.transform.position = position;
            effectObject.transform.localScale = scale;

            var renderer = effectObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return effectObject;
        }

        private static Sprite GetSlashSprite()
        {
            if (slashSprite != null)
            {
                return slashSprite;
            }

            var texture = new Texture2D(12, 12);
            texture.filterMode = FilterMode.Point;
            ClearTexture(texture);
            for (var i = 2; i < 10; i++)
            {
                SetPixel(texture, i, i, Color.white);
                SetPixel(texture, i, i - 1, new Color(1f, 0.84f, 0.38f, 0.85f));
            }

            texture.Apply();
            slashSprite = Sprite.Create(texture, new Rect(0, 0, 12, 12), new Vector2(0.5f, 0.5f), 12f);
            return slashSprite;
        }

        private static Sprite GetHeavyHitSprite()
        {
            if (heavyHitSprite != null)
            {
                return heavyHitSprite;
            }

            var texture = new Texture2D(14, 14);
            texture.filterMode = FilterMode.Point;
            ClearTexture(texture);
            for (var y = 2; y < 12; y++)
            {
                for (var x = 2; x < 12; x++)
                {
                    var dx = x - 6.5f;
                    var dy = y - 6.5f;
                    var distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                    if (distance > 3.8f && distance < 5.4f || x == 7 || y == 7)
                    {
                        SetPixel(texture, x, y, new Color(1f, 0.86f, 0.34f, 0.9f));
                    }
                }
            }

            texture.Apply();
            heavyHitSprite = Sprite.Create(texture, new Rect(0, 0, 14, 14), new Vector2(0.5f, 0.5f), 14f);
            return heavyHitSprite;
        }

        private static Sprite GetArrowSprite()
        {
            if (arrowSprite != null)
            {
                return arrowSprite;
            }

            var texture = new Texture2D(10, 4);
            texture.filterMode = FilterMode.Point;
            ClearTexture(texture);
            for (var x = 1; x < 8; x++)
            {
                SetPixel(texture, x, 2, new Color(1f, 0.88f, 0.5f, 1f));
            }

            SetPixel(texture, 8, 2, Color.white);
            SetPixel(texture, 7, 1, Color.white);
            SetPixel(texture, 7, 3, Color.white);
            texture.Apply();
            arrowSprite = Sprite.Create(texture, new Rect(0, 0, 10, 4), new Vector2(0.5f, 0.5f), 10f);
            return arrowSprite;
        }

        private static void ClearTexture(Texture2D texture)
        {
            var transparent = new Color(0f, 0f, 0f, 0f);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }
        }

        private static void SetPixel(Texture2D texture, int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
            {
                return;
            }

            texture.SetPixel(x, y, color);
        }
    }
}
