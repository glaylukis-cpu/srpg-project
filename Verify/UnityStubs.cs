using System;
using System.Collections;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireComponent : Attribute
    {
        public RequireComponent(Type requiredComponent) { }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeField : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HeaderAttribute : Attribute
    {
        public HeaderAttribute(string header) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute
    {
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType) { }
    }

    public enum RuntimeInitializeLoadType
    {
        AfterSceneLoad
    }

    public class Object
    {
        public string name { get; set; }

        public static void Destroy(Object obj) { }
        public static T FindObjectOfType<T>() where T : Object => null;
        public static T FindAnyObjectByType<T>() where T : Object => null;
    }

    public class Component : Object
    {
        public GameObject gameObject { get; set; }
        public Transform transform { get; set; } = new Transform();

        public T GetComponent<T>() where T : Component, new() => new T();
    }

    public class MonoBehaviour : Component
    {
        public Coroutine StartCoroutine(IEnumerator routine) => new Coroutine();
        public void StopCoroutine(Coroutine routine) { }
    }

    public sealed class Coroutine { }

    public class GameObject : Object
    {
        public GameObject(string name)
        {
            this.name = name;
            transform = new Transform { gameObject = this };
        }

        public GameObject(string name, params Type[] components)
            : this(name)
        {
        }

        public Transform transform { get; }
        public string tag { get; set; }

        public void SetActive(bool value) { }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            component.gameObject = this;
            component.transform = transform;
            return component;
        }
    }

    public class Transform
    {
        public Vector3 position { get; set; }
        public Vector3 localPosition { get; set; }
        public Vector3 localScale { get; set; }
        public int childCount => 0;
        public GameObject gameObject { get; set; }

        public void SetParent(Transform parent) { }
        public void SetParent(Transform parent, bool worldPositionStays) { }
        public void SetAsFirstSibling() { }
        public Transform GetChild(int index) => new Transform { gameObject = new GameObject("Child") };
    }

    public class RectTransform : Transform
    {
        public Vector2 anchorMin { get; set; }
        public Vector2 anchorMax { get; set; }
        public Vector2 pivot { get; set; }
        public Vector2 anchoredPosition { get; set; }
        public Vector2 sizeDelta { get; set; }
    }

    public enum RenderMode
    {
        ScreenSpaceOverlay
    }

    public class Canvas : Component
    {
        public RenderMode renderMode { get; set; }
    }

    public class SpriteRenderer : Component
    {
        public Sprite sprite { get; set; }
        public Color color { get; set; }
        public int sortingOrder { get; set; }
    }

    public class MeshRenderer : Component
    {
        public int sortingOrder { get; set; }
    }

    public class TextMesh : Component
    {
        public string text { get; set; }
        public int fontSize { get; set; }
        public float characterSize { get; set; }
        public TextAnchor anchor { get; set; }
        public TextAlignment alignment { get; set; }
        public Color color { get; set; }
    }

    public enum TextAlignment
    {
        Center
    }

    public enum FullScreenMode
    {
        ExclusiveFullScreen,
        FullScreenWindow,
        MaximizedWindow,
        Windowed
    }

    public class BoxCollider2D : Component
    {
        public Vector2 size { get; set; }
    }

    public class CircleCollider2D : Component
    {
        public float radius { get; set; }
    }

    public class Camera : Component
    {
        public static Camera main { get; set; }
        public bool orthographic { get; set; }
        public float orthographicSize { get; set; }
        public Color backgroundColor { get; set; }
    }

    public class Font : Object { }

    public class AudioClip : Object
    {
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream)
        {
            return new AudioClip { name = name };
        }

        public bool SetData(float[] data, int offsetSamples) => true;
    }

    public class AudioSource : Component
    {
        public AudioClip clip { get; set; }
        public bool loop { get; set; }
        public bool playOnAwake { get; set; }
        public float volume { get; set; }
        public bool isPlaying { get; set; }

        public void Play()
        {
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public void PlayOneShot(AudioClip clip, float volumeScale) { }
    }

    public static class Resources
    {
        public static T GetBuiltinResource<T>(string path) where T : Object, new() => new T();
        public static T Load<T>(string path) where T : Object => null;
    }

    public enum TextAnchor
    {
        UpperLeft,
        UpperRight,
        LowerLeft,
        MiddleLeft,
        MiddleCenter
    }

    public class Sprite
    {
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit) => new Sprite();
    }

    public class Texture2D : Object
    {
        public Texture2D(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public int width { get; }
        public int height { get; }
        public FilterMode filterMode { get; set; }
        public void SetPixel(int x, int y, Color color) { }
        public void Apply() { }
    }

    public enum FilterMode
    {
        Point
    }

    public sealed class WaitForSeconds
    {
        public WaitForSeconds(float seconds) { }
    }

    public readonly struct Rect
    {
        public Rect(float x, float y, float width, float height) { }
    }

    public struct Color
    {
        public Color(float r, float g, float b, float a) { }
        public static Color white => new Color();
        public static Color blue => new Color();
        public static Color yellow => new Color();
    }

    public struct Vector2
    {
        public static Vector2 one => new Vector2();
        public static Vector2 zero => new Vector2();
        public Vector2(float x, float y) { }
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int up => new Vector2Int(0, 1);
        public static Vector2Int right => new Vector2Int(1, 0);
        public static Vector2Int down => new Vector2Int(0, -1);
        public static Vector2Int left => new Vector2Int(-1, 0);
        public static Vector2Int operator +(Vector2Int left, Vector2Int right) => new Vector2Int(left.x + right.x, left.y + right.y);
    }

    public struct Vector3
    {
        public Vector3(float x, float y, float z) { }
        public static Vector3 one => new Vector3();
        public static Vector3 operator *(Vector3 value, float scale) => new Vector3();
        public static Vector3 operator +(Vector3 left, Vector3 right) => new Vector3();
    }

    public static class Time
    {
        public static float deltaTime => 0.016f;
    }

    public static class Debug
    {
        public static void Log(object message) { }
        public static void LogWarning(object message) { }
    }

    public enum KeyCode
    {
        Space,
        Return,
        W,
        U,
        R,
        S,
        M,
        UpArrow,
        DownArrow,
        LeftArrow,
        RightArrow,
        Alpha1,
        Alpha2,
        Alpha3,
        Alpha4,
        Alpha5,
        Alpha6,
        Backspace,
        Escape
    }

    public static class Input
    {
        public static bool GetKeyDown(KeyCode key) => false;
    }

    public static class Screen
    {
        public static int width { get; private set; } = 1600;
        public static int height { get; private set; } = 900;
        public static bool fullScreen { get; private set; }
        public static FullScreenMode fullScreenMode { get; private set; } = FullScreenMode.Windowed;

        public static void SetResolution(int width, int height, bool fullscreen)
        {
            Screen.width = width;
            Screen.height = height;
            fullScreen = fullscreen;
            fullScreenMode = fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        }

        public static void SetResolution(int width, int height, FullScreenMode fullscreenMode)
        {
            Screen.width = width;
            Screen.height = height;
            Screen.fullScreenMode = fullscreenMode;
            fullScreen = fullscreenMode != FullScreenMode.Windowed;
        }
    }

    public static class Mathf
    {
        public static int Abs(int value) => Math.Abs(value);
        public static int Max(int a, int b) => Math.Max(a, b);
        public static int Min(int a, int b) => Math.Min(a, b);
        public static float Sin(float value) => (float)Math.Sin(value);
        public static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
        public static int RoundToInt(float value) => (int)Math.Round(value);
    }

    public static class Application
    {
        public static void Quit() { }
    }
}

namespace UnityEngine.UI
{
    using UnityEngine;

    public class Text : Component
    {
        public string text { get; set; }
        public Font font { get; set; }
        public int fontSize { get; set; }
        public TextAnchor alignment { get; set; }
        public Color color { get; set; }
        public bool raycastTarget { get; set; }
        public bool enabled { get; set; } = true;
        public RectTransform rectTransform { get; } = new RectTransform();
    }

    public class Image : Component
    {
        public Sprite sprite { get; set; }
        public Color color { get; set; }
        public bool raycastTarget { get; set; }
        public bool preserveAspect { get; set; }
        public bool enabled { get; set; } = true;
        public RectTransform rectTransform { get; } = new RectTransform();
    }

    public class CanvasScaler : Component
    {
        public enum ScaleMode
        {
            ScaleWithScreenSize
        }

        public ScaleMode uiScaleMode { get; set; }
        public Vector2 referenceResolution { get; set; }
    }

    public class AspectRatioFitter : Component
    {
        public enum AspectMode
        {
            None,
            WidthControlsHeight,
            HeightControlsWidth,
            FitInParent,
            EnvelopeParent
        }

        public AspectMode aspectMode { get; set; }
        public float aspectRatio { get; set; }
    }

    public class GraphicRaycaster : Component { }
}
