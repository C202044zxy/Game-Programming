# Credits

## Code

All gameplay scripts (`Assets/Scripts/*.cs`) are original work for this
coursework, written from scratch for the Week 1 milestone.

## Engine and Packages

- Unity 2022.3.62f1 (Personal licence) — Unity Technologies
- `com.unity.cinemachine` 2.9.7 — virtual camera follow
- `com.unity.textmeshpro` 3.0.6 — reserved for Week 3 UI
- `com.unity.2d.sprite`, `com.unity.2d.tilemap` — 2D feature set

## Art

Sprites live in `Assets/Resources/Art/` and are loaded at runtime by
`GameArt.cs`.

- **Fish Pack** by Kenney (kenney.nl) — CC0 1.0 (public domain). Used for
  the player fish (`fish.png` = `fish_orange`), the predator
  (`predator.png` = `fish_grey_long_a`) and the ambient bubbles
  (`bubble.png` = `bubble_a`). Source: https://kenney.nl/assets/fish-pack
- `pearl.png` and `bg_water.png` (the deep-water gradient backdrop) are
  original textures authored for this coursework.

The cave walls are still rendered from a sprite generated procedurally at
runtime by `CaveBuilder.cs`, and `RuntimeSprites.cs` provides the soft glow
behind each pearl and a fallback shape if any art fails to load.

## Audio

None yet. Audio (background loop + sfxr/bfxr effects) lands in Week 3.
