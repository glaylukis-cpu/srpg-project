# Puzzle SRPG Prototype v0.2

Unity 2D short puzzle-SRPG prototype.

The project is built around an empty-scene runtime setup. Press Play in an empty scene and the core managers, camera, UI, grid, stage loader, stage manager, terrain, and units are created automatically.

## Environment

- Unity Editor: 6000.4.9f1
- Input: legacy Input Manager
- UI: built-in `UnityEngine.UI.Text`
- Stage data: C# data builders in `Assets/Scripts/Stage/StageData.cs`

## Main Features

- Title screen
- Stage select screen
- Polished panel-based stage select layout with highlighted stage rows, chapter card, selected-stage detail, decorated header, and controls footer
- Six-stage progression
- Runtime 8x8 grid generation
- StageData-driven unit and terrain setup
- Obstacles and Goal tiles
- Player move, attack, wait, and movement undo
- R key current-stage restart
- Enemy AI types: Aggressive, WeakTarget, Stationary, Guardian
- Enemy info selection and AI prediction
- Attack preview
- Enemy threat and selected-enemy range display
- HP display above units
- Damage popup
- Runtime-generated placeholder pixel unit sprites
- Simple attack pulse, hit flash, and KO popup
- Battle log
- Victory conditions: DefeatAllEnemies, ReachGoal
- Defeat conditions: AllPlayersDefeated, TurnLimitExceeded
- Result summary with rating, turn, survivors, and HP total

## Controls

Title:

```text
Enter: Stage Select
```

Stage Select:

```text
Up / Down: Select
Enter: Start
1-6: Quick Start
Esc / Backspace: Title
```

Battle:

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
Enter: Next / Retry
```

## Stage Order

1. Opening Formation - Intro
2. Crossfire Lanes - Easy+
3. Goal Under Pressure - Medium
4. Guardian Split - Hard
5. Last Turn Breakthrough - Hard+
6. Final Escape - Final

## Verification

Compile check:

```powershell
.\.dotnet-sdk\dotnet.exe build Verify\CompileCheck.csproj --nologo --no-restore
```

Unity batchmode compile:

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.com" -batchmode -quit -nographics -projectPath "<project path>" -logFile "<project path>\UnityBatchmode.log"
```

Current v0.2 status:

- Dotnet compile: passing
- Unity batchmode compile: passing
- StageData verification: passing
- Six-stage light solvability check: passing
- Editor play full clear: confirmed by user

See `Docs/CurrentPrototypeStatus.md`, `Docs/SixStagePlaytestRecord.md`, `Docs/v0.2StateLock.md`, and `Docs/VisualDirection2DHD.md` for the current state and visual direction notes.

## Title Background Replacement

The title screen background is loaded from:

```text
Assets/Resources/Title/TitleBackground.png
```

To replace the title screen later, overwrite that PNG with another image using the same file name. The UI loads it at runtime through `Resources.Load<Texture2D>("Title/TitleBackground")`.
