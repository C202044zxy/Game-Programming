using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The single scene entry point. Runs before everything else
/// (<see cref="DefaultExecutionOrder"/> -100) and assembles the whole game from
/// code: it builds the cave, the ambience, the <see cref="GameManager"/>, spawns
/// the player, seabed decorations, pearls and predators, and finally wires up the
/// follow camera. Tunable spawn counts and look-and-feel values are exposed as
/// fields so the level can be adjusted without touching the other scripts.
/// </summary>
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
    public int blessingsPerType = 2; // speed + disguise blessings scattered like pearls

    [Header("Seabed Decoration")]
    [Range(0f, 1f)] public float decorationDensity = 0.4f;

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

        SpawnDecorations(cave, player.transform.position);
        SpawnPearls(cave, player.transform.position);
        SpawnBlessings(cave, player.transform.position);
        SpawnPredators(cave, player.transform.position);
        SpawnPortal(cave.SpawnPoint);

        var rigGo = new GameObject("CameraRig");
        rigGo.transform.SetParent(transform, false);
        var rig = rigGo.AddComponent<CameraRig>();
        rigGo.transform.position = new Vector3(player.transform.position.x,
                                                player.transform.position.y,
                                                -10f);
        rig.Attach(Camera.main, player.transform);
    }

    /// <summary>
    /// Creates the player fish at <paramref name="spawn"/>: a sprite, a dynamic
    /// gravity-free <see cref="Rigidbody2D"/>, a circle collider kept at the
    /// intended world radius, and the <see cref="PlayerController"/>.
    /// </summary>
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
        // Manages the temporary "Blessing of the Fish" powers (added after the
        // controller so its Awake can find it).
        go.AddComponent<BlessingEffects>();
        return go;
    }

    /// <summary>
    /// Plants rocks and seagrass on the seabed (floor surface and reef tops) so
    /// the scene reads as a real underwater environment. Each decoration rests
    /// its base on the rock below it and sits behind the gameplay actors.
    /// </summary>
    void SpawnDecorations(CaveBuilder cave, Vector2 playerPos)
    {
        var group = new GameObject("Decorations");
        group.transform.SetParent(transform, false);

        foreach (var cell in cave.FloorSurfaceCells())
        {
            if (Random.value > decorationDensity) continue;
            if (Vector2.Distance(cell, playerPos) < 2.5f) continue; // keep spawn clear

            bool grass = Random.value < 0.6f;
            string art = grass ? GameArt.Seagrass : GameArt.Rock;
            float worldSize = grass ? Random.Range(1.0f, 1.5f) : Random.Range(0.8f, 1.3f);

            var go = new GameObject(grass ? "Seagrass" : "Rock");
            go.transform.SetParent(group.transform, false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1; // in front of the rock mass, behind bubbles/pearls/fish
            if (GameArt.Apply(sr, art, worldSize) == null) continue;
            if (Random.value < 0.5f) sr.flipX = true;

            // Rest the decoration's base on the top surface of the floor cell.
            float floorTop = cell.y - 0.5f;
            go.transform.position = new Vector3(cell.x, floorTop + sr.bounds.extents.y, 0f);
        }
    }

    /// <summary>
    /// Scatters <see cref="pearlCount"/> pearls through the open water, keeping
    /// the spawn area clear and using <see cref="SpreadSelect"/> for an even
    /// distribution.
    /// </summary>
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

    /// <summary>
    /// Scatters <see cref="blessingsPerType"/> of each blessing type through the
    /// open water (away from the spawn), spread evenly like pearls. These are
    /// one-time power-ups and do not count toward the pearl total. Each object is
    /// created inactive so its <see cref="BlessingType"/> is set before
    /// <see cref="Blessing.Awake"/> builds its (type-coloured) visuals.
    /// </summary>
    void SpawnBlessings(CaveBuilder cave, Vector2 playerPos)
    {
        var candidates = new List<Vector2>();
        foreach (var cell in cave.OpenCells())
            if (Vector2.Distance(cell, playerPos) > 2.5f) // keep spawn area clear
                candidates.Add(cell);

        var group = new GameObject("Blessings");
        group.transform.SetParent(transform, false);

        var spots = SpreadSelect(candidates, playerPos, blessingsPerType * 2);
        for (int i = 0; i < spots.Count; i++)
        {
            var go = new GameObject("Blessing");
            go.SetActive(false);
            go.transform.SetParent(group.transform, false);
            go.transform.position = new Vector3(spots[i].x, spots[i].y, 0f);
            var blessing = go.AddComponent<Blessing>();
            blessing.type = (i % 2 == 0) ? BlessingType.Speed : BlessingType.Disguise;
            go.SetActive(true);
        }
    }

    /// <summary>
    /// Places up to <see cref="maxPredators"/> sharks on the longest horizontal
    /// corridors, skipping lanes too close to the player's spawn or to a shark
    /// already placed, then configures each one's patrol via <see cref="Predator.Configure"/>.
    /// </summary>
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
    /// Places the exit <see cref="Portal"/> at the player's spawn point. It starts
    /// dormant and opens itself once every pearl is collected, so the level forms a
    /// there-and-back loop: swim out to gather pearls, swim home to escape.
    /// </summary>
    void SpawnPortal(Vector2 spawn)
    {
        var go = new GameObject("Portal");
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector3(spawn.x, spawn.y, 0f);
        go.AddComponent<Portal>();
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
