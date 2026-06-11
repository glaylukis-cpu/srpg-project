# Visual Direction: 2DHD-Inspired Prototype Notes

Date: 2026-06-04

## Current Visual Baseline

The project still uses runtime-generated prototype visuals:

- Checkerboard tile grid
- Terrain color coding
- Runtime-generated 8x8-style unit sprites
- Full-screen generated title background loaded from `Assets/Resources/Title/TitleBackground.png`
- Panel-based Stage Select layout inspired by tactical RPG menu screens, with a chapter card, decorated header, highlighted rows, right-side stage detail, and a dedicated controls footer
- TextMesh HP labels
- Text damage popups
- Simple attack pulse / hit flash / KO popup
- Unity UI Text for menus and battle information

This is intentionally light. No imported art pipeline is required yet.

## 2DHD-Inspired Direction

The target is not full 2DHD production quality yet. The useful direction for this prototype is:

- Readable tactical board first
- Pixel-style unit silhouettes
- Soft depth and contrast around important board information
- Slightly cinematic title / stage select presentation
- Small action feedback instead of full animation

## Recommended Next Steps

1. Camera / Board Presentation
   - Keep orthographic for now.
   - Consider a slight board shadow or simple ground backdrop.
   - Avoid perspective until grid clicking and coordinates remain reliable.

2. Tile Visuals
   - Keep color semantics stable:
     - Move: blue
     - Attack: red
     - Enemy move/threat: purple/pink
     - Goal: green
     - Obstacle: dark gray
   - If adding texture, use low-contrast pixel noise so highlights remain readable.

3. Unit Visuals
   - Replace generated 8x8 sprites with small imported sprites only after unit roles stabilize.
   - Use consistent silhouettes:
     - Soldier: compact frontliner
     - Knight: wide heavy body
     - Archer: slim ranged body
     - Rogue: narrow mobile body

4. Effects
   - Keep attack feedback short:
     - Attacker pulse
     - Target flash
     - Damage popup
     - KO popup
   - UnitType attack motion has been added in a lightweight form:
     - Soldier: quick slash lunge
     - Knight: slower heavy hit
     - Archer: small arrow projectile
     - Rogue: fast slash lunge
   - Keep each attack under roughly 0.5 seconds so enemy turns do not drag.
   - Avoid turning these into full animation-controller workflows until the final art pipeline is chosen.

5. UI
   - Title and stage select should become the first real polish target.
   - Add panels/backgrounds before adding more text.
   - Result Summary should stay scan-friendly: rating, turn, survivors, HP, enemies left.

## Avoid For Now

- Full 3D camera tilt
- Complex lighting
- Particle-heavy attacks
- Imported large asset sets
- Animation controllers for every unit type
- UI redesign that hides tactical information

## Decision

For v0.3, pursue a "board-game pixel tactics" look first. Treat true 2DHD as a later art direction once the stage flow and puzzle quality are stable.
