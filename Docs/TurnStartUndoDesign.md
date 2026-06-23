# Turn Start Undo Design

## 1. 目的

短編・詰将棋型SRPGとして、Player Turn中の試行錯誤を支える「ターン開始状態まで戻すUndo」を設計する。

現在は味方全員が行動しても自動でEnemy Turnへ移行せず、EnterによるEnd Turn確定を待つ。Enemy ThreatとEnemy Intent Previewを確認してから確定できるため、End Turn前に盤面をターン開始状態へ戻せることは、次の体験と相性がよい。

- 複数ユニットを動かした後でも、手順全体を考え直せる。
- AttackNow / MoveToward / Guardを確認してから配置をやり直せる。
- 複数手順の履歴管理を導入せず、詰将棋として明快なリセット操作にできる。
- Restart Stageより小さい単位で、安全に試行錯誤できる。

本設計の推奨結論は「Player Turn開始時のスナップショットを1つだけ保持し、1回のUndoでその状態へ戻す」である。複数手順Undoは対象外とする。

## 2. 現状コードの確認結果

確認済み事項と設計提案を区別するため、現状を先に整理する。

### 確認済み

- `TurnManager.BeginPlayerTurn()` がPlayer Turn開始境界になっている。
- `TurnManager.TryEndPlayerTurn()` が成功するとEnemy Turnが始まる。
- 現在のUキーは `PlayerController.TryUndoSelectedUnitMove()` を呼び、選択中ユニットの直前移動だけを戻す。
- 攻撃または待機を完了すると選択が解除され、現在の移動Undoフラグも無効になる。
- `Unit.Die()` は死亡ユニットを `GridManager.Units` とTile占有から外し、GameObjectを非アクティブにする。
- `Unit` のHP、死亡状態、GameObject active状態をまとめて戻す公開・内部APIはまだない。
- `GridManager.TryPlaceUnit()` は死亡ユニットを受け付けないため、死亡復元では状態復元と再登録の順序を明示する必要がある。
- Battle Logは `BattleUI` の内部リストで管理され、スナップショット取得・復元APIはまだない。
- Enemy Intent Previewには `Clear()` と再計算用 `Refresh()` がある。
- Victory / Defeat時は `TurnManager` がbattle ended状態になり、結果UIを表示する。

### 設計提案

- スナップショット取得と復元を `TurnSnapshotService` に集約する。
- `TurnManager` は取得・無効化のタイミングだけを管理する。
- `Unit` と `GridManager` にはUndo専用の小さな内部復元APIを追加する。
- 通常の `MoveTo()`、`Attack()`、`TakeDamage()` を逆方向に呼んで復元しない。SE、ログ、演出、勝敗判定などの副作用があるためである。
- Threat / Intentや移動・攻撃ハイライトは保存せず、復元完了後に現在状態から再計算する。

## 3. 基本仕様

### 保存

- Player Turn開始処理が完了した時点でスナップショットを保存する。
- 初回Player Turnと、Enemy Turn終了後に始まる各Player Turnで保存する。
- 次のPlayer Turn開始時は、前のスナップショットを破棄して上書きする。
- 保持するスナップショットは常に最大1つとする。
- スナップショットは現在ステージと現在ターンに紐付ける。

### 使用可能条件

次の条件をすべて満たす場合だけUndo可能とする。

- 現在フェーズがPlayer Turnである。
- End Turn確定前である。
- battle endedではない。
- Stage Select、Title、Options、Result遷移待ちではない。
- 攻撃・移動・Popupなどの演出処理中ではない。
- 現在ステージ・現在ターンと一致する有効なスナップショットがある。

味方に未行動ユニットが残っている場合、全員行動済みの場合、攻撃後、待機後のいずれでも使用できる。

### 無効化

- End Turn確定時、Enemy Turnへ切り替える直前にスナップショットを無効化する。
- Enemy Turn中はUndo不可とする。
- Victory / Defeat成立後はUndo不可とする。
- Retry、Next Stage、Stage Select、Titleへの遷移開始時にスナップショットを破棄する。
- シーン再構築やステージ再ロード時に古いUnit参照を残さない。

### Victory / Defeatの扱い

初期実装ではVictory / Defeat表示後のUndoを許可しない。結果確定後にbattle ended、BGM、評価、Result Summaryまで巻き戻すと責務と検証範囲が大きくなるためである。

このため、最後の敵を撃破した攻撃やGoal到達によって即座にVictoryが成立した場合、その操作はUndoできない。通常の敵撃破後Undoは「まだVictoryにならない敵撃破」で検証する。結果確定後Undoが必要になった場合は、別フェーズで勝敗確定タイミング自体を再設計する。

### Stage Retry / Next Stageとの関係

- Retryは現在ステージを再構築する既存仕様を優先し、Undoスナップショットは破棄する。
- Next Stageでは旧ステージのスナップショットを破棄し、新ステージの最初のPlayer Turnで新規保存する。
- StageManagerのステージ番号、解放状態、Session BestなどはUndo対象にしない。

## 4. Undo対象に含める状態

### Unitごとの動的状態

- Unit参照
- GridPosition
- WorldPosition
- CurrentHp
- MaxHp（復元値というより同一Unit確認用の整合性情報）
- IsDead
- GameObject active状態
- HasActed
- 選択状態
- 表示更新に必要な行動済み状態
- Tile占有再構築に必要な座標

Faction、UnitType、EnemyAIType、MovePower、AttackPower、AttackRangeはターン中に変化しない前提の識別・整合性確認情報として保持してよい。ただし初期実装では書き戻さず、スナップショット対象Unitが同一であることの検証に使う。

### Grid状態

- 全TileのOccupant
- GridManagerのUnit登録状態
- 死亡によってリストから外れたUnitを再登録するための情報

Tile占有はUnitSnapshotから再構築する。TileごとのOccupantを別々に保存して二重の正本を作らない。正本はUnitSnapshotのGridPositionとする。

### Turn状態

- TurnNumber
- TurnPhase
- スナップショットの有効フラグ
- ステージ識別子
- ターン識別子

正常なスナップショットのTurnPhaseはPlayerTurnである。復元時はPlayerTurnへ戻すが、敵ターンや別ターンから強制的に戻す用途には使わない。

### PlayerController状態

- SelectedUnit
- Enemy Threat ON/OFF
- 行動途中の移動Undoフラグ
- 行動途中の移動前座標

推奨するターン開始スナップショットではSelectedUnitは原則null、移動Undo状態はfalseになる。復元後も選択解除を標準とし、Threat ON/OFFだけを復元する。選択状態を厳密保存する場合でも、死亡・別ステージ参照の検証が必要である。

### UI状態

- Battle Logの表示エントリ
- Turn / Phase表示
- Selected Unit表示
- Enemy Threat ON/OFF表示
- Result表示が出ていないこと

移動範囲、攻撃範囲、Enemy Threatタイル、Enemy Intent PreviewのGameObjectは保存しない。復元後にクリアして再計算する。

### Battle Result状態

- BattleResult
- battle ended状態
- Result Summary表示状態

初期実装でUndo可能なスナップショットは `BattleResult.None` かつbattle endedでない状態に限定する。結果表示後の復元は拒否するため、これらは通常の書き戻し対象ではなく有効性検証の不変条件として扱う。

## 5. Undo対象に含めない状態

- StageDataそのもの
- StageManagerのステージ進行状態
- ステージ解放状態
- Current Session Best
- カメラ位置、Orthographic Size、背景設定
- 入力割り当て、Options設定、音量設定
- 生成済みSprite、Texture、Materialなどの静的表示資産
- BoardProjectionの設定
- UnitType、Faction、EnemyAITypeなどターン中に変化しない定義値の書き戻し
- 攻撃Coroutine、点滅Coroutine、移動Coroutineの途中状態
- DamagePopup、KO Popup、斬撃、矢、Hit Effectなどの短命オブジェクト
- AudioSourceの再生位置
- Enemy Intent Previewの生成済み表示オブジェクト
- 移動・攻撃・Threatハイライトの生成済み表示状態

短命オブジェクトは保存せず、Undo受付条件で演出中を除外し、復元開始時に残留物を消去する。

## 6. 推奨クラス設計

### TurnStartSnapshot

Player Turn開始状態を表す不変データ。

```text
TurnStartSnapshot
- StageId
- TurnNumber
- TurnPhase
- Units: IReadOnlyList<UnitSnapshot>
- BattleLog: BattleLogSnapshot
- EnemyThreatVisible
- SelectedUnitReference（初期実装では通常null）
- BattleResult
- WasBattleEnded
```

### UnitSnapshot

```text
UnitSnapshot
- UnitReference
- GridPosition
- WorldPosition
- CurrentHp
- MaxHp
- IsDead
- IsActive
- HasActed
- WasSelected
- Faction（検証用）
- UnitType（検証用）
- EnemyAIType（検証用）
```

GridPositionを論理位置の正本とする。WorldPositionも保存するが、斜め俯瞰盤面では復元時に `BoardProjection.GetUnitWorldPosition()` から再計算し、保存値は検証または特殊オフセット用のフォールバックとして扱う方が座標ずれを防ぎやすい。

### BattleLogSnapshot

```text
BattleLogSnapshot
- Entries: IReadOnlyList<string>
```

フォーマット前のイベント文字列ではなく、画面に表示している整形済みエントリを保存する。復元後にUndo通知を追加する場合でも、まずスナップショットを完全復元してから通知を1件追加する。

### TurnSnapshotService

推奨責務:

- Player Turn開始時のCapture
- Undo可能条件の判定
- Restoreの実行
- End Turn / Stage遷移時のInvalidate
- 現在ステージ・ターンとの整合性検証
- 二重Restore防止

MonoBehaviourにする必要がなければ通常クラスとして `TurnManager` が所有する。演出消去やPlayerController連携が必要な場合は、シーン内サービスとして構成してもよい。

### 必要になる内部復元API案

```text
Unit.RestoreState(UnitSnapshot snapshot)
GridManager.ClearAllOccupantsForRestore()
GridManager.RegisterRestoredUnit(Unit unit)
GridManager.RebuildOccupancy(IEnumerable<UnitSnapshot> units)
BattleUI.CaptureBattleLog()
BattleUI.RestoreBattleLog(BattleLogSnapshot snapshot)
PlayerController.PrepareForTurnRestore()
PlayerController.RefreshAfterTurnRestore(bool enemyThreatVisible)
TurnManager.RestorePlayerTurnState(TurnStartSnapshot snapshot)
```

これらは副作用のある通常ゲーム操作APIと分離し、`internal` など必要最小限の公開範囲にする。

## 7. Capture手順案

推奨タイミングは `BeginPlayerTurn()` 内で、UnitのHasActedリセットと敗北判定が完了し、battle endedでないことを確認した直後である。

1. 現在フェーズがPlayerTurnであることを確認する。
2. battle endedでないことを確認する。
3. 現在ステージ識別子とTurnNumberを取得する。
4. GridManagerが保持する全Unitを列挙する。
5. Unitごとの動的状態をUnitSnapshotへ保存する。
6. Battle Logエントリを保存する。
7. Threat ON/OFFと選択状態を保存する。
8. 既存スナップショットを新しいTurnStartSnapshotで置き換える。

死亡Unitは通常Player Turn開始時点ではGridManagerリストに存在しない可能性がある。前ターンのEnemy Turnで死亡したUnitを復元対象にしないため、これは正しい。Player Turn中に死亡したUnitは、ターン開始時のUnitSnapshot参照が残るため復活可能になる。

## 8. Restore手順案

順序を固定し、中間状態で占有衝突やUI再描画が起きないようにする。

1. Undo可能条件とステージ・ターン一致を検証する。
2. 入力を一時ロックし、同じフレームのクリック・W・Enter・Spaceを無視する。
3. 選択中Unitを解除する。
4. Player移動・攻撃ハイライト、Enemy Threat、Enemy Intent Preview、Attack Previewを全消去する。
5. 残留するDamagePopup、矢、斬撃、点滅などの一時演出を消去する。
6. 全TileのOccupantをクリアする。
7. スナップショット内の全Unitを一度復元可能なactive状態へ戻す。
8. UnitのIsDead、CurrentHp、HasActedなどを副作用なしで復元する。
9. GridPositionを復元し、WorldPositionとsortingOrderを投影座標から更新する。
10. UnitをGridManagerへ再登録する。
11. 生存Unitだけを対象にTile占有を再構築する。
12. GameObject active状態を最終確定する。
13. HP表示、台座、選択リング、行動済み色を更新する。
14. TurnNumber、TurnPhase、battle result不変条件を反映する。
15. Battle Logをスナップショットへ戻す。
16. Turn / Selected / ResultなどBattle UIを更新する。
17. 保存されていたEnemy Threat ON/OFF状態を戻す。
18. ThreatがONならタイルThreatとEnemy Intent Previewを現在盤面から再計算する。
19. 入力ロックを解除する。
20. 必要ならUndo完了ログを追加する。

復元に失敗した場合は入力ロックを解除し、不完全な盤面で続行させない。開発ビルドではErrorを出し、製品ビルドでは安全側としてStage Retryを案内する設計を検討する。

## 9. 既存Undoとの関係

現在の移動Undoは、選択中Unitが移動した直後かつ行動確定前だけ有効である。ターン開始Undoとは用途と復元範囲が異なる。

### 段階導入案

- 初期検証中: Uは既存移動Undoを維持し、Shift+Uをターン開始Undoに割り当てる。
- 安定後: Uをターン開始Undoへ変更し、既存移動Undoは廃止または別キーへ移す。
- UI文言は機能移行と同じタイミングで更新する。

### 推奨最終仕様

`U: Reset Player Turn` を推奨する。短編詰将棋では、直前移動だけ戻るUとターン全体が戻るUを状況で切り替えるより、常にターン開始へ戻る方が予測可能である。

移行期間中に両方を同じUへ割り当てる案は避ける。選択状態によって結果が変わり、ユーザーがどこまで戻るか予測しにくいためである。

## 10. 入力仕様案

- Player Turn中のみ有効。
- End Turn前のみ有効。
- 味方の行動数に関係なく使用可能。
- 移動後、攻撃後、待機後、複数味方行動後に使用可能。
- 攻撃・Popupなどの演出中は入力を無視する。
- Enemy Turn中は無効。
- Victory / Defeat後は無効。
- Stage遷移待ち、Result表示中、Menu表示中は無効。
- 同一フレームでEnd TurnとUndoが同時成立しないよう、入力優先順位を固定する。

推奨優先順位は、Result / Menu処理、演出ロック、Undo、End Turn、その他Battle入力の順である。

## 11. ログ仕様案

Battle Logはターン開始状態へ復元する。その後にUndo操作を明示するログを追加する。

推奨文言:

```text
Player turn restored.
```

この方式では、復元前に発生した移動・攻撃・撃破ログは消え、Undoを行った事実だけが残る。連続してUndoした場合も、毎回スナップショットのログへ戻してから同じ通知を追加するため、通知が無制限に蓄積しない。

完全にターン開始時のログへ戻すことを優先する場合は、Undo通知をBattle Logではなく一時Toastに表示する代替案もある。Phase 1では既存UIを再利用できるBattle Log通知を推奨する。

## 12. リスクと対策

### 死亡Unitの復活

リスク: `Die()` によりGridManagerから削除され、GameObjectとHP表示が非アクティブになる。

対策: Unit参照をスナップショットに保持し、IsDeadを戻してからGridManagerへ再登録する専用APIを使う。HP表示の子GameObjectも再表示する。

### Tile占有の不整合

リスク: Unitを個別に移動して戻すと、一時的な占有衝突や古いOccupantが残る。

対策: 全Tile占有を先にクリアし、全Unit状態を復元した後で生存Unitの占有を一括再構築する。重複座標を検出した場合はRestore失敗とする。

### HP・表示更新漏れ

リスク: privateなCurrentHpを戻してもHP Text、背景、active状態が更新されない。

対策: `Unit.RestoreState()` 内でHP、死亡、表示更新を不可分に処理する。

### HasActed表示更新漏れ

リスク: 値だけ戻し、Sprite色や台座が暗いまま残る。

対策: 既存 `SetHasActed()` または副作用を限定した復元APIから `RefreshColor()` を必ず呼ぶ。

### Victory / Defeat巻き戻し

リスク: Result表示、BGM、評価、Stage遷移入力まで巻き戻す必要が生じる。

対策: 初期実装ではbattle ended後のUndoを禁止する。結果巻き戻しは別設計に分離する。

### Enemy Intentの残留

リスク: 復元前のUnitを指す矢印・リングが残る。

対策: Restore開始時に `EnemyIntentPreview.Clear()` を呼び、全復元後にThreat ONの場合だけ再計算する。

### Battle Log復元漏れ

リスク: 盤面は戻ったのに攻撃・撃破ログだけ残る。

対策: 表示済みログエントリのコピー取得・一括復元APIをBattleUIに追加する。

### 既存移動Undoとの競合

リスク: Uの結果が選択状態によって変わる。

対策: 検証期間は別キーに分け、最終的にはUをターン開始Undoへ統一する。

### 演出中Undo

リスク: Coroutineが復元後に古い対象へ処理を続ける。

対策: `PlayerController.IsAnimating` 中はUndoを拒否する。将来、演出キャンセルを実装する場合も、全Coroutineの停止契約を別途定義する。

### 古いステージ参照

リスク: Stage RetryやNext Stage後に旧Unit参照を復元する。

対策: Snapshotへステージ識別子を持たせ、遷移開始時にInvalidateする。現在ステージと一致しないSnapshotは使用しない。

## 13. 段階的実装計画

### Phase 1: コアスナップショット

- `TurnStartSnapshot`、`UnitSnapshot`、`TurnSnapshotService` を追加する。
- Player Turn開始時に1つのSnapshotを保存する。
- End Turn時とStage遷移時にInvalidateする。
- GridPosition、HP、IsDead、active、HasActed、Grid登録、Tile占有を復元する。
- 演出中・battle ended・Enemy Turn中のUndoを拒否する。

### Phase 2: UIと予測表示

- Battle LogのCapture / Restoreを追加する。
- 選択、移動・攻撃ハイライト、Attack Previewをクリアする。
- Enemy Threat ON/OFFを復元する。
- Enemy ThreatとEnemy Intent Previewを保存せず再計算する。
- Undo完了ログを追加する。

### Phase 3: 安定性検証

- 移動、攻撃、待機、非最終敵撃破、複数味方行動後を検証する。
- 死亡Unit復活、HP表示、台座、sortingOrder、Tile占有を確認する。
- Stage 1からStage 6まで確認する。
- Retry、Next Stage、Stage Select、Victory / Defeatとの排他を確認する。

### Phase 4: 既存Undo統合

- 検証中は既存Uとターン開始Undoを別キーで運用する。
- 安定後、Uをターン開始Undoへ統一する。
- Controls、README、関連Docsを更新する。
- 旧移動Undoフラグとコードを削除する場合は、独立した小さな変更として行う。

## 14. 実装時に変更が想定されるファイル

- `Assets/Scripts/Battle/TurnManager.cs`
- `Assets/Scripts/Battle/PlayerController.cs`
- `Assets/Scripts/Grid/GridManager.cs`
- `Assets/Scripts/Units/Unit.cs`
- `Assets/Scripts/UI/BattleUI.cs`
- `Assets/Scripts/Visual/EnemyIntentPreview.cs`
- `Verify/UnityStubs.cs`（新APIをCompileCheck対象にする場合）
- 新規Undo関連クラス
  - `Assets/Scripts/Battle/TurnStartSnapshot.cs`
  - `Assets/Scripts/Battle/TurnSnapshotService.cs`

今回は上記C#ファイルを変更しない。

## 15. 実装時に変更禁止または注意する範囲

- `StageManager` のステージ進行ロジック
- `StageData` のステージ定義
- `StageLoader` の生成規則
- 敵AIの対象選択・実行ロジック
- 攻撃可能判定
- 攻撃範囲計算
- ダメージ計算
- 移動可能範囲計算
- BoardProjectionの投影計算
- UnitAttackAnimatorの攻撃方向・演出仕様

StageManagerへUndo状態を直接持たせない。遷移時のInvalidate通知が必要なら、既存遷移処理の直前にサービスをクリアする最小連携に留める。

## 16. 検証項目

### 基本復元

- [ ] 未行動状態でUndoして盤面が変化しない。
- [ ] 移動後にターン開始位置へ戻る。
- [ ] 攻撃後に攻撃者と対象のHP・位置が戻る。
- [ ] 非最終敵の撃破後に敵が復活する。
- [ ] 待機後にHasActedが戻る。
- [ ] 複数味方の行動後に全Unitがターン開始状態へ戻る。

### UnitとGrid

- [ ] GridPositionとWorldPositionが一致する。
- [ ] 全生存UnitのTile占有が正しい。
- [ ] 同一Tileに複数Unitが登録されない。
- [ ] 死亡復元UnitがGridManagerへ再登録される。
- [ ] HP TextとHP背景が再表示される。
- [ ] HasActedの暗色表示が戻る。
- [ ] 台座、選択リング、sortingOrderが正しい。

### UIと予測

- [ ] Selected Unit表示が初期状態へ戻る。
- [ ] 移動・攻撃ハイライトが残留しない。
- [ ] Battle Logがターン開始時点へ戻る。
- [ ] Undo完了通知が仕様どおり表示される。
- [ ] Enemy Threat OFFなら復元後もOFFである。
- [ ] Enemy Threat ONならThreatタイルが再計算される。
- [ ] AttackNow / MoveToward / Guard表示が現在盤面から再計算される。
- [ ] 古いIntent矢印・リングが残らない。

### 排他と遷移

- [ ] End Turn前はUndoできる。
- [ ] End Turn確定後はUndoできない。
- [ ] Enemy Turn中はUndoできない。
- [ ] Victory / Defeat後はUndoできない。
- [ ] 攻撃演出中はUndo入力が無視される。
- [ ] Retry後に旧Snapshotを使えない。
- [ ] Next Stage後に旧Snapshotを使えない。
- [ ] Stage Selectへ戻った後に旧Snapshotを使えない。
- [ ] StageManagerの既存Enter遷移と競合しない。

### 回帰確認

- [ ] 移動、攻撃、待機が従来どおり動く。
- [ ] 既存のEnemy AIが変わっていない。
- [ ] End Turn制が変わっていない。
- [ ] 勝敗条件が変わっていない。
- [ ] Stage 1からStage 6を起動できる。
- [ ] Windows buildで入力と復元を確認できる。

## 17. 実装開始時の推奨順序

1. Snapshotデータ型とCaptureだけを追加し、内容を開発ログで確認する。
2. Unitの副作用なし復元APIを追加する。
3. Grid占有クリア・再登録・再構築APIを追加する。
4. 位置、HP、死亡、HasActedだけをRestoreする。
5. Battle LogとPlayerController表示状態をRestoreする。
6. Threat / Intent再計算を接続する。
7. Stage遷移とEnd TurnでInvalidateする。
8. 検証後に入力キーと既存移動Undoを統合する。

各段階でUnity compileと対象シナリオの手動確認を行い、復元対象を一度に増やしすぎない。
