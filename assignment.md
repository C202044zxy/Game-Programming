# 2D Game Improvement Assignment

**Course:** Game Programming
**Document type:** Instructor handout

---

## Overview

In this class activity, you will update an existing 2D game into a clearer, more polished playable experience. A player should understand what to do, see useful information, receive feedback, and notice at least one meaningful improvement.

| | |
|---|---|
| **Time** | 75 minutes |
| **Marks** | 20 total |
| **Focus** | Menus, HUD, gameplay |

---

## Task

- Start from one 2D game project you already have, a class starter project, or an approved sample.
- Keep the core idea recognizable, but improve how the player starts, plays, understands, and finishes the game.
- Add at least three meaningful updates: one UI or menu update, one gameplay update, and one feedback or polish update.
- Test the full player flow: start, play, win or lose, restart, and exit or return to menu.

> Keep the scope realistic. A small polished improvement is better than a large unfinished idea.

---

## Minimum Requirements

- Create a main menu or start screen with **New Game**, **Instructions**, and **Exit** or **Back**.
- Update the HUD so it shows at least two useful values, such as score, lives, speed, timer, health, level, or high score.
- Add one gameplay change that affects how the game plays, not only how it looks.
- Give clear feedback for player actions using sound, animation, color, particles, hit effects, or screen messages.
- Add short objective or instruction text inside the game so the player knows the goal.
- Credit any borrowed art, audio, fonts, tutorials, code snippets, or starter assets.

---

## Good Ideas

- Add power-ups that change fire rate, speed, health, score multiplier, or jump strength.
- Increase difficulty as the score rises by changing movement speed, spawn rate, or enemy patterns.
- Create a second level, bonus area, or level select screen with simple objective text.
- Add a pause screen, retry button, or objective reminder for the current level.
- Replace placeholder art or sound with a more consistent style.

---

## Rubric and Success Criteria

**Total: 20 marks**

| Criterion | 4 marks | 2 to 3 marks | 0 to 1 mark |
|---|---|---|---|
| **Goal and interaction** | The game goal is clear. The main interaction works reliably and feels intentional. | The main interaction works, but one part is basic, unclear, or not fully polished. | The goal is unclear, the interaction is missing, or it does not work reliably. |
| **Menu and instructions** | Menu, instructions, and return flow are complete, readable, and easy to use. | Menu or instructions are present, but navigation, wording, or layout needs improvement. | Menu or instructions are missing, confusing, or not connected to the game. |
| **HUD and feedback** | HUD values are useful and readable. Feedback through sound, animation, color, or captions clearly helps the player. | HUD or feedback is included, but it is limited, inconsistent, or not always helpful. | HUD is missing or unclear, and the game gives little feedback when actions happen. |
| **Gameplay improvement** | A new feature meaningfully improves play, such as power-ups, difficulty scaling, new levels, objectives, or destructible objects. | A new feature is attempted, but it is small, rough, or only partly affects gameplay. | There is little evidence of a new feature or the feature is incomplete. |
| **Technical quality and polish** | The project is organized, stable, visually readable, and ready to demonstrate. | The project mostly works, but there are minor bugs, presentation issues, or organization problems. | The work is incomplete, unstable, hard to test, or difficult to demonstrate. |

> **Note:** I will reward clear design choices, working features, readable presentation, and evidence that you tested the complete player flow.

---

## What to Submit

- Submit the project folder or required scene, scripts, and asset files.
- Include a short change log explaining what you added, changed, and tested.
- Include a credits or citations note for borrowed resources.
- Be ready to show the game in Play mode and explain your choices in about one minute.

---

## Update Ideas and Build Checklist

Work in small steps. First make the existing game run, then add one improvement, test it, and only then add another. Do not wait until the end to check the menu, HUD, objective text, and restart flow.

### Recommended Workflow

1. **Choose one update** — Pick one main improvement. Write the player goal in one clear sentence.
2. **Build the menu** — Add start, instructions, changes or credits, and a clear way to return or leave.
3. **Improve the HUD** — Show values the player needs: score, lives, level, speed, timer, high score, or ammo.
4. **Add gameplay change** — Implement a power-up, enemy rule, objective, destructible object, new level, or difficulty rule.
5. **Add feedback** — Use sound, animation, color, screen text, particles, or camera response to confirm player actions.
6. **Test and explain** — Run from the menu, test every change, fix obvious bugs, and prepare a one-minute explanation.

### Student Checklist

- Keep file and scene names clear so I can find your work quickly.
- Write a short change log: what the game did before, what you changed, and why it improves the player experience.
- Run one final test from the menu before submitting.

### Possible Improvement Paths

- **Menu path:** add level select, instructions, changes, credits, restart, and exit controls.
- **HUD path:** add score, high score, lives, speed, timer, ammo, health, or objective progress.
- **Gameplay path:** add power-ups, enemy patterns, projectiles, collectibles, destructible objects, or a new level.
- **Polish path:** add consistent audio, animation, visual effects, readable fonts, and clear objective text.

---

## Helpful Resource Topics

Use the official documentation for your class game engine or framework. The table below lists topics to search when you need help implementing your update.

| Resource topic | Use it for |
|---|---|
| UI buttons and menus | New Game, Instructions, Changes, Exit, pause, and restart screens |
| 2D collisions or triggers | Pickups, enemies, hazards, objectives, and player contact |
| Audio tools | Click sounds, power-up effects, hit sounds, background music, and level themes |
| Score, lives, and manager script | Store values, update the HUD, restart the game, and track win or lose results |
| Tilemaps, sprites, and prefabs | Build levels quickly and reuse enemies, bullets, collectibles, roads, or maze pieces |
| Testing notes | Record what changed, what was tested, and what still needs work |

### Citation and Testing Reminder

- Use class examples first, then official documentation, then approved tutorials if needed.
- Credit borrowed art, audio, fonts, tutorials, code snippets, and starter assets.
- Do not add a feature you cannot test. A working small feature is better than an unfinished large feature.

---

## Final Self-Check

- [ ] Menu starts the game correctly.
- [ ] HUD updates while playing.
- [ ] New gameplay feature can be seen and tested.
- [ ] Objective text is readable.
- [ ] Change log and credits are included.
