using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Holds the run-time game state: how many pearls exist, how many the player
/// has collected, and what happens on death. Owns the on-screen score readout.
/// One instance is created per scene by <see cref="GameBootstrap"/>.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Total { get; private set; }
    public int Collected { get; private set; }
    public bool AllCollected => Total > 0 && Collected >= Total;

    Text scoreLabel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BuildHud();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Called once by each pearl as it spawns so the total is known.</summary>
    public void RegisterPearl()
    {
        Total++;
        UpdateLabel();
    }

    /// <summary>Called when the player swims into a pearl.</summary>
    public void CollectPearl()
    {
        Collected = Mathf.Min(Collected + 1, Total);
        UpdateLabel();
        // Week 3: when AllCollected becomes true, activate the exit portal here.
    }

    /// <summary>Predator contact: restart the level. Fast loop, no penalty screen.</summary>
    public void PlayerDied()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void BuildHud()
    {
        var canvasGo = new GameObject("HUD");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGo.AddComponent<GraphicRaycaster>();

        var labelGo = new GameObject("ScoreLabel");
        labelGo.transform.SetParent(canvasGo.transform, false);

        scoreLabel = labelGo.AddComponent<Text>();
        scoreLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreLabel.fontSize = 40; // ~36pt effective at 1080p, above the 24pt floor
        scoreLabel.fontStyle = FontStyle.Bold;
        scoreLabel.color = new Color(0.85f, 0.95f, 1f, 1f);
        scoreLabel.alignment = TextAnchor.UpperLeft;
        scoreLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
        scoreLabel.verticalOverflow = VerticalWrapMode.Overflow;

        var rt = scoreLabel.rectTransform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(28f, -22f);
        rt.sizeDelta = new Vector2(480f, 70f);

        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (scoreLabel != null)
            scoreLabel.text = $"Pearls  {Collected} / {Total}";
    }
}
