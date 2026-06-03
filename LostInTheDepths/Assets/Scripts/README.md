# Scripts

Gameplay code for **Lost in the Depths**, a 2D underwater pearl-collecting game.
Everything is assembled from code at runtime — the only thing the scene file
needs is one `GameBootstrap` component. There are no prefabs to wire up and no
manual references to drag in the Inspector.

## Startup flow

```
GameScene
  └── GameBootstrap (DefaultExecutionOrder -100)
        ├── CaveBuilder ............ generates terrain, exposes spawn queries
        ├── UnderwaterAmbience ..... water backdrop + drifting bubbles
        ├── GameManager ............ score state + HUD
        ├── Player ................. SpriteRenderer + Rigidbody2D + PlayerController
        ├── Decorations ............ rocks / seagrass on the seabed
        ├── Pearls ................. collectibles (Pearl)
        ├── Predators .............. patrolling sharks (Predator)
        └── CameraRig .............. Cinemachine follow camera
```

`GameBootstrap` builds the `CaveBuilder` first, then asks it where the player
can spawn and where pearls and predators should go. Each spawned object is
self-contained: it builds its own visuals, colliders and behaviour in `Awake`
/`Start`, so the bootstrap only has to create the `GameObject` and add the
component.

## Files

### `GameBootstrap.cs`
The single scene entry point. Runs before everything else and wires the whole
game together: builds the cave and ambience, creates the `GameManager`, spawns
the player, seabed decorations, pearls and predators, then attaches the follow
camera.

- **Tunables (Inspector):** `playerColor`, `playerRadius`, `pearlCount`,
  `maxPredators`, `predatorSpeed`, `decorationDensity`.
- **Key logic:** `SpreadSelect` does greedy farthest-point sampling so pearls
  spread evenly instead of clustering; predator lanes are chosen longest-first
  and kept clear of the player's spawn and of each other.

### `CaveBuilder.cs`
Procedurally builds the playfield from a fixed `seed` (reproducible between
runs): a single open body of water bounded by an irregular rocky floor, a thin
ceiling, side walls, and a few reef mounds. Adjacent wall cells in a row are
merged into one stretched box + collider to keep object counts low.

- **State:** `Size`, `SpawnPoint`.
- **Queries used by the bootstrap:**
  - `CellToWorld(col, y)` — grid → world position.
  - `IsOpen(col, y)` — is a cell open water?
  - `OpenCells()` — all open cells (pearl candidates).
  - `FloorSurfaceCells()` — open cells sitting on rock (decoration anchors).
  - `HorizontalCorridors(minLen)` — runs of open cells as `(a, b)` endpoints
    (shark patrol lanes).

### `PlayerController.cs`
Drives the player fish. Reads WASD / arrow input each frame and applies a thrust
force to the `Rigidbody2D` in `FixedUpdate`, capped at `topSpeed`. Gravity is off
and water drag makes the fish coast to a stop, giving a swim-like feel. Flips the
sprite to face the travel direction.

- **Tunables:** `thrust`, `topSpeed`, `waterDrag`.

### `Predator.cs`
A shark enemy with a small state machine — **Patrol → Chase → Return**. It
ping-pongs along its assigned lane until the player enters `detectRange`, then
lunges at `speed * chaseMultiplier`; once the player passes `loseRange`
(hysteresis so it doesn't flicker) it returns to the nearest lane point and
resumes patrolling. Contact with the player triggers `GameManager.PlayerDied()`.

- **Setup:** `Configure(a, b, speed)` is called by the bootstrap with the lane
  endpoints and patrol speed.
- **Tunables:** `chaseMultiplier`, `detectRange`, `loseRange`, `length`, `radius`.

### `Pearl.cs`
A glowing collectible. Builds its own pulsing glow halo and pearl sprite plus a
trigger collider, registers itself with the `GameManager` on spawn
(`RegisterPearl`), and reports collection (`CollectPearl`) when the player swims
into it before destroying itself.

- **Tunables:** `glowColor`, `glowRadius`, `pearlSize`, `coreRadius`,
  `pickupRadius`.

### `GameManager.cs`
Singleton (`GameManager.Instance`) holding the run-time game state. Tracks the
pearl `Total` / `Collected` (`AllCollected` is true once every pearl is taken),
owns the on-screen score HUD, and restarts the level on `PlayerDied()`.

- **Interface:** `RegisterPearl()`, `CollectPearl()`, `PlayerDied()`,
  properties `Total`, `Collected`, `AllCollected`.

### `CameraRig.cs`
Wraps a Cinemachine orthographic virtual camera that smoothly follows the
player. `Attach(mainCamera, target)` ensures the main camera has a
`CinemachineBrain`, then configures the framing. The dead/soft zones let the fish
drift near screen centre without the camera reacting, so the view never jitters.

- **Tunables:** `orthoSize`, dead/soft-zone sizes, `xDamping`, `yDamping`.

### `UnderwaterAmbience.cs`
Cosmetic mood: a deep-water gradient backdrop sized to the cave bounds plus a
field of bubbles that rise, sway and loop back to the bottom. `Build(origin,
gridSize)` is called once by the bootstrap.

- **Tunables:** `bubbleCount`, `riseSpeedRange`, `sizeRange`, `bubbleAlpha`.

### `GameArt.cs`
Static loader/cache for the imported sprites under `Resources/Art` (see
[`../../../CREDITS.md`](../../../CREDITS.md)). `Load(name)` returns a cached
sprite; `Apply(sr, name, worldSize)` assigns it to a renderer and scales the
transform so the sprite's longest side spans `worldSize` world units, returning
`null` if the art is missing so callers can fall back to a procedural shape.

- **Sprite name constants:** `Fish`, `Shark`, `Pearl`, `Bubble`, `Water`,
  `Rock`, `Seagrass`.

### `RuntimeSprites.cs`
Generates simple procedural sprites at runtime (no asset import needed):
`Circle(size, fill)` for solid actors / fallbacks and `Glow(size, core)` for the
soft halo behind each pearl. All sprites are authored at 1 world unit so callers
size them purely through transform scale.

## Sorting orders

Layering is set by `SpriteRenderer.sortingOrder` (higher = nearer the camera):

| Order | Layer                                   |
|------:|-----------------------------------------|
|  -10  | Cave flat water fill (`CaveBuilder`)    |
|   -9  | Water gradient backdrop (`UnderwaterAmbience`) |
|    0  | Cave walls                              |
|    1  | Seabed decorations (rock / seagrass)    |
|    2  | Bubbles                                 |
|    3  | Pearl glow halo                         |
|    4  | Pearl core / shark                      |
|    5  | Player fish                             |
