# Current Prototype Status

Date: 2026-06-05

## Summary

This is a Unity 2D short puzzle-SRPG prototype. The current build is a playable 6-stage loop that can be completed from start to finish in the Unity Editor. The project is designed to run from an empty scene: managers, camera, UI, grid, stage data, terrain, and units are created automatically at Play.

## Unity / Verification

- Unity Editor: 6000.4.9f1
- Runtime setup: Empty scene auto-bootstrap
- Full playthrough: Confirmed
- Lightweight compile: Passing
- Unity compile: Passing
- Six-stage light recheck: Passing
- Fresh v0.4 full editor playthrough: Confirmed
- Screen transition check: Passing
- v0.4 demo polish checklist: Recorded in `Docs/v0.4DemoPolishChecklist.md`
- v0.5 audio/build checklist: In progress in `Docs/v0.5AudioBuildChecklist.md`
- v0.5 Windows build verification: Build succeeded, hidden player smoke reached Title screen
- v0.5 warning cleanup: `FindObjectOfType<T>()` deprecation warning resolved
- v0.5 Title Options visible check: Confirmed by user in Unity Editor
- v0.5 Options rebuild: dotnet compile, Unity batchmode compile, Windows build, and hidden Title smoke passed
- v0.5 final visible Windows check: Title, Options, Stage Select, Stage 1 Battle, Result, Exit, and core SE confirmed by user
- v0.5 latest Windows rebuild: title operation-bar text fix included; hidden smoke reached Title screen
- v0.5 Options display settings: resolution presets and Windowed / Fullscreen toggle added, build and hidden smoke passed
- Guardian Reaction Range display: Implemented in `ffd3475` and confirmed in Unity Editor

## Main Scripts

- `Assets/Scripts/Grid/Tile.cs`
- `Assets/Scripts/Grid/GridManager.cs`
- `Assets/Scripts/Units/Unit.cs`
- `Assets/Scripts/Battle/PlayerController.cs`
- `Assets/Scripts/Battle/TurnManager.cs`
- `Assets/Scripts/Audio/AudioManager.cs`
- `Assets/Scripts/UI/BattleUI.cs`
- `Assets/Scripts/UI/DamagePopup.cs`
- `Assets/Scripts/Stage/StageData.cs`
- `Assets/Scripts/Stage/StageLoader.cs`
- `Assets/Scripts/Stage/StageManager.cs`

## Implemented Systems

- Runtime 8x8 grid generation
- StageData-driven stage loading
- Title screen
- 6-stage progression
- Stage select screen
- Title Options screen for temporary audio settings
- Polished Stage Select layout with row highlights, chapter card, right-side detail panel, decorated header, and bottom controls bar
- Obstacles and Goal tiles
- Player unit selection
- Movement range display
- Attack range display
- Enemy threat display
- Selected enemy movement/attack range display
- Guardian reaction range display in Enemy Threat view
- HP, attack, movement, attack range
- Unit death and board occupancy cleanup
- Board HP text above units
- Damage popup text
- Temporary runtime-generated BGM / SE
- AudioManager with replaceable AudioClip fields
- Runtime Options controls for Master / BGM / SE volume and Mute
- Runtime Options display controls for 1280x720 / 1600x900 / 1920x1080 and Windowed / Fullscreen
- Debug mute toggle
- Runtime-generated placeholder pixel unit sprites
- Simple attack pulse, hit flash, and KO popup
- UnitType-specific attack motion for Soldier / Knight / Archer / Rogue
- Attack preview on enemy hover
- Move, then attack or wait to confirm
- U key movement undo before action confirmation
- W key wait / confirm
- R key current-stage restart
- Turn system
- Enemy AI types: Aggressive, WeakTarget, Stationary, Guardian
- Enemy information selection
- Enemy AI prediction text
- Victory conditions: DefeatAllEnemies, ReachGoal
- Defeat conditions: AllPlayersDefeated, TurnLimitExceeded
- Stage intro UI
- Stage objective UI
- Stage description text
- Controls UI
- Battle log UI
- Clear evaluation log
- Victory / Defeat / All Clear result UI
- Result summary with Rating, Turn, Survivors, HP Total, objective, limit, and enemies left

## Recent Visual Combat Update

Date: 2026-06-07

- Added `UnitAttackAnimator` for short UnitType-specific attack motion.
- Soldier: quick forward slash with a small slash effect.
- Knight: heavier wind-up, short lunge, and impact flash effect.
- Archer: brief draw-back with a small arrow projectile.
- Rogue: faster lunge with a compact slash effect.
- Player attacks and enemy AI attacks both use the same UnitType animation path.
- Damage calculation, attack range, enemy AI decisions, victory/defeat rules, Undo, Restart, Threat display, and StageData are unchanged.
- Player input is locked while an attack animation is playing to avoid accidental clicks/keys during the hit sequence.
- Lightweight compile check passed after implementation.

## Controls

```text
Click Ally: Select
Hover Enemy: Preview
Click Enemy: Info / Attack
Click Blue Tile: Move
W: Wait / Confirm
U: Undo Move
R: Restart Stage
Esc / S: Stage Select
Space: Toggle Threat / Selected Enemy Range
M: Mute / Unmute audio
Enter: Next / Retry
```

Title / stage select:

```text
Title:
Up / Down: Select
Enter: Confirm
Options: Master / BGM / SE / Mute
Left / Right: Adjust option values
Esc: Back

Stage Select:
Up / Down: Select
Enter: Start
1-6: Quick Start
Esc / Backspace: Title
```

Stage select displays:

- DisplayName
- ThemeName
- DifficultyLabel
- Description
- Objective
- TurnLimit
- Grid size
- Player / enemy count
- Obstacle / Goal count

## Current Stage Order

1. Opening Formation - Intro
2. Crossfire Lanes - Easy+
3. Goal Under Pressure - Medium
4. Guardian Split - Hard
5. Last Turn Breakthrough - Hard+
6. Final Escape - Final

## Recent Balance Adjustments

All recent balance changes were limited to `StageData.cs`.

- Stage 4: increased player Archer and Rogue HP by 1.
- Stage 4: reduced both Guardian enemies from ATK 6 to ATK 4.
- Stage 5: reduced the Guardian HP from 13 to 12.

The goal of these changes was to reduce the overly sacrificial feel of Stage 4 and slightly soften the strict final damage check in Stage 5 without changing mechanics.

## Current Design Direction

The prototype has moved beyond the v0.2 playable lock into a v0.4 demo polish baseline. The core 6-stage loop is validated, stage identity is visible in UI, result summaries expose enough information for balancing, and title-to-stage-select-to-battle flow is available for fast retesting.

Current priority is Steam demo readiness: audio pass, Windows build verification, resolution checks, and screenshot candidate selection. New mechanics should remain deferred until the demo candidate is stable.

## v0.5 Windows Build Verification

Date: 2026-06-07

- Windows build script: `SRPG.EditorTools.BuildDemo.BuildWindows`
- Build target: `StandaloneWindows64`
- Output: `Builds/Windows/FinalEscapeTacticsDemo.exe`
- dotnet compile: Pass, 0 warnings, 0 errors
- Unity batchmode compile: Pass, exit code 0
- Windows build: Pass, exit code 0
- Player smoke: Pass, hidden player launch reached Title screen
- 16:9 launch smoke:
  - 1280x720: Title reached
  - 1600x900: Title reached
  - 1920x1080: Title reached
- Logs:
  - `Logs/v0.5WindowsBuildCompile.log`
  - `Logs/v0.5WindowsBuild.log`
  - `Logs/v0.5WindowsPlayerSmoke.log`
  - `Logs/v0.5WindowsPlayerSmoke_1280x720.log`
  - `Logs/v0.5WindowsPlayerSmoke_1600x900.log`
  - `Logs/v0.5WindowsPlayerSmoke_1920x1080.log`

Remaining manual build checks:

- Visible Windows build UI layout at 1280x720, 1600x900, and 1920x1080.
- Actual BGM / SE listening check.
- Title -> Stage Select -> Battle input in the Windows build.
- Battle attack motion / input lock in the Windows build.
- Victory / Defeat / Result Summary in the Windows build.
- Title EXIT behavior in the Windows build.

## v0.5 Cleanup Pass

Date: 2026-06-07

- Replaced remaining runtime `FindObjectOfType<T>()` calls with `FindAnyObjectByType<T>()`.
- Updated local Unity stubs for compile verification.
- dotnet compile: Pass, 0 warnings, 0 errors.
- Unity batchmode compile: Pass, exit code 0.
- Windows build: Pass, exit code 0.
- Build log: `Logs/v0.5FindAnyObjectWindowsBuild.log`.
- Scope: warning cleanup only. No battle rules, enemy AI, StageData, damage calculation, UI layout, Undo, Restart, or Threat behavior changes.

## v0.5 Options Visible Check

Date: 2026-06-07

- Title Options screen was added for temporary audio settings.
- Options supports Master / BGM / SE volume adjustment, Mute toggle, and Back to Title.
- User confirmed the Options screen operated without issue in Unity Editor.
- dotnet compile after the Options check: Pass, 0 warnings, 0 errors.
- Unity batchmode compile after the Options check: Pass, exit code 0.
- Windows rebuild after the Options check: Pass, exit code 0.
- Hidden Windows player smoke after the Options rebuild: Pass, reached Title screen.
- Logs:
  - `Logs/v0.5OptionsCompile.log`
  - `Logs/v0.5OptionsWindowsBuild.log`
  - `Logs/v0.5OptionsWindowsPlayerSmoke.log`
- Scope: audio settings UI only. No battle logic, enemy AI, StageData, victory/defeat rules, Undo, Restart, or Threat behavior changes.

## v0.5 Final Visible Windows Check

Date: 2026-06-11

User-confirmed:

- Title: Up / Down, Enter, Options, and Exit work.
- Options: layout is intact, can return to Title, and Title controls still work after returning.
- Stage Select: Up / Down, Enter, 1-6 Quick Start, and Esc / Backspace work.
- Stage 1 Battle: selection, movement, attack, attack motion, SE, Damage Popup, W Wait / Confirm, U Undo, R Restart, Space Threat, and Esc / S work.
- Result: Victory, Result Summary, and Enter to Stage 2 work.
- Audio: Cursor, Confirm, Cancel, Attack, Hit, KO, and Victory SE were confirmed and not reported as too loud.

Issue found and addressed:

- Title operation-bar text was missing in the visible build.
- `BattleUI.cs` was adjusted so the title prompt text is placed on the Canvas root and brought to the front.
- dotnet compile after fix: Pass, 0 warnings, 0 errors.
- Unity batchmode compile after fix: Pass, exit code 0.
- Windows build after fix: Pass, exit code 0.
- Hidden Windows smoke after fix: Pass, reached Title screen.

Remaining:

- Re-open the freshly rebuilt executable and visually confirm the Title operation-bar text is now visible.
- Visible 1280x720 / 1600x900 / 1920x1080 layout check.
- Optional full six-stage visible build playthrough before public demo packaging.

## v0.5 Options Display Settings

Date: 2026-06-11

- Options now includes Resolution and Display rows.
- Resolution presets: 1280x720, 1600x900, 1920x1080.
- Display modes: Windowed, Fullscreen.
- Left / Right adjusts Resolution and Display rows.
- Enter toggles Mute / Display or confirms Back.
- dotnet compile: Pass, 0 warnings, 0 errors.
- Unity batchmode compile: Pass, exit code 0.
- Windows build: Pass, exit code 0.
- Hidden Windows smoke: Pass, reached Title screen.
- Logs:
  - `Logs/v0.5OptionsDisplayCompile.log`
  - `Logs/v0.5OptionsDisplayWindowsBuild.log`
  - `Logs/v0.5OptionsDisplayWindowsPlayerSmoke.log`

Remaining:

- Visible Options check for Resolution and Display switching.
- Visible layout checks at 1280x720, 1600x900, and 1920x1080.

## Guardian Reaction Range Display

Date: 2026-06-18

- Implementation commit: `ffd3475 Show guardian reaction range in enemy threat view`.
- Stage 4 manual check confirmed Guardian reaction range appears when Enemy Threat is ON.
- When no enemy is selected, all Guardian reaction ranges are displayed.
- When an individual Guardian is selected, only that Guardian's reaction range is displayed.
- Enemy Threat OFF clears the Guardian reaction range display.
- Guardians still guard / wait while players remain outside the reaction range.
- Guardians still activate normally when a player enters the reaction range.
- Guardian AI behavior was not changed.
- `GuardianReactionRange = 3` was preserved.
- `StageManager`, `StageData`, and `TurnManager` were not changed by this implementation.
- No fatal Console errors were reported during the Unity Editor manual check.
