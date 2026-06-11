# 6 Stage Playtest Record

This record reflects the current stage order:
1 -> old Stage 1, 2 -> old Stage 4, 3 -> old Stage 6, 4 -> old Stage 5, 5 -> old Stage 2, 6 -> old Stage 3.

## Build / Session

- Date: 2026-06-04
- Unity Version: 6000.4.9f1
- Build / Commit: Local prototype
- Tester: User full clear confirmed
- Input Device: Mouse and keyboard
- Notes: All 6 stages were cleared in editor play. One full playthrough clear was confirmed after the StageData-only balance adjustments. Clear Evaluation logs below were collected with lightweight StageData route rechecks. Result UI now displays Rating, Turn, Survivors, and HP Total. Screen transition paths were checked for the v0.2 lock.

## Global Checks

- Controls readable: Pass
- Objective readable: Pass
- Threat display useful: Pass
- Enemy selected range display useful: Pass
- Attack preview clear: Pass
- Battle log readable: Pass
- HP display readable: Pass
- Damage popup readable: Pass
- Restart / undo flow comfortable: Pass
- Title to Stage Select flow: Pass
- Stage Select to battle flow: Pass
- Battle to Stage Select flow with Esc / S: Pass

## Stage 1 - Opening Formation

- Theme: Basic Movement and Attacks
- Difficulty: Intro
- Objective: Defeat all enemies
- Turn Limit: None
- Attempts: Cleared during full run
- Cleared: Yes
- Clear Turn: 3
- Rating: CLEAR
- Player Survivors: 2
- Player HP Remaining: 9 total
- Enemy Survivors: 0
- Key Route:
  1. Use the Soldier to hold the front line.
  2. Use the Archer to attack safely from range.
  3. Confirm basic move, attack, wait, and threat display behavior.
- Main Failure Cause: Low risk stage; no consistent failure point recorded.
- Confusing Moment: None recorded.
- Balance Notes: Works as the opening tutorial stage.
- Clear Evaluation Log: Clear Evaluation: Stage 1/6 | Result: Victory | Rating: CLEAR | Turn: 3 | Limit: None | Objective: DefeatAllEnemies | Players Alive: 2 | Player HP Total: 9 | Enemies Alive: 0 | Enemy HP Total: 0

## Stage 2 - Crossfire Lanes

- Theme: Threat Display and Safe Lanes
- Difficulty: Easy+
- Objective: Defeat all enemies
- Turn Limit: 7 Turns
- Attempts: Cleared during full run
- Cleared: Yes
- Clear Turn: 4
- Rating: S
- Player Survivors: 2
- Player HP Remaining: 6 total
- Enemy Survivors: 0
- Key Route:
  1. Check enemy ranges before entering the center lanes.
  2. Use the Rogue to pressure the fixed archers.
  3. Collapse on the aggressive enemies once they leave formation.
- Main Failure Cause: Entering overlapping archer threat without checking Space display.
- Confusing Moment: None recorded after selected-enemy range display was added.
- Balance Notes: Good second stage because it teaches threat reading without being too punishing.
- Clear Evaluation Log: Clear Evaluation: Stage 2/6 | Result: Victory | Rating: S | Turn: 4 | Limit: 7 | Objective: DefeatAllEnemies | Players Alive: 2 | Player HP Total: 6 | Enemies Alive: 0 | Enemy HP Total: 0

## Stage 3 - Goal Under Pressure

- Theme: Weak Target Chase
- Difficulty: Medium
- Objective: Reach a goal tile
- Turn Limit: 8 Turns
- Attempts: Cleared during full run
- Cleared: Yes
- Clear Turn: Route check found a Turn 4 clear
- Rating: S
- Player Survivors: 2
- Player HP Remaining: 19 total
- Enemy Survivors: 4 allowed; ReachGoal stage
- Key Route:
  1. Advance behind the Knight instead of rushing the Rogue alone.
  2. Let WeakTarget enemies commit to their chase path.
  3. Send the mobile unit to the Goal once the route opens.
- Main Failure Cause: Low-HP units exposed too early to WeakTarget enemies.
- Confusing Moment: Goal route is readable after threat display.
- Balance Notes: Works as an earlier Goal stage before the final escape map.
- Clear Evaluation Log: Clear Evaluation: Stage 3/6 | Result: Victory | Rating: S | Turn: 4 | Limit: 8 | Objective: ReachGoal | Players Alive: 2 | Player HP Total: 19 | Enemies Alive: 4 | Enemy HP Total: 36

## Stage 4 - Guardian Split

- Theme: Pulling Guardians Separately
- Difficulty: Hard
- Objective: Defeat all enemies
- Turn Limit: 8 Turns
- Attempts: Cleared during full run
- Cleared: Yes
- Clear Turn: 5
- Rating: S
- Player Survivors: 3 in recheck route
- Player HP Remaining: 13 total
- Enemy Survivors: 0
- Key Route:
  1. Turn 1: A to (3,1), B to (2,4), C to (2,3), all wait.
  2. Use B and C to remove the stationary archer and soften guardians.
  3. Finish with A after B and C have traded themselves for damage.
- Main Failure Cause: Pulling both Guardian enemies before damage is ready.
- Confusing Moment: Sacrifice route is valid but visually harsh.
- Balance Notes: Strong puzzle stage. It felt too sacrificial, so the Archer and Rogue gained +1 HP and both Guardian enemies were reduced from ATK 6 to ATK 4. Recheck now clears with all 3 players alive, so the stage keeps the guardian-pull puzzle while feeling less punitive.
- Clear Evaluation Log: Clear Evaluation: Stage 4/6 | Result: Victory | Rating: S | Turn: 5 | Limit: 8 | Objective: DefeatAllEnemies | Players Alive: 3 | Player HP Total: 13 | Enemies Alive: 0 | Enemy HP Total: 0

## Stage 5 - Last Turn Breakthrough

- Theme: Route Optimization
- Difficulty: Hard+
- Objective: Defeat all enemies
- Turn Limit: 6 Turns
- Attempts: Cleared during full run
- Cleared: Yes
- Clear Turn: 5
- Rating: A
- Player Survivors: 1
- Player HP Remaining: 3 total
- Enemy Survivors: 0
- Key Route:
  1. Handle the aggressive Soldier with Player A while Player B routes toward the Guardian.
  2. Eliminate the stationary Archer without taking unnecessary damage.
  3. Spend every turn efficiently; the Knight Guardian only falls if both allies contribute enough total damage.
- Main Failure Cause: Wasting one action leaves the Knight Guardian alive.
- Confusing Moment: Damage math is strict; attack preview is important.
- Balance Notes: Good late-stage damage puzzle. The Guardian HP was reduced from 13 to 12 so the final damage check has a little more tolerance while still requiring efficient actions. Recheck improved to Turn 5, but only one player survives at 3 HP, so the stage still feels like the strictest fight.
- Clear Evaluation Log: Clear Evaluation: Stage 5/6 | Result: Victory | Rating: A | Turn: 5 | Limit: 6 | Objective: DefeatAllEnemies | Players Alive: 1 | Player HP Total: 3 | Enemies Alive: 0 | Enemy HP Total: 0

## Stage 6 - Final Escape

- Theme: Goal Route Under Fire
- Difficulty: Final
- Objective: Reach a goal tile
- Turn Limit: 7 Turns
- Attempts: Cleared during full run
- Cleared: Yes
- Clear Turn: Route check found a Turn 5 clear
- Rating: A
- Player Survivors: 1
- Player HP Remaining: 2 total
- Enemy Survivors: 3 allowed; ReachGoal stage
- Key Route:
  1. Turn 1: A to (2,2), B to (1,3), both wait.
  2. Turn 2-3: Use A and B together to remove Enemy D, then Enemy A.
  3. Let A draw fire while B reaches (7,7) for Victory.
- Main Failure Cause: Trying to wipe all enemies instead of escaping.
- Confusing Moment: A can die after enabling the escape; this is acceptable.
- Balance Notes: Good final stage because it asks the player to understand the objective, not just fight.
- Clear Evaluation Log: Clear Evaluation: Stage 6/6 | Result: Victory | Rating: A | Turn: 5 | Limit: 7 | Objective: ReachGoal | Players Alive: 1 | Player HP Total: 2 | Enemies Alive: 3 | Enemy HP Total: 17

## Overall Balance Notes

- Easiest Stage: Stage 1 - Opening Formation
- Hardest Stage: Stage 5 - Last Turn Breakthrough, because it has the strictest damage/action economy.
- Best Puzzle Moment: Stage 4 - Guardian Split, when the player realizes the guardians can be managed separately.
- Most Confusing Rule: Sacrificing a unit can still be correct if the objective permits it.
- Stage That Needs Tuning: Stage 5 remains the sharpest stage after recheck; Stage 4 now looks healthier.
- Suggested Next Change: If Stage 5 feels too punishing in live play, adjust only StageData by either adding +1 HP to Player B or reducing Stage5_Enemy_A ATK by 1.

## v0.2 Readiness Notes

- Result UI includes Rating / Turn / Survivors / HP Total.
- Six-stage light recheck passed after Result UI work.
- Current build is suitable to tag as v0.2.
- State lock note: see `Docs/v0.2StateLock.md`.
- Recommended next feature direction: v0.3 title / stage select visual polish, or a proper source-controlled validation runner.

## v0.4 Demo Polish Base Record

- Date: 2026-06-04
- Scope: Documentation and verification pass for Steam demo planning.
- Gameplay changes: None.
- StageData changes: None.
- Dotnet compile: Pass, 0 warnings, 0 errors.
- Unity batchmode compile: Pass, return code 0.
- Fresh automated transition verification: Pass.
- Fresh automated all-stage-load verification: Pass.
- Transition verification marker: `V04_DEMO_POLISH_TRANSITION_VERIFICATION_PASS`.
- Transition verification log: `Logs/v0.4TransitionVerification.log`.
- Fresh v0.4 full editor playthrough: Confirmed by user on 2026-06-05.
- Checklist note: see `Docs/v0.4DemoPolishChecklist.md`.
- Demo planning note: see `Docs/SteamDemoPlan.md`.
- Playtest carry-forward: The six-stage full clear and per-stage Clear Evaluation logs above remain the current balance baseline.
- Manual recheck still useful: detailed Stage 5 / Stage 6 feel notes, screenshot candidate capture, and build-version play confirmation.
