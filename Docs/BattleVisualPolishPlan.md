# Battle Visual Polish Plan

Date: 2026-06-18

## 1. Purpose

This document defines a low-risk visual polish plan for the Battle screen before a public SRPG demo.

The goal is not to rebuild the battle presentation with new art. The goal is to make the current Battle screen look less cheap with the smallest practical cost by improving:

- UI consistency
- Board readability
- Tile and terrain symbolism
- Unit readability
- Range highlight clarity
- Lightweight combat feedback

This is a design document only. It does not change battle rules, enemy AI, stage data, input, victory/defeat conditions, audio behavior, or runtime flow.

## 2. Source Check

### Confirmed Existing Files

These files were checked read-only before writing this document:

- `Assets/Scripts/UI/BattleUI.cs`
- `Assets/Scripts/Grid/Tile.cs`
- `Assets/Scripts/Grid/GridManager.cs`
- `Assets/Scripts/Units/Unit.cs`
- `Assets/Scripts/UI/DamagePopup.cs`
- `Docs/CurrentPrototypeStatus.md`
- `Docs/UnityManualTestChecklist.md`
- `Docs/ReleaseChecklist.md`

### Notes About Existing Docs

- `Docs/CurrentPrototypeStatus.md` is readable and was used as the main status source.
- `Docs/UnityManualTestChecklist.md` exists, but its content appears mojibake/encoding-corrupted in the current shell output. It should not be used as a source of detailed wording until encoding is repaired or verified in an editor.
- `Docs/ReleaseChecklist.md` exists, but its content also appears mojibake/encoding-corrupted in the current shell output. It should be treated carefully for exact text reuse.

### Number Sources

- The Battle grid is described as `8x8` in `Docs/CurrentPrototypeStatus.md` under Implemented Systems.
- The stage loop is described as `6-stage progression` in `Docs/CurrentPrototypeStatus.md`.
- `BattleUI.cs` defines `MaxBattleLogEntries = 5`, confirmed by `rg`.
- `BattleUI.cs` defines `StagePreviewSize = 8`, confirmed by `rg`; this is for Stage Select preview, not direct Battle board size.

## 3. Current Battle Screen State

### Confirmed From Docs

`Docs/CurrentPrototypeStatus.md` confirms the following Battle-related systems are implemented:

- Runtime 8x8 grid generation
- Obstacles and Goal tiles
- Player unit selection
- Movement range display
- Attack range display
- Enemy threat display
- Selected enemy movement/attack range display
- Guardian reaction range display in Enemy Threat view
- HP display above units
- Damage popup text
- UnitType-specific attack motion for Soldier / Knight / Archer / Rogue
- Attack preview on enemy hover
- Battle log UI
- Controls UI
- Result summary UI

### Confirmed From Code

#### UI

`BattleUI.cs` owns the runtime UI for the Battle screen. The file contains serialized references and creation paths for:

- Stage info panel
- Objective panel
- Selected unit panel
- Enemy Threat panel
- Battle Log panel
- Controls panel
- Attack Preview panel
- Result panel

The Battle HUD is assembled through methods such as:

- `EnsureBattleHudObjects()`
- `EnsurePanelPair(...)`
- `SetBattleHudVisible(...)`
- `SetControlsInfo()`
- `RefreshBattleLogText()`
- `ShowAttackPreview(...)`
- `ShowResult(...)`

#### Tiles and Board

`Tile.cs` controls tile colors, terrain visual layers, and highlight overlays. It has colors/flags for:

- Normal terrain
- Obstacle terrain
- Goal terrain
- Move highlight
- Attack highlight
- Enemy move threat highlight
- Enemy attack threat highlight
- Guardian reaction highlight

`GridManager.cs` controls board generation and world-level board presentation. It contains colors and helper creation for:

- Tile base colors
- Board frame
- Board base
- Board backdrop
- Board shadow
- Battle mist/light panels
- World panels around the board

#### Units

`Unit.cs` controls unit sprites and visual child renderers. It includes:

- Resource sprite loading path
- Runtime pixel sprite fallback
- Unit shadow
- Selection ring
- HP text and HP text background
- Top light overlay
- Hit flash
- Attack scale pulse
- KO popup

#### Damage Popup

`DamagePopup.cs` controls temporary floating text for damage and KO-like feedback. It tracks active popups and supports cleanup.

## 4. Current Issues

The following issues are based on code inspection plus the known project direction. They should be rechecked visually in Unity before implementation.

### Priority A: Range and Tile Readability

Risk:

- Range colors can compete with each other.
- Guardian reaction range, enemy move threat, enemy attack threat, player move range, and player attack range now all share the same tile overlay system.
- `Tile.RefreshColor()` uses a single chosen overlay color based on priority; overlapping states cannot all be visible at once.

Why it can look cheap:

- If colors are too saturated or too similar, the board reads as noisy.
- If the same tile can mean several things but only one color is shown, players may mistrust the UI.

### Priority B: Battle UI Information Density

Risk:

- Battle HUD has many panels: stage, objective, selected, threat, log, controls, attack preview, result.
- `BattleUI.cs` positions most Battle HUD elements by fixed runtime-created UI coordinates.

Why it can look cheap:

- Too many boxes can make the Battle view feel like debug UI.
- Dense text and inconsistent spacing can lower perceived quality even if the systems work.

### Priority C: Unit Readability

Risk:

- Unit art currently mixes resource sprite loading and runtime fallback pixel sprites.
- HP text, selection rings, shadows, and top-light overlays all sit near the unit body.

Why it can look cheap:

- If HP labels sit too close to heads or weapons, units feel cramped.
- If the selected ring or shadow is too strong, it can look like a blob under the unit.
- If fallback sprites and resource sprites differ too much, visual consistency can break.

### Priority D: Combat Feedback

Risk:

- Current effects are functional: attack scale pulse, hit flash, KO popup, damage popup.
- They may still feel minimal compared with the more polished Title and Stage Select screens.

Why it can look cheap:

- A hit that only flashes briefly can feel flat.
- Damage popups without timing polish can look detached from the attack.
- KO feedback may need a little more clarity without becoming noisy.

### Priority E: Background and Decoration

Risk:

- The Battle screen already has dark backdrop, board frame, mist/light panels, and a board stage light.
- Adding more decoration too early could make the screen busy.

Why it can look cheap:

- Too little atmosphere makes the board feel like a prototype.
- Too much decoration can hide gameplay information.

## 5. Polish Direction

### Confirmed Direction

The project visual direction is already moving toward:

- Dark fantasy
- Gold UI frames
- Blue/red faction readability
- Tactical clarity
- HD-2D-inspired diorama feel

### Recommended Direction

Before adding lots of images, prioritize:

1. UI consistency
2. Board readability
3. Clear tactical symbols
4. Unit legibility
5. Small combat feedback improvements

The Battle screen should still be readable first. Visual polish should support tactical decisions, not cover them.

### Do Before Demo

- Clarify highlight color hierarchy.
- Keep HUD panels consistent and readable.
- Ensure unit HP, selection state, and faction are readable at common 16:9 resolutions.
- Make attack/hit/KO feedback feel clear but short.

### Do Not Do Before Demo

- Full 2DHD conversion
- Large character illustration pass
- Large animation library
- New stage backgrounds per stage
- New combat mechanics
- Full UI framework rewrite

## 6. Implementation Priorities

### Priority 1: Tile and Highlight Color Cleanup

Target files:

- `Assets/Scripts/Grid/Tile.cs`
- `Assets/Scripts/Grid/GridManager.cs`
- `Assets/Scripts/Battle/PlayerController.cs`

What to change:

- Review `Tile.RefreshColor()` highlight priority.
- Keep player-selected action highlights readable over enemy threat ranges.
- Consider enemy attack threat higher priority than enemy move threat.
- Keep Guardian reaction range visibly distinct but lower priority than actual attack danger.
- Tune alpha values so tiles retain terrain identity under highlights.

Expected effect:

- Battle decisions become easier to read.
- Stage 4 Guardian information remains visible without overwhelming attack danger.

Risks:

- Changing priority can alter player interpretation of overlapped danger tiles.
- Too much transparency can make highlights hard to see.
- Too much saturation can make the board noisy.

Demo requirement:

- Required before demo if any range color is confusing in manual play.

### Priority 2: Battle UI Panel Spacing and Text Hierarchy

Target file:

- `Assets/Scripts/UI/BattleUI.cs`

What to change:

- Standardize panel title colors, body text color, and spacing.
- Reduce debug-like density in Objective, Selected, Battle Log, and Controls panels.
- Keep Controls two-column layout readable.
- Keep Battle Log empty state visible but subdued.
- Make Attack Preview and Result Summary match the same panel style.

Expected effect:

- Battle screen feels like the same product as Title and Stage Select.
- Text-heavy panels look intentional rather than provisional.

Risks:

- Fixed coordinate UI can break at smaller 16:9 windows if panel sizes change.
- Over-polishing panel borders can make the screen boxy.

Demo requirement:

- Required before demo for visible polish pass, but should be done in small layout-only commits.

### Priority 3: Unit Visual Legibility

Target file:

- `Assets/Scripts/Units/Unit.cs`

What to change:

- Check unit sprite scale per UnitType.
- Check HP text vertical offset and black backing placement.
- Check selection ring size and alpha.
- Ensure acted state remains readable on resource sprites.
- Keep Soldier / Knight / Archer / Rogue silhouettes distinct.

Expected effect:

- Units read better at a glance.
- HP no longer competes with heads/weapons.
- Player can quickly identify selected unit and acted units.

Risks:

- Over-scaling units can obscure tile highlights.
- HP text changes can look good on one unit type and bad on another.
- Resource sprite pivot changes can affect all unit placement.

Demo requirement:

- Required if manual checks show HP overlap or selected unit ambiguity.

### Priority 4: Combat Feedback Timing

Target files:

- `Assets/Scripts/Units/Unit.cs`
- `Assets/Scripts/UI/DamagePopup.cs`
- Possible animation-related file if attack animation logic is separated elsewhere

What to change:

- Tune hit flash duration.
- Tune damage popup rise speed / lifetime.
- Tune KO popup size and timing.
- Keep attack feedback short enough that enemy turns do not drag.

Expected effect:

- Attacks feel clearer.
- Damage and KO are easier to notice.
- Combat feels less flat without changing mechanics.

Risks:

- Longer effects can slow perceived battle tempo.
- More intense flashes can become visually noisy.

Demo requirement:

- Should-have before demo, not required if current visible build already feels acceptable.

### Priority 5: Background and Board Atmosphere

Target files:

- `Assets/Scripts/Grid/GridManager.cs`
- `Assets/Scripts/Grid/Tile.cs`

What to change:

- Tune board backdrop and mist/light panel colors.
- Make board frame and inner light subtly support the board without competing with tiles.
- Keep obstacles and goals readable.
- Avoid new image assets unless a later art pass explicitly requires them.

Expected effect:

- Battle board feels more like a diorama.
- The scene feels less like a raw grid.

Risks:

- Atmosphere layers can reduce tile contrast.
- Darker backgrounds may hide red enemies or HP text.

Demo requirement:

- Optional polish. Do after readability is stable.

### Priority 6: Additional Decoration

Target files:

- `Assets/Scripts/UI/BattleUI.cs`
- `Assets/Scripts/Grid/GridManager.cs`

What to change:

- Add only minor decorative accents if needed.
- Avoid more panels unless they solve a readability problem.
- Avoid decorative animation before gameplay UI is fully stable.

Expected effect:

- Slightly more premium presentation.

Risks:

- Decoration can become visual clutter.
- It can distract from tactical board state.

Demo requirement:

- Later. Not required for demo candidate.

## 7. Deferred Items

These should be deferred until after the demo candidate is stable:

- Full character illustration replacement
- Complete 2DHD art conversion
- Large animation set per unit and action
- Per-stage background illustration pass
- Full lighting/post-processing stack
- Advanced particle effects
- Cinematic camera movement
- New tile art atlas
- New skill VFX
- Large UI framework rewrite

## 8. Recommended Task Order

These are intentionally small so they can be handed to Codex/OpenHands one at a time.

1. Inspect Battle highlight priority and propose exact color/priority changes.
2. Implement tile/highlight color tuning only.
3. Manually verify Move / Attack / Enemy Threat / Guardian Reaction overlap in Stage 1, Stage 4, and Stage 6.
4. Inspect Battle HUD panel layout at 1280x720, 1600x900, and 1920x1080.
5. Adjust Battle HUD panel text spacing only.
6. Manually verify Selected, Controls, Battle Log, Attack Preview, and Result Summary.
7. Inspect unit HP overlap across Soldier / Knight / Archer / Rogue, player and enemy.
8. Adjust HP label / backing / selection ring only.
9. Manually verify unit readability in Stage 1 and Stage 4.
10. Inspect attack/hit/KO timing.
11. Tune DamagePopup and hit flash timing only.
12. Manually verify player attack, enemy attack, KO, Victory, and Defeat.
13. If still needed, tune board atmosphere colors and inner light.
14. Run a final six-stage visual pass before Windows demo rebuild.

## 9. Manual Verification Checklist

### Battle Screen Readability

- [ ] Normal tiles remain visible under no highlight.
- [ ] Obstacles are immediately recognizable as blocked tiles.
- [ ] Goal tiles are recognizable without hiding units.
- [ ] Player move range is distinct from player attack range.
- [ ] Enemy attack threat is distinct from enemy movement threat.
- [ ] Guardian reaction range is distinct from enemy attack threat.
- [ ] Overlapping highlights do not mislead the player.

### UI Panels

- [ ] Stage info panel is readable.
- [ ] Objective/Tip panel is readable and not over-dense.
- [ ] Selected panel is readable for no selection, player selection, and enemy selection.
- [ ] Enemy Threat panel reflects ON/OFF state.
- [ ] Battle Log has a clear empty state and readable recent logs.
- [ ] Controls panel is readable and aligned.
- [ ] Attack Preview appears only when appropriate.
- [ ] Result Summary does not overflow.

### Unit Readability

- [ ] Player units and enemy units are distinguishable by color.
- [ ] Soldier / Knight / Archer / Rogue are distinguishable by silhouette.
- [ ] HP text does not overlap the most important part of the unit.
- [ ] Selected unit ring is visible.
- [ ] Acted player unit state remains visible.
- [ ] Damage popup is readable over the board.

### Combat Feedback

- [ ] Attack motion is visible but not slow.
- [ ] Hit flash is visible but not excessive.
- [ ] KO feedback is clear.
- [ ] Enemy turn does not feel delayed by effects.
- [ ] Victory / Defeat result appears after the correct moment.

### Regression Checks

- [ ] Click Ally: Select still works.
- [ ] Hover Enemy: Preview still works.
- [ ] Click Enemy: Info / Attack still works.
- [ ] Click Blue Tile: Move still works.
- [ ] W Wait / Confirm still works.
- [ ] U Undo Move still works.
- [ ] R Restart Stage still works.
- [ ] Space Threat toggle still works.
- [ ] Esc / S Stage Select still works.
- [ ] Enter Next / Retry still works.

## 10. Risks and Guardrails

### Confirmed Guardrails

- Do not change battle logic.
- Do not change enemy AI.
- Do not change StageData balance.
- Do not change victory/defeat logic.
- Do not add new assets during the first polish pass.

### Main Risks

- UI polish can accidentally affect layout at smaller 16:9 resolutions.
- Highlight changes can make tactical information less clear.
- Unit scale changes can hide tile highlights or HP text.
- Effect timing changes can make enemy turns feel slower.

### Recommended Mitigation

- Keep each polish pass scoped to one visual layer.
- Verify in Unity Editor after every small change.
- Prefer color/spacing/timing changes over new systems.
- Document any subjective visual choice that needs user approval.

## 11. Suggested Definition of Done

Battle visual polish is demo-ready when:

- Stage 1, Stage 4, and Stage 6 are visually readable.
- All core range displays remain distinct.
- Guardian reaction range is understandable in Stage 4.
- Selected unit and target unit are clear.
- HP and damage popups are readable.
- Battle UI matches the Title and Stage Select tone.
- No existing controls or battle rules regress.
- A visible Windows build check confirms the result.
