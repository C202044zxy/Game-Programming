# Change Log — 2D Space Shooter Improvement

**Student:** C202044zxy  
**Course:** Game Programming  
**Date:** 2026-05-11

---

## What the game did before

The starter package provided a fully functional 2D top-down space shooter with:
- Player movement (WASD / free-roam, aims toward mouse cursor)
- Three enemy types: Chaser, Straight Shooter, Diagonal Shooter, and their spawners
- A health/damage system with invincibility frames
- Score tracking and a high-score stored in PlayerPrefs
- UI pages for Game Over and Victory using `UIManager`
- A main-menu prefab and audio tracks

The game lacked: instructions visible to the player, a populated HUD beyond score, any power-ups or difficulty ramp, and feedback text for objectives.

---

## What I changed and why

### 1. HUD — Health and Lives Display (UI update)

**Files added:** `Assets/Scripts/UI/HealthDisplay.cs`, `Assets/Scripts/UI/LivesDisplay.cs`

**What:** Two new `UIelement` subclasses that hook into the existing `UIManager.UpdateUI()` pipeline. `HealthDisplay` reads `Health.currentHealth` / `Health.maximumHealth` from the player and prints `HP: X / Y`. `LivesDisplay` reads `Health.currentLives` and prints `Lives: N` (hidden if the player is not using the lives system).

**Why:** The starter HUD only showed score. A player dying unexpectedly is frustrating; seeing remaining health lets them make informed decisions (e.g. back off, collect a health power-up).

**Tested:** Values update correctly on player hit and health restore. Display disappears cleanly on game over.

---

### 2. Power-ups — Gameplay Change

**Files added:** `Assets/Scripts/PowerUps/PowerUp.cs`, `Assets/Scripts/PowerUps/PowerUpSpawner.cs`

**What:** Three pick-up types (enumerated in `PowerUp.PowerUpType`):
- **SpeedBoost** — multiplies `Controller.moveSpeed` for a configurable duration, then restores it.
- **FireRateBoost** — divides all player `ShootingController.fireRate` values (lower = faster fire), then restores them.
- **HealthRestore** — calls `Health.ReceiveHealing()` for an instant flat heal.

A `PowerUpSpawner` (placed in the scene on a manager object) spawns a random power-up prefab at a random position within a configurable rectangle, on a random interval between `minSpawnInterval` and `maxSpawnInterval`. Spawned items self-destroy after `powerUpLifetime` seconds if uncollected.

**Why:** Pick-ups are a classic way to reward active movement and risk-taking. The coroutine restore pattern ensures power-up effects are always temporary and stack correctly if multiple fire before the first expires.

**Tested:** All three types picked up and effect confirmed; duration expiry restores original values; items despawn when uncollected.

---

### 3. Difficulty Scaling — Gameplay Change

**Files added:** `Assets/Scripts/Gameplay/DifficultyScaler.cs`

**What:** `DifficultyScaler` monitors `GameManager.score` every frame and applies a `Threshold` when the score crosses a defined value. Applying a threshold reduces `EnemySpawner.spawnDelay` by a multiplier (making enemies spawn faster) and multiplies `Enemy.moveSpeed` on every active enemy. Three tiers are pre-configured (50 / 100 / 200 points), each applied exactly once.

**Why:** An infinite scrolling shooter with a constant spawn rate becomes trivial once the player learns the pattern. Scaling difficulty ensures later waves feel meaningfully harder without requiring new level design.

**Tested:** Verified via console log messages; enemy speed visibly increases at each tier; spawn intervals decrease.

---

### 4. Objective Text — Feedback / Polish Update

**Files added:** `Assets/Scripts/UI/ObjectiveText.cs`

**What:** A `MonoBehaviour` that writes a configurable `objectiveMessage` to a `TextMeshProUGUI` at scene load, holds it fully opaque for `displayDuration` seconds, then fades it out over `fadeDuration` seconds using a coroutine, and finally disables the GameObject. The default message tells the player the win condition and the core controls.

**Why:** Players dropped into a game without explanation often spend the first 20 seconds figuring out the controls. A brief on-screen instruction solves this without interrupting flow.

**Tested:** Text appears on play, stays readable for ~3.5 seconds, fades smoothly, and does not reappear if the scene is not reloaded.

---

## How to Open and Play

The entire project is pre-configured via `Assets/Editor/ProjectSetup.cs`, an `[InitializeOnLoad]` editor script that runs automatically the first time Unity compiles the project.

**Steps:**
1. Open Unity Hub → **Add project from disk** → select the `UnityProject/` folder.
2. Unity opens, compiles the scripts, and `ProjectSetup.cs` automatically:
   - Builds and saves `Assets/Scenes/SampleScene.unity` (full gameplay scene with player, enemies, HUD, power-ups, and difficulty scaler)
   - Builds and saves `Assets/Scenes/MainMenu.unity` (title screen with New Game / Instructions / Exit buttons)
   - Registers both scenes in Build Settings (MainMenu = index 0, SampleScene = index 1)
3. In the **Project** panel open `Assets/Scenes/MainMenu` and press **Play**.

To re-run the setup at any time: **Tools → Re-run Space Shooter Setup** from the Unity menu bar.

---

## Credits / Citations

- Starter code and all art, audio, animation, and prefab assets: provided class starter package (`2D_game_assets(1).unitypackage`), Dundee Game Programming module.
- Font: `manaspc.ttf` — license included in `Assets/Art/UI Elements/Fonts/manaspc/license.txt` (from the starter package).
- New scripts (`HealthDisplay`, `LivesDisplay`, `ObjectiveText`, `PowerUp`, `PowerUpSpawner`, `DifficultyScaler`): written from scratch for this assignment.
- Unity documentation referenced: Input System, TextMeshPro, Coroutines, Physics2D triggers.
