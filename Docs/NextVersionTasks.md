# Next Version Tasks

Target: v0.4 demo polish baseline, then v0.5 audio/build preparation.

See also:

- `Docs/SteamDemoPlan.md`
- `Docs/v0.4DemoPolishChecklist.md`
- `Docs/v0.5AudioBuildChecklist.md`

## Completed for v0.2

- Six-stage light recheck passed.
- Clear Evaluation logs were recorded in `SixStagePlaytestRecord.md`.
- Stage descriptions were added to `StageData`.
- Stage Intro and left-top UI show stage identity more clearly.
- Result UI shows Rating, Turn, Survivors, and HP Total.
- Stage select screen was added for faster retesting.
- Title screen was added before stage select.
- Battle can return to Stage Select with Esc / S.
- Stage Select displays stage details from StageData.
- README and controls memo were added.
- `Docs/v0.2StateLock.md` records the locked state.
- Unity batchmode compile passed.
- Result Summary was strengthened for scanability.
- Runtime-generated placeholder pixel unit sprites were added.
- Simple attack pulse, hit flash, and KO popup were added.
- 2DHD-inspired visual direction notes were added.

## Completed for v0.4 Demo Polish Base

- Title / Stage Select / Battle now share a dark + gold + blue UI direction.
- Stage Select has a polished menu layout with stage details, difficulty colors, map preview, and Current Session Best.
- Battle HUD has been panelized and visually aligned with the menu screens.
- Battle board visuals, tiles, obstacles, goals, units, HP text, highlights, and light feedback have been polished for prototype presentation.
- Steam demo scope was organized in `Docs/SteamDemoPlan.md`.
- v0.4 checklist execution was recorded in `Docs/v0.4DemoPolishChecklist.md`.
- Dotnet compile passed.
- Unity batchmode compile passed.

## Completed for v0.5 Audio / Build Prep

- Added `AudioManager` with separate BGM and SE playback.
- Added quiet runtime-generated placeholder BGM for Title, Stage Select, and Battle.
- Added quiet runtime-generated placeholder SE for cursor, confirm, cancel, attack, hit, KO, Victory, Defeat, Restart, and Undo.
- Wired BGM switching to Title, Stage Select, Battle, Victory / Defeat, and ALL CLEAR.
- Wired SE to core menu input, stage start, battle actions, result flow, Restart, Undo, and Threat toggle.
- Added debug mute toggle with `M`.
- Added Title Options screen for temporary Master / BGM / SE volume and Mute controls.
- User confirmed the Title Options screen operated without issue in Unity Editor.
- Added Windows build script: `SRPG.EditorTools.BuildDemo.BuildWindows`.
- Added v0.5 checklist in `Docs/v0.5AudioBuildChecklist.md`.
- Windows build succeeded at `Builds/Windows/FinalEscapeTacticsDemo.exe`.
- Hidden Windows player smoke test reached the Title screen.
- 1280x720 / 1600x900 / 1920x1080 hidden launch smoke tests reached the Title screen.
- Resolved `FindObjectOfType<T>()` deprecation warning by moving runtime lookups to `FindAnyObjectByType<T>()`.

## Priority 1 - v0.4 Final Manual Recheck

- Run one fresh 6-stage editor playthrough after the latest visual polish.
- Recheck Stage 5 and Stage 6 difficulty feel.
- Recheck Title -> Stage Select -> Battle -> Result transitions.
- Recheck Restart, Undo, Threat, Wait / Confirm, and Esc flows.
- Confirm there are no major UI overlaps at the target resolution.

## Priority 2 - v0.5 Audio / Feedback

- Manually listen to Title / Stage Select / Battle BGM in Unity Editor.
- Manually check attack, hit, KO, Victory, Defeat, Restart, Undo, and Cancel SE.
- Tune volume only if the placeholder sounds are distracting.
- Replace runtime-generated clips later with real assets when available.

## Priority 3 - Windows Build Prep

- Run the Windows build in a visible window.
- Verify Title / Stage Select / Battle controls in the build.
- Check 16:9 resolutions visually for UI overflow.
- Confirm Exit works in the build.
- Record any build-only warnings or errors.

## Latest v0.5 Build Verification Notes

- Date: 2026-06-07
- dotnet compile: Pass, 0 warnings, 0 errors.
- Unity batchmode compile: Pass, exit code 0.
- Windows build: Pass, `Builds/Windows/FinalEscapeTacticsDemo.exe`.
- Hidden player smoke: Pass, Title reached.
- Hidden 16:9 launch smoke: Pass at 1280x720, 1600x900, and 1920x1080.
- Deprecated `FindObjectOfType<T>()` warning: resolved, cleanup build passed.
- Remaining: visible build interaction, subjective audio check, visible resolution layout check, and Title EXIT behavior.

## Latest v0.5 Options Notes

- Date: 2026-06-07
- Title Options screen: Editor-visible operation confirmed by user.
- dotnet compile after Options confirmation: Pass, 0 warnings, 0 errors.
- Unity batchmode compile after Options confirmation: Pass, exit code 0.
- Windows rebuild after Options confirmation: Pass, exit code 0.
- Hidden Windows player smoke after Options rebuild: Pass, reached Title screen.
- Remaining before v0.5 lock:
  - Visible Windows check for Options, Title EXIT, audio levels, and core Stage 1 flow.
  - Visible 1280x720 / 1600x900 / 1920x1080 layout check.

## Latest v0.5 Final Visible Notes

- Date: 2026-06-11
- Visible Windows build manual check:
  - Title controls confirmed.
  - Options confirmed.
  - Stage Select controls confirmed.
  - Stage 1 battle controls confirmed.
  - Victory / Result Summary / Enter to Stage 2 confirmed.
  - Cursor / Confirm / Cancel / Attack / Hit / KO / Victory SE confirmed and not reported as too loud.
  - Title EXIT confirmed to close the executable.
- Issue found:
  - Title operation-bar text was missing in the visible build.
- Fix applied:
  - `BattleUI.cs` now places title prompt text on the Canvas root and brings it to the front.
  - dotnet compile: Pass, 0 warnings, 0 errors.
  - Unity batchmode compile: Pass, exit code 0.
  - Windows build: Pass, exit code 0.
  - Hidden Windows smoke: Pass, reached Title screen.
- Remaining before v0.5 lock:
  - Re-open the freshly rebuilt executable and visually confirm the Title operation-bar text is visible.
  - Visible 1280x720 / 1600x900 / 1920x1080 layout check.
  - Optional full six-stage visible build playthrough.

## Latest v0.5 Options Display Notes

- Date: 2026-06-11
- Added Resolution and Display rows to the existing Title Options screen.
- Resolution presets:
  - 1280x720
  - 1600x900
  - 1920x1080
- Display modes:
  - Windowed
  - Fullscreen
- Verification:
  - dotnet compile: Pass, 0 warnings, 0 errors.
  - Unity batchmode compile: Pass, exit code 0.
  - Windows build: Pass, exit code 0.
  - Hidden Windows smoke: Pass, reached Title screen.
- Remaining before v0.5 lock:
  - Visible Options check for Resolution switching.
  - Visible Options check for Windowed / Fullscreen switching.
  - Visible layout check at each supported resolution.

## Priority 4 - Steam Page Prep

- Capture screenshot candidates:
  - Title
  - Stage Select
  - Stage 4 / 5 / 6 battle screens
  - Attack Preview
  - Enemy Threat ON
  - Victory Result Summary
- Draft short Steam demo description text.
- Confirm title/background art usage rights before public use.

## Priority 5 - Puzzle Quality

- Review whether sacrifice routes are intended or accidental.
- Make sure each stage teaches or tests one main idea:
  - Stage 1: basic actions
  - Stage 2: threat lanes
  - Stage 3: goal routing under pressure
  - Stage 4: guardian pull timing
  - Stage 5: strict action economy
  - Stage 6: objective discipline and escape
- Avoid adding new mechanics until each current stage has a clearly documented intended route.

## Deferred Ideas

- ScriptableObject stage assets
- Undo full turn
- Attack animation
- Skills or weapon types
- Unit portraits
- Save data
- Audio
- Controller support
