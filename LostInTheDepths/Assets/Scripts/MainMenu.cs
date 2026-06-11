using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The game's entry screen, built from code in the same runtime-assembled style as
/// <see cref="WinScreen"/>. It shows the game title, a one-line tagline and a clickable
/// <b>Start</b> button; clicking the button (or pressing Enter/Space) loads
/// <c>GameScene</c> and begins play. The StartScene holds a single GameObject carrying
/// this component, plus a camera with an audio listener. This scene is first in the
/// build order, so it is what the player sees when the game launches.
/// </summary>
public class MainMenu : MonoBehaviour
{
    Image startButton;
    float pulseClock;

    void Start()
    {
        BuildUi();
    }

    void Update()
    {
        // Gently pulse the button so the entry screen feels alive; Enter or Space
        // start the game as a keyboard shortcut alongside the click.
        pulseClock += Time.deltaTime;
        if (startButton != null)
        {
            float glow = 0.85f + 0.15f * Mathf.Sin(pulseClock * 3f);
            startButton.color = new Color(0.15f * glow, 0.55f * glow, 0.7f * glow, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(KeyCode.Space))
            StartGame();
    }

    /// <summary>Loads the playable level. Wired to the Start button's click event.</summary>
    void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void BuildUi()
    {
        var canvasGo = new GameObject("MenuCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();
        // A clickable UI needs an EventSystem to route pointer input to the button.
        EnsureEventSystem();

        MakeText(canvasGo.transform, "Title", "Lost in the Depths", 110, FontStyle.Bold,
                 new Color(0.7f, 0.97f, 1f), new Vector2(0, 230));
        MakeText(canvasGo.transform, "Tagline",
                 "Collect every pearl, dodge the sharks, and escape through the portal.",
                 40, FontStyle.Italic, new Color(0.8f, 0.9f, 1f), new Vector2(0, 110));

        MakeButton(canvasGo.transform, "Start", new Vector2(0, -70), StartGame);

        MakeText(canvasGo.transform, "Hint", "Click Start  ·  or press Enter / Space",
                 32, FontStyle.Normal, new Color(0.6f, 0.72f, 0.82f), new Vector2(0, -230));
    }

    /// <summary>
    /// Builds a labelled, clickable button and registers <paramref name="onClick"/>.
    /// Keeps a reference to the background image so <see cref="Update"/> can pulse it.
    /// </summary>
    void MakeButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject("StartButton");
        go.transform.SetParent(parent, false);

        startButton = go.AddComponent<Image>();
        startButton.color = new Color(0.15f, 0.55f, 0.7f, 1f);
        var rt = startButton.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(340f, 110f);

        var button = go.AddComponent<Button>();
        button.targetGraphic = startButton;
        button.onClick.AddListener(onClick);

        var txt = MakeText(go.transform, "Label", label, 56, FontStyle.Bold, Color.white,
                           Vector2.zero);
        txt.rectTransform.sizeDelta = rt.sizeDelta;
    }

    Text MakeText(Transform parent, string name, string content, int size,
                  FontStyle style, Color color, Vector2 pos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var txt = go.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text = content;
        txt.fontSize = size;
        txt.fontStyle = style;
        txt.color = color;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        var rt = txt.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(1400f, size + 30f);
        return txt;
    }

    /// <summary>
    /// Ensures the scene has an <see cref="EventSystem"/> so the Start button can
    /// receive clicks; the StartScene ships without one to keep the asset minimal.
    /// </summary>
    static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<UnityEngine.EventSystems.EventSystem>();
        go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }
}
