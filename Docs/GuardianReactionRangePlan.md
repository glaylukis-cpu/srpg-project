# Guardian 反応範囲表示の実装計画

## 問題の背景

Stage4 では、Guardian（守衛）2体がマップ上に配置されているが、プレイヤーはそれらの反応範囲を視覚的に確認することができない。このため、Guardian を引き離すための最適な経路やタイミングが判断しづらく、試行錯誤を強いられる UX 上の問題が存在する。

Stage4 の設計意図（`StageData.cs:204` のコメントより）:

> Two guardians can be pulled separately if the player checks each reaction range before entering.

この「reaction range を確認する」仕組みがまだ実装されていないため、上記の設計が機能していない。

## Guardian 反応範囲の仕様

Guardian は反応範囲外では guarded / 待機状態にある。敵ターン時に以下の順序で動作する:

1. `enemy.InitialGridPosition` から **マンハッタン距離 3 以内** に味方がいるかどうかを判定
2. 反応範囲内に味方がいる場合、Guardian は起動し `ActAggressiveEnemy(enemy, target)` に移行 → その後は通常敵と同様に移動・接近・攻撃する可能性がある
3. 反応範囲に味方がいない場合、Guardian は待機のまま

- GuardianReactionRange = 3（`GridManager.cs:27`）
- 反応判定の基準は **現在位置ではなく `InitialGridPosition`**（配置時のグリッド位置）
- 実装：`GridManager.IsPlayerInsideGuardianReactionRange()`（`GridManager.cs:506`）、`TurnManager.ActGuardianEnemy()`（`TurnManager.cs:299`）

## 実装方針

「Enemy Threat ON」機能（敵の攻撃範囲表示）に **Guardian Reaction Range の追加表示** を統合する方針とする。

### 表現方法

既存の `Tile.cs` にあるハイライト機構を拡張し、新しい色で Guardian 反応範囲タイルを表示する:

1. **Tile.cs** へ以下の追加:
   - Guardian 反応範囲用の色プロパティ (`_guardianReactionRangeColor`) — デフォルトは緑色（#00FF00, alpha ≈ 0.22）としたが、実装時に視認性が見づらい場合は薄い黄 / 琥珀色 / シアンなどへ調整可能
   - Guardian 反応範囲適用フラグ (`isGuardianReactionRange`) — true にセットされたTileを別色で描画するよう更新
   - フラグと色を設定するための Setter 追加

2. **GridManager.cs** へ以下の追加:
   - `GetGuardianReactionRangeTiles(Guardian)` メソッドを追加
   - Guardian の `InitialGridPosition` からマンハッタン距離 ≤ 3 の全タイルを取得・返す

3. **PlayerController.cs** の Enemy Threat ON 表示フローに以下の追加:
   - Guardianユニットの反応範囲タイルを取得し、Tile setter を通じてハイライト
   - 既存の攻撃範囲 / 移動範囲とは別の色で描画されるため併存可能

### 条件付き表示

敵が脅威（attackable）である場合かつ Enemy Threat が ON の場合に限り適用され、既存の `IsEnemyThreat` / `DrawThreatOverlay` と同じ制御フローに従う。

## 修正候補ファイル

以下のファイルを最低限変更する：

| ファイル | 変更内容 |
|---|---|
| `Assets/Scripts/Grid/Tile.cs` | Guardian 反応範囲描画のためのプロパティ追加（適用フラグ等） |
| `Assets/Scripts/Grid/GridManager.cs` | 反応範囲判定を既存の Threat/attackable フロントと統合; 緑色オーバーレイ描画メソッド追加 |
| `Assets/Scripts/Battle/PlayerController.cs` | Guardian の反応範囲が視認できるように Enemy Threat ON の表示制御に連携部分があれば調整 |

## 変更してはいけない箇所

- **敵AI挙動**: Guardian がマンハッタン距離3以内で攻撃するというロジック、他の敵の移動・攻撃判定
- **Guardian反応距離**: マンハッタン距離3というバリュー自体
- **StageData**: ユニット配置やステージパラメータ
- **勝利/敗北条件**: 全敵撃破でVICTORY、自軍全滅でDEFEAT の判定
- **StageManager 進行処理**: ターン管理、フェーズ遷移、画面遷移

## ステージ状況

| Stage | Guardian 数 | GridPos | Range |
|---|---|---|---|
| 3 | 1 | (5,6) | 3 |
| 4 | 2 | (6,3), (6,6) | 3 |
| 5 | 1 | (6,3) | 3 |
| 6 | 1 | (6,5) | 3 |

Stage4 が最も反応範囲表示の恩恵を受けるステージである（2体の Guardian を引き離す戦略が前提）。

## 開発計画

Codex 制限解除後に実装する予定である。Codex のアクセス制約中は本ドキュメントで方針を記録し、実際のコード変更は Codex に委讓する。

## Unity Editor で確認すべき項目

- [ ] Stage4 / Stage6（Guardianステージ）において Enemy Threat を ON にすると Guardian 上に緑色オーバーレイが表示される
- [ ] オーバーレイ領域が `InitialGridPosition` のマンハッタン距離3以内（ダイアモンド形状）と一致している
- [ ] 味方が緑色オーバーレイの境界線上に移動した際に正しく範囲内/範囲外判定が行われている
- [ ] Guardian を手前に引きずり回しても、反応範囲表示は `InitialGridPosition` に固定されている（現在位置を基準にしたものではない）
- [ ] Guardian 以外の通常敵には Guardians の反応範囲ハイライトが表示されないこと
- [ ] Stage5 の Guardian にも反应範囲が表示されること
- [ ] Enemy Threat OFF では何ら描画されないこと
- [ ] Console にエラー / Warning が出力されていないこと