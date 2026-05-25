using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int totalPearls;
    int collectedPearls;
    float startTime;

    public int CollectedPearls => collectedPearls;
    public float ElapsedTime   => Time.time - startTime;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        totalPearls = FindObjectsByType<Pearl>(FindObjectsSortMode.None).Length;
        startTime   = Time.time;
    }

    public void OnPearlCollected()
    {
        collectedPearls++;
        if (totalPearls > 0 && collectedPearls >= totalPearls)
            ActivatePortal();
    }

    void ActivatePortal()
    {
        var portal = FindFirstObjectByType<Portal>();
        if (portal != null) portal.Activate();
    }

    public void PlayerDied() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void LoadWinScene()
    {
        PlayerPrefs.SetInt("FinalScore", collectedPearls);
        PlayerPrefs.SetFloat("FinalTime", ElapsedTime);
        PlayerPrefs.Save();

        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            if (SceneUtility.GetScenePathByBuildIndex(i).EndsWith("WinScene.unity"))
            {
                SceneManager.LoadScene(i);
                return;
            }
        }
        // WinScene not yet in build — restart current scene as fallback.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
