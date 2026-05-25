# Game Concept and Design — Lost in the Depths

## 1. Game Concept

**Lost in the Depths** is a short 2D side-scrolling underwater platformer built in Unity. The player controls a small fish trapped inside a dark underwater cave. To escape, they must collect all scattered glowing pearls while evading predator fish, then swim through the activated exit portal.

The game is designed to be completed in roughly two to four minutes. The appeal lies not in length but in feel: the buoyant, floaty swimming physics make even simple movement satisfying, and the dimly lit cave rewards careful exploration over frantic reaction.

**Core loop:** Swim → collect pearl → evade predator → unlock portal → escape.

**Player goal:** Collect all pearls as quickly as possible, avoid dying to predators, and exit through the portal.

---

## 2. Game Design Principles

### 2.1 Core Mechanic

The central mechanic is momentum-based swimming. The player holds a direction key to accelerate through water, then releases to drift. This single mechanic creates expressive movement: skilled players execute tight turns and speed runs; cautious players glide carefully around enemies. The mechanic is easy to learn but rewards mastery.

**Why this works:** A single well-tuned mechanic with clear feedback is more compelling than multiple shallow systems. The floaty physics are intrinsically satisfying, meaning the player enjoys moving even when not progressing — a strong foundation for a short game.

### 2.2 Tension and Pacing

The pearls are distributed across different sections of the cave, drawing the player deeper before they can escape. Predator fish patrol key routes, creating natural choke points. The win condition (portal only activates after all pearls are collected) ensures the player cannot rush straight to the exit, forcing them to explore and engage with the risk.

**Pacing principle:** Tension builds as the player commits deeper into the cave. The return journey to the portal, carrying a full score, gives a satisfying release — a classic there-and-back arc.

### 2.3 Feedback

- **Pearls:** Play a brief particle burst and audio chime on collection; score counter increments visibly.
- **Portal activation:** A distinct visual and audio cue fires when the final pearl is collected, signalling that the exit is ready.
- **Death:** Screen flash and scene reload — fast, no long penalty screen. Keeps the player in the loop.
- **Win:** Transition to a results screen showing final score and time taken.

Good feedback makes the game readable without tutorials or text instructions.

### 2.4 Level Design

One level. The cave is structured as a loose figure-eight so the player crosses the same central space twice, creating familiarity with a fresh angle on the second pass. Pearl placement is intentional:

- Six pearls in open, safe corridors (easy, teaches the collection mechanic).
- Six pearls in patrolled areas (requires timing around enemies).
- Two or three pearls in narrow dead-ends (optional risk for players who want a high score but can be skipped and retried).

This graduated placement means the difficulty curve is built into geography rather than explicit difficulty modes.

---

## 3. Creativity and Originality

The underwater cave setting is a deliberate inversion of the conventional platformer. There is no gravity to manage in the usual sense; instead, buoyancy and drag replace it. The player's main spatial challenge is controlling momentum in three dimensions of movement (up, down, left, right) rather than the binary jump-or-fall of a land platformer.

The glowing-pearl aesthetic connects the collectibles to the environment (bioluminescence is a real cave-ocean phenomenon) and doubles as environmental lighting. The pearls literally illuminate the darkness around them, making collected areas visually darker and subtly marking explored space — a simple but organic way to convey progress without a minimap.

---

## 4. Scope

The game is intentionally small. Deliverables are:

| Item | Description |
|---|---|
| 1 playable scene | Underwater cave with tilemap-based geometry |
| Player controller | Rigidbody2D swimming with drag and directional force |
| Pearl prefab | Trigger-based collectible with particle effect and audio |
| Predator prefab | Left-right patrol with Lerp; kills player on contact |
| Exit portal | Inactive until all pearls collected; loads win scene on enter |
| UI | Score counter, win screen with score and time |
| Audio | Background ambient loop, pearl chime, death sound, win jingle |

**What is out of scope:** multiple levels, save systems, upgrades, boss fights, cutscenes, mobile controls. These would be extensions and are not planned.

This scope is achievable in the time available because each system is independent and can be built and tested one at a time.

---

## 5. Tools, Assets, and Resources

### Engine

**Unity 2022 LTS** with the 2D template. Chosen because the coursework started in Unity, all starter code uses Unity's Rigidbody2D and tilemap systems, and the 2D physics and tilemap tools are well-suited to this genre.

### Art Assets

All art will be sourced from free, openly licensed asset packs:

- **Tiles and background:** [Kenney.nl](https://kenney.nl) underwater tile sets (CC0 public domain — no attribution required, but attribution will be given anyway in credits).
- **Player and enemy sprites:** Simple hand-drawn sprites using Aseprite (free/open source) or sourced from itch.io under CC0 or CC-BY licences.
- **Particles:** Unity's built-in particle system — no external asset needed.

All assets will be listed in a `CREDITS.md` file in the project, noting the source and licence for each.

### Audio

- Background ambient loop: free underwater ambient audio from [freesound.org](https://freesound.org) (CC0).
- Sound effects: generated using [sfxr/bfxr](https://www.bfxr.net/) (free, browser-based tool that produces royalty-free chiptune effects). No licensing complications.

### Code

All code will be written from scratch in C#. No third-party code libraries beyond Unity's standard packages (Rigidbody2D, Cinemachine for camera follow, TextMeshPro for UI).

---

## 6. Legal, Ethical, Social, and Accessibility Considerations

### Legal

- All external assets will be verified to carry a CC0, CC-BY, or similarly permissive licence before use.
- Unity Personal licence is free for projects below the revenue threshold; this is an educational project and qualifies.
- No trademarked names, logos, or characters will appear in the game.

### Ethical and Social

- The game contains no violence (death is a scene reload, no blood or explicit imagery).
- No user data is collected. The game runs entirely offline with no network calls.
- No monetisation, loot boxes, or manipulative design patterns.

### Accessibility

- **Controls:** Movement uses WASD or arrow keys. Both will be supported simultaneously.
- **Colour:** Pearls are both bright-white glowing objects and accompanied by a distinct audio chime, so collection feedback does not rely on colour alone. This supports players with colour vision deficiency.
- **Text size:** UI text will use TextMeshPro at a minimum effective size of 24pt at 1080p.
- **Motion sensitivity:** The camera follows the player with a small lag to avoid sudden lurching movements. This reduces potential discomfort for players sensitive to rapid camera motion.
- **Difficulty:** The game has no time limit on collecting pearls; only predator contact causes failure. A cautious player can take as long as they need.

### Security

This is a standalone desktop game with no networked features, no user accounts, and no data storage. No security risks apply.

---

## 7. Development Plan

The following plan assumes a four-week development window. Each week targets one vertical slice of the game, so a playable build exists at the end of every week.

### Week 1 — Core Movement and Scene

- Set up Unity project with 2D template.
- Build cave tilemap from Kenney tiles (rough layout, exact dressing can wait).
- Implement player swimming controller (Rigidbody2D, directional force, drag).
- Add Cinemachine camera that follows the player with soft zone.
- **Milestone:** Player can swim around the cave. Movement feels satisfying.

### Week 2 — Collectibles, Enemies, and State

- Place 12 pearl prefabs with trigger-based collection and score UI.
- Implement predator patrol (Lerp between two waypoints, back-and-forth).
- Add player-predator collision → death state → scene reload.
- **Milestone:** Full game loop is functional. Pearl collection, death, and respawn all work.

### Week 3 — Win Condition, Polish, and Audio

- Implement portal: deactivated, activates when pearl count reaches total, loads win scene on entry.
- Win scene: display score and time.
- Add all sound effects and ambient audio.
- Add particle effects on pearl collection.
- **Milestone:** The game can be won and lost with audio and visual feedback throughout.

### Week 4 — Level Polish and Testing

- Refine cave tilemap geometry: add detail, vary tunnel widths, ensure no dead geometry.
- Tune predator patrol speeds and positions for intended difficulty curve.
- Verify pearl placement matches the graduated difficulty described in section 2.4.
- Play-test with at least two other people; adjust anything that causes repeated frustration at the wrong moment.
- Final build and submission.
- **Milestone:** One polished, shippable level.

---

## 8. Summary

Lost in the Depths is a tight, self-contained 2D platformer built around one satisfying mechanic. Its scope is small enough to deliver in full, its tools are free and appropriate, its assets are legally clear, and its design is grounded in specific decisions rather than vague ambition. The development plan produces a working build at the end of each week, making it easy to identify and respond to problems before the deadline.
