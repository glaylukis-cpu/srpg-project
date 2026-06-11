# Controls Memo

Last updated: 2026-06-05

## Title

```text
Enter: Open Stage Select
```

## Stage Select

```text
Up / Down: Change selected stage
Enter: Start selected stage
1-6: Quick start stage
Esc / Backspace: Return to title
```

The selected stage detail panel shows:

- Stage number
- DisplayName
- ThemeName
- DifficultyLabel
- Description
- Grid size
- Objective
- Turn limit
- Player / enemy count
- Obstacle / Goal count

## Battle

```text
Click Ally: Select player unit
Hover Enemy: Show attack preview while an ally is selected
Click Enemy: Show info when no ally is selected / attack when an ally is selected
Click Blue Tile: Move
W: Wait / Confirm action
U: Undo move before attack or wait confirmation
R: Restart current stage
Esc / S: Open Stage Select
Space: Toggle global enemy threat / selected enemy range
Enter: Next stage after Victory / Retry after Defeat
M: Mute / Unmute temporary audio
```

## Important Flow Notes

- Moving does not immediately end the unit action.
- After moving, the same unit stays selected.
- After moving, the unit can attack, wait, or undo the move.
- After attacking or waiting, the unit becomes acted and cannot act again that turn.
- If all player units have acted, EnemyTurn begins.
- ReachGoal stages end immediately when a living player unit reaches a Goal tile.
- Stage Select can be opened during battle with Esc or S. Starting a stage from Stage Select reloads that stage from its initial state.
- Temporary v0.5 audio uses generated placeholder clips. `M` is a debug mute toggle and is not required for normal play.
