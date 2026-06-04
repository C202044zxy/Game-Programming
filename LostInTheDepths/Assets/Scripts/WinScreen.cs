using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Builds the win / results screen from code, in the same runtime-assembled style
/// as the in-game HUD. It reads the run's score and time from <see cref="GameManager"/>'s
/// static result fields — statics survive the scene change that brought us here — then
/// stops the ambient drone, plays the win jingle, and shows the totals. Pressing any
/// key dives back into a fresh run. The WinScene holds one GameObject carrying this
/// component, plus a camera with an audio listener.
/// </summary>
public class WinScreen : MonoBehaviour
{
    Text prompt;
    float blinkClock;

    void Start()
    {
        SoundFX.StopAmbience();
        SoundFX.PlayWin();
        BuildUi();
    }

    void Update()
    {
        // Pulse the restart prompt; any key returns to the cave for another run.
        blinkClock += Time.deltaTime;
        if (prompt != null)
        {
            var c = prompt.color;
            c.a = 0.5f + 0.5f * Mathf.Sin(blinkClock * 3f);
            prompt.color = c;
        }
        if (Input.anyKeyDown)
            SceneManager.LoadScene("GameScene");
    }

    void BuildUi()
    {
        var canvasGo = new GameObject("WinCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        float t = GameManager.ResultTime;
        int minutes = (int)(t / 60f);
        float seconds = t - minutes * 60f;
        string time = $"{minutes}:{seconds:00.0}";

        MakeText(canvasGo.transform, "Title", "You Escaped!", 96, FontStyle.Bold,
                 new Color(0.7f, 0.97f, 1f), new Vector2(0, 180));
        MakeText(canvasGo.transform, "Score",
                 $"Pearls collected   {GameManager.ResultCollected} / {GameManager.ResultTotal}",
                 52, FontStyle.Normal, Color.white, new Vector2(0, 40));
        MakeText(canvasGo.transform, "Time", $"Time   {time}",
                 52, FontStyle.Normal, Color.white, new Vector2(0, -30));
        prompt = MakeText(canvasGo.transform, "Prompt", "Press any key to dive back in",
                 40, FontStyle.Italic, new Color(0.8f, 0.9f, 1f), new Vector2(0, -180));
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
        rt.sizeDelta = new Vector2(1100f, size + 30f);
        return txt;
    }
}
