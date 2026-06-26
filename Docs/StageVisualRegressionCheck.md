# Stage1-6 Visual Regression Check

## Target Commit

- Visual update commit: `fc8729b Improve board visuals and rubble obstacles`
- Latest local commit at document creation: `fc8729b Improve board visuals and rubble obstacles`

## Purpose

This document records the visual regression checklist for Stage1-6 after the board visual polish.

The check focuses on:

- Board thickness, side faces, and shadow readability.
- Floor color and tile detail readability.
- Rock/rubble obstacle readability after adding `RockRubble_0` through `RockRubble_3`.
- Threat / Move / Intent / HP / unit base visibility.
- Basic interaction visibility and flow across all stages.

This document is a verification checklist and record. It does not change gameplay logic, stage data, UI behavior, or assets.

## Common Checklist

- Board thickness reads naturally as a miniature tactical board.
- Floor color is not too dark, and each tile boundary remains readable.
- Rock/rubble obstacles read as one-tile blockers.
- Rock/rubble obstacles do not look like faces, tiny pebbles, or oversized multi-tile rubble.
- Obstacles do not block Threat / Move / Intent / HP readability.
- Player and enemy bases remain visible against the floor.
- HP labels remain readable.
- Enemy Threat ON/OFF remains readable.
- AttackNow / MoveToward / Guard intent previews remain visible.
- `U: Undo Move` works.
- `Shift+U: Reset Turn` works.
- `Enter: End Turn` works.
- Stage1 Guided Tutorial Hint appears and does not block clicks.
- Battle Log / Controls / Selected / Stage Info panels are not visually broken.
- Victory / Defeat / stage transition display remains intact.

## Stage1: Opening Formation

- Visual:
  - Board: Pending
  - Obstacles: Pending
  - Units: Pending
  - UI: Pending
- Interaction:
  - Click / Move: Pending
  - Attack: Pending
  - U Undo Move: Pending
  - Shift+U Reset Turn: Pending
  - Space Enemy Threat: Pending
  - Enter End Turn: Pending
- Result:
  - Pending
- Notes:
  - Confirm Guided Tutorial Hint does not block board interaction.
  - Confirm the three center obstacles read as three blocked tiles.

## Stage2: Crossfire Lanes

- Visual:
  - Board: Pending
  - Obstacles: Pending
  - Units: Pending
  - UI: Pending
- Interaction:
  - Click / Move: Pending
  - Attack: Pending
  - U Undo Move: Pending
  - Shift+U Reset Turn: Pending
  - Space Enemy Threat: Pending
  - Enter End Turn: Pending
- Result:
  - Pending
- Notes:
  - Confirm obstacle-heavy lanes still read clearly.
  - Confirm Enemy Threat display is not buried by rubble visuals.

## Stage3

- Visual:
  - Board: Pending
  - Obstacles: Pending
  - Units: Pending
  - UI: Pending
- Interaction:
  - Click / Move: Pending
  - Attack: Pending
  - U Undo Move: Pending
  - Shift+U Reset Turn: Pending
  - Space Enemy Threat: Pending
  - Enter End Turn: Pending
- Result:
  - Pending
- Notes:
  - Confirm goal-related readability if the stage objective depends on reaching a goal.

## Stage4

- Visual:
  - Board: Pending
  - Obstacles: Pending
  - Units: Pending
  - UI: Pending
- Interaction:
  - Click / Move: Pending
  - Attack: Pending
  - U Undo Move: Pending
  - Shift+U Reset Turn: Pending
  - Space Enemy Threat: Pending
  - Enter End Turn: Pending
- Result:
  - Pending
- Notes:
  - Confirm Archer / Stationary-style intent display does not conflict with rubble visuals.
  - Confirm Guardian / Threat information remains readable if present.

## Stage5

- Visual:
  - Board: Pending
  - Obstacles: Pending
  - Units: Pending
  - UI: Pending
- Interaction:
  - Click / Move: Pending
  - Attack: Pending
  - U Undo Move: Pending
  - Shift+U Reset Turn: Pending
  - Space Enemy Threat: Pending
  - Enter End Turn: Pending
- Result:
  - Pending
- Notes:
  - Confirm dense enemy and obstacle situations still preserve tile readability.

## Stage6

- Visual:
  - Board: Pending
  - Obstacles: Pending
  - Units: Pending
  - UI: Pending
- Interaction:
  - Click / Move: Pending
  - Attack: Pending
  - U Undo Move: Pending
  - Shift+U Reset Turn: Pending
  - Space Enemy Threat: Pending
  - Enter End Turn: Pending
- Result:
  - Pending
- Notes:
  - Confirm Goal / Obstacle / Move display remains readable.
  - Confirm final-stage Result display still appears correctly.

## Focus Areas

- Stage1:
  - Guided Tutorial Hint should not interfere with clicks.
  - Center obstacles should read as three separate blocked tiles.
- Stage2:
  - Rock/rubble obstacles should remain readable in obstacle-heavy lanes.
  - Crossfire Lanes should keep Threat display readable.
- Stage4:
  - Archer / Stationary-style Intent display should not visually conflict with rubble.
- Stage6:
  - Goal / Obstacle / Move display should remain readable.

## Known Open Items

- Keep the current Undo key split:
  - `U` = Undo Move
  - `Shift+U` = Reset Turn
- Rock/rubble assets are currently `RockRubble_0` through `RockRubble_3`.
- Stage-specific terrain themes are not implemented yet.
- Final floor and obstacle color art pass can be handled later.
- Steam screenshot composition check is a separate task.

## Conclusion

- Fatal visual regression across Stage1-6: Pending manual Unity Editor check.
- Next visual issues to fix: Pending manual Unity Editor check.
- Keep the committed board visual update as-is: Pending manual Unity Editor check.
