# Commercial Readiness Audit

Date: 2026-07-10
Target: Windows / Steam short-form commercial release
Product: Final Escape Tactics
Current candidate version: 0.6.0

## Current Assessment

The project has a complete six-stage playable loop, title/options/stage-select flow,
victory and defeat handling, audio feedback, multiple 16:9 display presets, and a
repeatable Windows x86_64 build. It is suitable for a closed commercial candidate
test after the release-blocking asset and platform items below are resolved.

## Implemented In This Pass

- Versioned persistent data keys under `FinalEscapeTactics.Save.*`.
- Persistent Master/BGM/SE volume and Mute settings.
- Persistent resolution and window/fullscreen settings.
- Borderless fullscreen mode for reliable Alt+Tab behavior.
- Stage 1-only first-run progression with sequential stage unlocking.
- Persistent clear status and best result per stage.
- Locked and cleared state display in Stage Select.
- Locked-stage protection for both menu confirmation and 1-6 quick start.
- Two-step Exit confirmation with Esc/Backspace cancellation.
- Product/version/developer text on the title screen.
- Development-only input, turn, stage-load, and combat logs are suppressed in release builds.
- Release Player Settings for company, product, version, single-instance behavior,
  and disabled legacy analytics submission.
- Strict, clean Windows release build output at `Builds/Windows/Release/FinalEscapeTactics.exe`.

## Release Blockers Requiring Owner Input Or External Work

- Confirm ownership/commercial-use rights for title art, unit sprites, terrain art,
  fonts, music, sound effects, and all promotional captures.
- Replace runtime-generated placeholder BGM/SE with release-quality licensed audio,
  or explicitly approve the generated audio for release.
- Supply and configure a final Windows executable icon and Steam branding assets.
- Decide the legal publisher/developer name. `GlayL` is currently used as the
  provisional Company Name and title-screen credit.
- Prepare store copy, screenshots, capsule art, supported-language declarations,
  system requirements, content disclosures, and any required privacy statement.
- Create the Steamworks app/depot configuration and perform a SteamPipe upload test.

## Required Manual Release Validation

- Clean-machine launch on Windows 10 and Windows 11.
- Fresh-profile test: Stage 1 unlocked, Stages 2-6 locked.
- Clear/relaunch test: next stage and best record remain saved.
- Audio/display change/relaunch test at 1280x720, 1600x900, and 1920x1080.
- Full six-stage playthrough from a clean save, including ALL CLEAR.
- Victory, defeat, retry, Undo, Restart, Threat, and return-to-title regression pass.
- Keyboard and mouse checks on each supported resolution and display mode.
- Long-session check for frame pacing, memory growth, audio clipping, and Player.log errors.
- Steam overlay, launch option, uninstall/reinstall, and cloud-save policy checks.

## Important Non-Blocking Improvements

- Controller support and remappable controls.
- Accessibility pass for color-only information, text size, and screen shake/flash options.
- A dedicated credits/licenses screen and an in-build save-data reset action.
- Crash-report/support instructions and a public support contact.
- Automated play-mode tests for save migration, progression, and stage completion.

## Save Compatibility

Save schema version is `1`. New fields must use additive keys or include an explicit
migration before changing the schema version. Do not reuse existing keys for different
meanings after a public build has shipped.
