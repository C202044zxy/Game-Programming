using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scales difficulty as the player's score increases.
/// At each score threshold the spawn delay on all EnemySpawners is reduced
/// and all currently active enemies move faster.
///
/// Place this script on a persistent manager GameObject (e.g. the GameManager object).
/// Thresholds are evaluated once and only applied once so the effect is cumulative
/// across multiple tiers.
/// </summary>
public class DifficultyScaler : MonoBehaviour
{
    [System.Serializable]
    public class Threshold
    {
        [Tooltip("Score that triggers this tier")]
        public int score;
        [Tooltip("Multiplier applied to spawn delay (>1 = faster spawning). E.g. 1.3 = 30% faster.")]
        [Range(1f, 4f)] public float spawnRateMultiplier = 1.3f;
        [Tooltip("Multiplier applied to each active enemy's moveSpeed. E.g. 1.2 = 20% faster.")]
        [Range(1f, 3f)] public float enemySpeedMultiplier = 1.2f;

        [HideInInspector] public bool applied;
    }

    [Tooltip("Ordered list of score thresholds that raise difficulty")]
    public List<Threshold> thresholds = new List<Threshold>
    {
        new Threshold { score = 50,  spawnRateMultiplier = 1.3f, enemySpeedMultiplier = 1.2f },
        new Threshold { score = 100, spawnRateMultiplier = 1.5f, enemySpeedMultiplier = 1.35f },
        new Threshold { score = 200, spawnRateMultiplier = 2.0f, enemySpeedMultiplier = 1.5f }
    };

    private EnemySpawner[] spawners;

    private void Start()
    {
        spawners = FindObjectsOfType<EnemySpawner>();
    }

    private void Update()
    {
        foreach (Threshold t in thresholds)
        {
            if (!t.applied && GameManager.score >= t.score)
            {
                Apply(t);
                t.applied = true;
            }
        }
    }

    private void Apply(Threshold t)
    {
        // Refresh spawner list in case new ones were created at runtime
        spawners = FindObjectsOfType<EnemySpawner>();
        foreach (EnemySpawner s in spawners)
            s.spawnDelay = Mathf.Max(0.3f, s.spawnDelay / t.spawnRateMultiplier);

        foreach (Enemy e in FindObjectsOfType<Enemy>())
            e.moveSpeed *= t.enemySpeedMultiplier;

        Debug.Log($"[DifficultyScaler] Tier applied at score {t.score}: " +
                  $"spawn x{t.spawnRateMultiplier}, speed x{t.enemySpeedMultiplier}");
    }
}
