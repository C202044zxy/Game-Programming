using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int totalPearls;
    int collectedPearls;
    float startTime;

    public int CollectedPearls => collectedPearls;
    public float ElapsedTime => Time.time - startTime;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        totalPearls = FindObjectsByType<Pearl>(FindObjectsSortMode.None).Length;
        startTime = Time.time;
    }

    public void OnPearlCollected()
    {
        collectedPearls++;
        if (collectedPearls >= totalPearls)
            OnAllPearlsCollected();
    }

    void OnAllPearlsCollected()
    {
        Portal portal = FindFirstObjectByType<Portal>();
        if (portal != null) portal.Activate();
    }

    public void PlayerDied()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadWinScene()
    {
        PlayerPrefs.SetInt("FinalScore", collectedPearls);
        PlayerPrefs.SetFloat("FinalTime", ElapsedTime);
        SceneManager.LoadScene("WinScene");
    }
}
