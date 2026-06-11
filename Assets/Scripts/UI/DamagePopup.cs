using System.Collections.Generic;
using UnityEngine;

namespace SRPG.UI
{
    public class DamagePopup : MonoBehaviour
    {
        private static readonly List<DamagePopup> ActivePopups = new List<DamagePopup>();

        [SerializeField] private float lifetime = 0.9f;
        [SerializeField] private float floatSpeed = 0.9f;

        private TextMesh damageText;
        private float elapsedTime;

        public static void Show(Vector3 worldPosition, int damage, bool defeated)
        {
            var popupObject = new GameObject("DamagePopup");
            popupObject.transform.position = worldPosition + new Vector3(0f, 0.62f, -0.2f);

            var popup = popupObject.AddComponent<DamagePopup>();
            popup.Initialize(damage, defeated);
        }

        public static void ShowText(Vector3 worldPosition, string text, Color color, float characterSize, int fontSize)
        {
            var popupObject = new GameObject("DamagePopupText");
            popupObject.transform.position = worldPosition + new Vector3(0f, 0.72f, -0.2f);

            var popup = popupObject.AddComponent<DamagePopup>();
            popup.InitializeText(text, color, characterSize, fontSize);
        }

        public static void ClearAll()
        {
            for (int i = ActivePopups.Count - 1; i >= 0; i--)
            {
                if (ActivePopups[i] != null)
                {
                    Destroy(ActivePopups[i].gameObject);
                }
            }

            ActivePopups.Clear();
        }

        private void Initialize(int damage, bool defeated)
        {
            ActivePopups.Add(this);

            var textRenderer = gameObject.AddComponent<MeshRenderer>();
            damageText = gameObject.AddComponent<TextMesh>();
            damageText.text = $"-{Mathf.Max(0, damage)}";
            damageText.anchor = TextAnchor.MiddleCenter;
            damageText.alignment = TextAlignment.Center;
            damageText.fontSize = defeated ? 36 : 30;
            damageText.characterSize = defeated ? 0.17f : 0.14f;
            damageText.color = defeated ? new Color(1f, 0.18f, 0.08f, 1f) : new Color(1f, 0.32f, 0.24f, 1f);
            textRenderer.sortingOrder = 30;
        }

        private void InitializeText(string text, Color color, float characterSize, int fontSize)
        {
            ActivePopups.Add(this);

            var textRenderer = gameObject.AddComponent<MeshRenderer>();
            damageText = gameObject.AddComponent<TextMesh>();
            damageText.text = text;
            damageText.anchor = TextAnchor.MiddleCenter;
            damageText.alignment = TextAlignment.Center;
            damageText.fontSize = fontSize;
            damageText.characterSize = characterSize;
            damageText.color = color;
            textRenderer.sortingOrder = 31;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            transform.position += new Vector3(0f, floatSpeed * Time.deltaTime, 0f);

            if (elapsedTime >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            ActivePopups.Remove(this);
        }
    }
}
