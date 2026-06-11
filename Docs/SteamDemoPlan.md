# Steam Demo Plan

## v0.5 Demo Candidate Status Update

Date: 2026-06-07

Current state:

- Title, Stage Select, Battle, Result, six-stage flow, audio feedback, Windows build script, and Title Options screen are implemented.
- Title Options screen was checked visibly in Unity Editor and confirmed by the user as operating without issue.
- dotnet compile after the Options confirmation passed with 0 warnings and 0 errors.
- Unity batchmode compile after the Options confirmation passed.
- Fresh Windows build after the Options confirmation passed.
- Hidden Windows player smoke after the Options rebuild reached Title screen.

Still needed before v0.5 lock:

- Visible Windows build check for Title Options, Title EXIT, audio levels, Stage 1 flow, and Result Summary readability.
- Visible 1280x720 / 1600x900 / 1920x1080 layout check.

Do not expand scope before v0.5 lock:

- No new battle mechanics.
- No StageData balance changes unless a blocking bug is found.
- No enemy AI changes.
- No new progression systems, save system, equipment, experience, or skill trees.

## v0.5 Final Visible Build Update

Date: 2026-06-11

Visible build confirmed by user:

- Title menu operation: Up / Down, Enter, Options, and Exit.
- Options screen: layout intact, return to Title works, Title menu remains usable afterward.
- Stage Select: Up / Down, Enter, 1-6 Quick Start, Esc / Backspace.
- Stage 1 Battle: select, move, attack, attack motion, SE, Damage Popup, W Wait / Confirm, U Undo, R Restart, Space Threat, Esc / S.
- Result: Victory, Result Summary, Enter to Stage 2.
- Audio: Cursor / Confirm / Cancel / Attack / Hit / KO / Victory SE confirmed and not too loud.

Issue and fix:

- Title operation-bar text was missing in the visible build.
- A minimal UI-only fix was applied in `BattleUI.cs`.
- Fresh dotnet compile, Unity batchmode compile, Windows build, and hidden Title smoke passed after the fix.

Remaining before v0.5 lock:

- Visually confirm the Title operation-bar text in the freshly rebuilt executable.
- Perform visible 1280x720 / 1600x900 / 1920x1080 layout checks.
- Optional: one full six-stage visible build playthrough.

## v0.5 Options Display Update

Date: 2026-06-11

Implemented:

- Resolution switching in Title Options.
- Windowed / Fullscreen switching in Title Options.
- Supported resolution presets:
  - 1280x720
  - 1600x900
  - 1920x1080

Verification:

- dotnet compile passed.
- Unity batchmode compile passed.
- Windows build passed.
- Hidden Windows smoke reached Title screen.

Remaining:

- Visible confirmation that Resolution switching works.
- Visible confirmation that Windowed / Fullscreen switching works.
- Visible layout check at 1280x720, 1600x900, and 1920x1080.

## v0.5 Audio / Build Prep Update

Date: 2026-06-05

Status:
- Temporary runtime-generated audio has been added.
- `AudioManager` handles Title / Stage Select / Battle BGM and core SE.
- Audio can be muted with `M`.
- Windows build script has been added at `Assets/Editor/BuildDemo.cs`.
- Detailed checklist: `Docs/v0.5AudioBuildChecklist.md`.

Implemented v0.5 items:
- Title BGM
- Stage Select BGM
- Battle BGM
- Title Options screen for temporary audio settings
- Cursor / Confirm / Cancel SE
- Attack / Hit / KO SE
- Victory / Defeat SE
- Restart / Undo SE
- BGM switching across Title, Stage Select, Battle, Victory / Defeat, and ALL CLEAR
- Build method: `SRPG.EditorTools.BuildDemo.BuildWindows`
- Target output: `Builds/Windows/FinalEscapeTacticsDemo.exe`

Still required before Demo Candidate:
- Listen to placeholder audio in Unity Editor and tune volume if needed.
- Run Windows build and smoke test. Hidden smoke passed on 2026-06-07.
- Check 1280x720, 1600x900, and 1920x1080 UI layout visually.
- Confirm no major Console errors in build.
- Confirm title background art usage rights before public Steam materials.

## v0.5 Windows Build Verification

Date: 2026-06-07

Results:

- dotnet compile: Pass, 0 warnings, 0 errors.
- Unity batchmode compile: Pass, exit code 0.
- Windows build: Pass.
- Output: `Builds/Windows/FinalEscapeTacticsDemo.exe`.
- Build script: `SRPG.EditorTools.BuildDemo.BuildWindows`.
- Hidden Windows player smoke: Pass, reached Title screen.
- Hidden launch smoke reached Title at 1280x720, 1600x900, and 1920x1080.
- Build log confirmed Resources inclusion for `TitleBackground.png` and unit sprites.
- `FindObjectOfType<T>()` deprecation warning was resolved after build verification; cleanup compile/build passed.

Not yet verified:

- Visible Windows build input flow from Title to Stage Select to Battle.
- Audible BGM / SE behavior in the build.
- Visible UI layout at common 16:9 resolutions.
- Title EXIT closing the executable.
- Victory / Defeat / Result Summary in the build.

Next:

- Launch the Windows build visibly and run a short Stage 1 interaction check.
- Capture screenshots for Title, Stage Select, Battle, Attack Preview, Threat ON, and Result Summary.
- Replace placeholder generated audio only after the build flow is stable.

作成日: 2026-06-04  
対象: Unity 2D short tactical RPG prototype / Final Escape Tactics

この文書は、現在のプロトタイプをSteam向け短編デモ候補へ近づけるための整理メモです。  
目的は「今すぐ必要なもの」「あると印象が上がるもの」「今は後回しにするもの」を切り分け、短いデモとして完成へ寄せることです。

## Current Completion

### Completed

- Title -> Stage Select -> Battle -> Result の基本導線がある
- Stage Selectから6ステージを選択して開始できる
- Stage SelectからTitleへ戻れる
- Battle中EscでStage Selectへ戻れる
- Victory後Enterで次ステージへ進める
- Defeat後Enterでリトライできる
- Final Stage clear後にALL CLEARを表示できる
- Rキーで現在ステージを即Restartできる
- Uキーで移動Undoできる
- WキーでWait / Confirmできる
- 6ステージ構成がStageDataで管理されている
- StageDataにDisplayName / ThemeName / DifficultyLabel / Description / VictoryCondition / DefeatConditions / TurnLimitがある
- 6ステージの現在順は以下:
  - Stage 1: Opening Formation
  - Stage 2: Crossfire Lanes
  - Stage 3: Goal Under Pressure
  - Stage 4: Guardian Split
  - Stage 5: Last Turn Breakthrough
  - Stage 6: Final Escape
- 移動 / 攻撃 / 待機 / ダメージ / 撃破 / 勝敗判定が実装済み
- 移動後に攻撃または待機で行動確定するSRPGらしい流れがある
- 攻撃プレビュー、HP表示、ダメージポップアップ、Battle Logがある
- Result SummaryとCurrent Session Bestがある
- 敵AIタイプがある:
  - Aggressive
  - WeakTarget
  - Stationary
  - Guardian
- 勝利条件がある:
  - DefeatAllEnemies
  - ReachGoal
- 敗北条件がある:
  - AllPlayersDefeated
  - TurnLimitExceeded
- 敵情報表示とAI行動予測がある
- MoveRange / AttackRange / EnemyThreat表示がある
- Title / Stage Select / BattleのUIトーンが暗色 + 金枠 + 青系ハイライトで統一されてきている
- Unity Editorで実画面プレイ確認済み
- dotnet compile / Unity batchmode compileは直近で成功済み

### Needs Polish

- 音がないため、攻撃・被弾・決定・Victory/Defeatの手応えが弱い
- 初見プレイヤーがThreat表示、Undo、Wait / Confirmに気づける導線がまだ弱い
- ステージ説明はあるが、最初の1分で操作目的を理解できるか追加確認が必要
- 戦闘画面は改善済みだが、ビルド解像度での文字崩れ確認が必要
- Stage 5 / Stage 6の難度体感は再確認したい
- Windowsビルドでフォント、画面比率、入力、終了処理の確認が必要
- タイトル背景画像など、Steam掲載時の権利・使用範囲を確認する必要がある

### Deferred

- セーブ / ロード
- 装備システム
- 経験値 / レベルアップ
- スキルツリー
- 複雑な会話イベント
- 多数ステージ追加
- 本格的な2DHDアート全差し替え
- 実績
- Steam API連携
- オンライン機能
- コントローラー完全対応
- 多言語対応

## Demo Scope Classification

### A. Demo Must-Have

Steamデモとして最低限必要なもの。

- Title Screen
- Stage Select
- 6ステージ通しプレイ
- 各ステージの勝敗条件が明確に分かること
- リトライ導線
- Restart導線
- Undo導線
- 操作説明
- Victory / Defeat / ALL CLEAR表示
- Result Summary
- Current Session Best
- 重大バグなし
- Windowsビルド作成
- ビルド版での操作確認
- 主要解像度でのUI確認
- ゲーム終了導線

### B. Demo Should-Have

あると印象がかなり良くなるもの。

- 仮BGM
- 仮SE
- 決定 / キャンセル / カーソル移動音
- 攻撃 / 被弾 / 撃破 / Victory / Defeat音
- 軽いチュートリアル導線の強化
- Stage Introの文言最終調整
- 戦闘演出の最終ポリッシュ
- UIの最終余白調整
- スクショ映えする画面作り
- Steamページ用スクショ候補選定
- 短いプレイ動画候補の選定
- 簡易クレジット
- README / 操作説明メモ更新

### C. Post-Demo / Later

今は後回しにするもの。

- セーブデータ
- Steam実績
- 装備
- 育成
- 複雑なスキル
- スキルツリー
- 多数ステージ追加
- 本格2DHDアート全差し替え
- 本格シナリオイベント
- 本格会話UI
- コントローラー完全対応
- 多言語対応
- Steam API連携
- オンライン機能

これらは魅力を増やせるが、今入れると短編デモとしての完成が遅れる。現段階では「6ステージを遊び切れる、分かりやすい、崩れない」を優先する。

## Roadmap

### v0.4: Demo Polish Base

- 見た目と操作性の最終ポリッシュ
- Title / Stage Select / BattleのUI統一確認
- 戦闘画面の可読性確認
- Stage 1〜6通しプレイ再確認
- Stage 5 / Stage 6の難度体感確認
- 操作説明の見落とし確認
- README / ControlsMemo更新
- SixStagePlaytestRecord更新

### v0.5: Audio and Build Prep

- 仮BGM追加
- 仮SE追加
- カーソル移動 / 決定 / キャンセル音追加
- 攻撃 / 被弾 / 撃破 / Victory / Defeat音追加
- Stage IntroとObjective文言の最終整理
- Result / Current Session Best表示の最終確認
- Windowsビルド作成
- ビルド版での基本操作確認
- 主要解像度でのUI確認

### Demo Candidate

- 重大バグ修正
- Consoleエラー確認
- 6ステージ最終通し確認
- Stage Selectから各ステージ開始確認
- Title / Stage Select / Battle / Resultの遷移確認
- Steamページ用スクショ候補選定
- 短いプレイ動画候補選定
- 配布ビルド作成
- ZIP化または配布形式の確認
- 最終操作説明とREADME更新

## Do Not Do Now

短編デモを完成させるため、以下は現段階では入れない。

- 装備システム
- 経験値 / レベルアップ
- スキルツリー
- セーブ / ロード
- 複雑な会話イベント
- 多数ステージ追加
- 本格的な2DHDアート全差し替え
- オンライン機能
- 実績
- Steam API連携
- コントローラー完全対応
- 多言語対応

理由: これらは製品版の厚みには効くが、今入れるとデモの安定化、操作導線、ビルド確認が遅れる。

## One-Week Priority Tasks

### Priority 1

- Stage 1〜6通しプレイ
- バグ記録
- 操作導線確認
- Victory / Defeat / ALL CLEAR確認
- Result Summary確認
- R Restart確認
- U Undo確認
- Battle中Esc -> Stage Select確認
- Stage Select -> Title確認

### Priority 2

- 戦闘画面の可読性微調整
- Title / Stage Select / BattleのUI統一確認
- StageDataのDisplayName / ThemeName / DifficultyLabel / Description最終確認
- Stage 5 / Stage 6の難度体感確認
- 初見向け操作説明の不足確認

### Priority 3

- 仮BGM追加
- 仮SE追加
- 攻撃 / 被弾 / 撃破 / Victory / Defeatの音追加
- README更新
- ControlsMemo更新
- Steamデモ向け説明文の下書き

### Priority 4

- Windowsビルド作成
- ビルド版で操作確認
- 画面解像度確認
- スクショ候補選定
- 短いプレイ動画候補選定

## Demo Must-Have Checklist

- [ ] Title -> Stage Select -> Battle の導線確認
- [ ] Stage Select -> Title の導線確認
- [ ] Battle中Esc -> Stage Select の導線確認
- [ ] Stage SelectからStage 1開始
- [ ] Stage SelectからStage 2開始
- [ ] Stage SelectからStage 3開始
- [ ] Stage SelectからStage 4開始
- [ ] Stage SelectからStage 5開始
- [ ] Stage SelectからStage 6開始
- [ ] Stage 1〜6通しプレイ
- [ ] Enterでステージ開始
- [ ] 1〜6でQuick Start
- [ ] RでRestart
- [ ] UでUndo
- [ ] WでWait / Confirm
- [ ] SpaceでEnemy Threat切替
- [ ] 攻撃プレビュー確認
- [ ] HP表示確認
- [ ] ダメージポップアップ確認
- [ ] Battle Log確認
- [ ] Victory確認
- [ ] Defeat確認
- [ ] ALL CLEAR確認
- [ ] Result Summary確認
- [ ] Current Session Best確認
- [ ] Windowsビルド作成
- [ ] ビルド版で操作確認
- [ ] 主要解像度でUI確認
- [ ] 終了導線確認
- [ ] 重大エラーなし

## Demo Should-Have Checklist

- [ ] タイトルBGM追加
- [ ] 戦闘BGM追加
- [ ] Stage Select BGM追加
- [ ] カーソル移動SE追加
- [ ] 決定SE追加
- [ ] キャンセルSE追加
- [ ] 攻撃SE追加
- [ ] 被弾SE追加
- [ ] 撃破SE追加
- [ ] Victory / Defeat SE追加
- [ ] 攻撃演出確認
- [ ] 被弾演出確認
- [ ] 撃破演出確認
- [ ] Stage Intro文言確認
- [ ] 操作説明確認
- [ ] スクショ候補作成
- [ ] README更新
- [ ] ControlsMemo更新
- [ ] 簡易クレジット追加

## Later Checklist

- [ ] セーブ / ロード
- [ ] Steam実績
- [ ] Steam API連携
- [ ] 装備
- [ ] 成長要素
- [ ] スキルツリー
- [ ] 本格アート差し替え
- [ ] 本格会話イベント
- [ ] コントローラー完全対応
- [ ] 多言語対応
- [ ] 追加ステージ大量制作

## Risks and Mitigations

| Risk | Impact | Mitigation |
| --- | --- | --- |
| 音がないと画面が良くても寂しく感じる | デモの印象が弱くなる | v0.5で仮BGM/SEを最優先で入れる |
| 戦闘テンポが遅い可能性 | 詰将棋感より待ち時間が目立つ | 6ステージ通しで敵ターン待ち時間と演出時間を計測する |
| 初見プレイヤーがUndoやThreat表示に気づかない | デモ体験が理不尽に感じられる | Controls、Stage Intro、Stage 1説明でU/Spaceを明示する |
| Wait / Confirmの意味が分かりにくい | 移動後に詰まる可能性 | 操作説明で「Move -> Attack or W Confirm」を短く示す |
| Stage 5 / Stage 6が難しすぎる可能性 | 途中離脱の原因になる | 通しプレイ記録を見てターン制限・敵HPをStageDataだけで調整する |
| Stage 1〜2が説明不足の可能性 | 初見でルール理解が遅れる | Stage IntroとDescriptionを短く具体的にする |
| ビルド版でフォントや解像度が崩れる | Steamデモとして見栄えが落ちる | Windowsビルド後に16:9中心で複数解像度確認する |
| タイトル背景画像の権利・使用範囲が未確認 | Steam掲載で問題になる | 使用可能な自作/権利確認済み素材へ差し替えるか、権利情報を明文化する |
| Unity Editorでは動くがビルドで入力挙動が違う | デモ配布後に操作不能になる | ビルド版でTitle/Stage/Battle全入力を確認する |
| Console警告が残る | 品質不安につながる | Demo CandidateでError/Exceptionをゼロにする。非致命Warningは記録する |

## Steam Page Material Candidates

画像作成はまだ行わない。候補だけ整理する。

- Title Screen
- Stage Select Screen
- Stage 4: Guardian Splitの戦闘画面
- Stage 5: Last Turn Breakthroughの戦闘画面
- Stage 6: Final Escapeの戦闘画面
- Goal到達直前の画面
- Victory Result Summary
- Attack Preview表示中の画面
- Enemy Threat ON表示中の画面
- 敵情報 / AI Prediction表示中の画面
- Damage Popupが出ている攻撃直後の画面
- ALL CLEAR表示

## Definition of Demo Candidate Ready

- [ ] 6ステージが通しで最後まで遊べる
- [ ] 重大なConsole Errorがない
- [ ] WindowsビルドでTitle -> Stage Select -> Battle -> Resultが動く
- [ ] Restart / Undo / Threat / Esc遷移がビルド版で動く
- [ ] UIが1280x720以上の16:9で破綻しない
- [ ] Title背景素材の使用範囲が確認済み
- [ ] README / 操作説明が最新
- [ ] Steamページ用スクショ候補が最低5枚ある
- [ ] 配布ビルド名とバージョンが整理されている

## v0.4 Checklist Execution Record

- Date: 2026-06-04
- Result: v0.4 Demo Polish Base checklist recorded.
- Checklist file: `Docs/v0.4DemoPolishChecklist.md`
- Dotnet compile: Pass, 0 warnings, 0 errors.
- Unity batchmode compile: Pass, return code 0.
- Fresh automated transition verification: Pass.
- Fresh automated all-stage-load verification: Pass.
- Transition verification marker: `V04_DEMO_POLISH_TRANSITION_VERIFICATION_PASS`.
- Transition verification log: `Logs/v0.4TransitionVerification.log`.
- Fresh v0.4 full editor playthrough: Confirmed by user on 2026-06-05.
- Gameplay logic changes: None.
- StageData changes: None.
- Playtest baseline: Existing six-stage full clear in `Docs/SixStagePlaytestRecord.md`.
- Remaining v0.4 manual check: detailed Stage 5 / Stage 6 feel notes, screenshot candidate capture, and build-version play confirmation.
