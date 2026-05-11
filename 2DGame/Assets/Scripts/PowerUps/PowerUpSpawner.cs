using UnityEngine;

/// <summary>
/// Spawns power-up prefabs at random positions within a configurable area.
/// Assign 1-3 different PowerUp prefabs to powerUpPrefabs in the Inspector.
/// Each spawned power-up is automatically destroyed after powerUpLifetime seconds
/// so the arena doesn't fill up if the player ignores them.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    [Tooltip("Array of power-up prefabs to randomly choose from when spawning")]
    public GameObject[] powerUpPrefabs;

    [Tooltip("Minimum seconds between spawns")]
    public float minSpawnInterval = 8f;
    [Tooltip("Maximum seconds between spawns")]
    public float maxSpawnInterval = 15f;

    [Tooltip("Horizontal spawn range around the spawner's position")]
    public float spawnRangeX = 8f;
    [Tooltip("Vertical spawn range around the spawner's position")]
    public float spawnRangeY = 4f;

    [Tooltip("Seconds a power-up stays in the world before auto-destroying")]
    public float powerUpLifetime = 10f;

    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnPowerUp();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        if (prefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            0f);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        Destroy(obj, powerUpLifetime);
    }
}
