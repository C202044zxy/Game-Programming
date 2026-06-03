using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameBootstrap : MonoBehaviour
{
    [Header("Player Look")]
    public Color playerColor = new Color(1f, 0.78f, 0.32f, 1f);
    public float playerRadius = 0.35f;

    [Header("Spawning")]
    public int pearlCount = 12;
    public int maxPredators = 3;
    public float predatorSpeed = 1.8f;

    void Start()
    {
        var cave = new GameObject("Cave").AddComponent<CaveBuilder>();
        cave.transform.SetParent(transform, false);

        var ambience = new GameObject("Ambience").AddComponent<UnderwaterAmbience>();
        ambience.transform.SetParent(transform, false);
        ambience.Build(cave.originOffset, cave.Size);

        var manager = new GameObject("GameManager");
        manager.transform.SetParent(transform, false);
        manager.AddComponent<GameManager>();

        var player = SpawnPlayer(cave.SpawnPoint);

        SpawnPearls(cave, player.transform.position);
        SpawnPredators(cave, player.transform.position);

        var rigGo = new GameObject("CameraRig");
        rigGo.transform.SetParent(transform, false);
        var rig = rigGo.AddComponent<CameraRig>();
        rigGo.transform.position = new Vector3(player.transform.position.x,
                                                player.transform.position.y,
                                                -10f);
        rig.Attach(Camera.main, player.transform);
    }

    GameObject SpawnPlayer(Vector2 spawn)
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(spawn.x, spawn.y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;
        if (GameArt.Apply(sr, GameArt.Fish, playerRadius * 2f) == null)
        {
            // Fall back to the procedural circle if the fish art is missing.
            sr.sprite = RuntimeSprites.Circle(64, playerColor);
            go.transform.localScale = new Vector3(playerRadius * 2f, playerRadius * 2f, 1f);
        }

        var body = go.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Keep the world-space collision radius at playerRadius regardless of the
        // scale the sprite sizing chose above.
        var col = go.AddComponent<CircleCollider2D>();
        col.radius = playerRadius / Mathf.Max(0.0001f, go.transform.localScale.x);

        go.AddComponent<PlayerController>();
        return go;
    }

    void SpawnPearls(CaveBuilder cave, Vector2 playerPos)
    {
        var candidates = new List<Vector2>();
        foreach (var cell in cave.OpenCells())
            if (Vector2.Distance(cell, playerPos) > 2.5f) // keep spawn area clear
                candidates.Add(cell);

        var group = new GameObject("Pearls");
        group.transform.SetParent(transform, false);

        foreach (var pos in SpreadSelect(candidates, playerPos, pearlCount))
        {
            var go = new GameObject("Pearl");
            go.transform.SetParent(group.transform, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            go.AddComponent<Pearl>();
        }
    }

    void SpawnPredators(CaveBuilder cave, Vector2 playerPos)
    {
        var corridors = cave.HorizontalCorridors(6);
        // Longest lanes first so patrols feel substantial.
        corridors.Sort((x, y) =>
            Vector2.Distance(y.a, y.b).CompareTo(Vector2.Distance(x.a, x.b)));

        var chosen = new List<(Vector2 a, Vector2 b)>();
        foreach (var lane in corridors)
        {
            if (chosen.Count >= maxPredators) break;
            // Never start a predator on top of the player's spawn corridor.
            if (DistanceToSegment(playerPos, lane.a, lane.b) < 4f) continue;

            Vector2 mid = (lane.a + lane.b) * 0.5f;
            bool tooClose = false;
            foreach (var picked in chosen)
                if (Vector2.Distance((picked.a + picked.b) * 0.5f, mid) < 4f) { tooClose = true; break; }
            if (tooClose) continue;

            chosen.Add(lane);
        }

        var group = new GameObject("Predators");
        group.transform.SetParent(transform, false);

        for (int i = 0; i < chosen.Count; i++)
        {
            var go = new GameObject($"Predator_{i}");
            go.transform.SetParent(group.transform, false);
            var pred = go.AddComponent<Predator>();
            pred.Configure(chosen[i].a, chosen[i].b, predatorSpeed + 0.3f * i);
        }
    }

    /// <summary>
    /// Greedy farthest-point sampling: repeatedly pick the candidate that is
    /// furthest from everything already chosen, giving an even spread across the
    /// cave without random clustering.
    /// </summary>
    static List<Vector2> SpreadSelect(List<Vector2> candidates, Vector2 seed, int count)
    {
        var chosen = new List<Vector2>();
        if (candidates.Count == 0) return chosen;

        var pool = new List<Vector2>(candidates);

        // First pick: the candidate furthest from the player spawn.
        int bestIdx = 0;
        float bestScore = -1f;
        for (int i = 0; i < pool.Count; i++)
        {
            float d = Vector2.Distance(pool[i], seed);
            if (d > bestScore) { bestScore = d; bestIdx = i; }
        }
        chosen.Add(pool[bestIdx]);
        pool.RemoveAt(bestIdx);

        while (chosen.Count < count && pool.Count > 0)
        {
            bestIdx = 0;
            bestScore = -1f;
            for (int i = 0; i < pool.Count; i++)
            {
                float nearest = float.MaxValue;
                foreach (var c in chosen)
                    nearest = Mathf.Min(nearest, Vector2.Distance(pool[i], c));
                if (nearest > bestScore) { bestScore = nearest; bestIdx = i; }
            }
            chosen.Add(pool[bestIdx]);
            pool.RemoveAt(bestIdx);
        }
        return chosen;
    }

    static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / Mathf.Max(0.0001f, ab.sqrMagnitude));
        return Vector2.Distance(p, a + ab * t);
    }
}
