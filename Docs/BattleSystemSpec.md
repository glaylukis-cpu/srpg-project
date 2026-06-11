# SRPG Battle System Specification

## グリッド
- GridManager.cs: グリッドの生成と管理
- Tile.cs: タイルの基本情報（地形タイプなど）

## ユニット
- Unit.cs: ユニットの基本クラス
  - ファクション（Player/Enemy）
  - ユニットタイプ（Soldier, Knight, Archer, Rogue）
  - 攻撃力、HP、移動力などのパラメータ
  - 移動、攻撃、死亡処理機能

## ターン制
- TurnManager.cs: ターン管理
  - PlayerTurnとEnemyTurnの切り替え
  - ターン数管理
  - プレイヤーターン開始/終了処理
  - 敵ターン開始/終了処理

## プレイヤー操作
- PlayerController.cs: プレイヤーの操作
  - ユニット選択、移動、攻撃処理
  - 移動可能範囲/攻撃可能範囲の表示
  - タイルクリック処理

## 敵AI
- TurnManager.cs 内に実装
  - Aggressive: 最も近いプレイヤーを攻撃
  - WeakTarget: 最もHPが低いプレイヤーを攻撃
  - Stationary: 移動せず、射程内のプレイヤーを攻撃する固定型AI
  - Guardian: 護衛AI（範囲内にプレイヤーがいる場合のみ攻撃）

## 勝利条件
- StageData.cs で設定可能
  - DefeatAllEnemies: 全ての敵を倒す
  - ReachGoal: 自ターン中にゴールタイルに移動

## 敗北条件
- StageData.cs で設定可能
  - AllPlayersDefeated: プレイヤー全員が倒れる
  - TurnLimitExceeded: ターン制限を超える

## UI
- BattleUI.cs: 戦闘画面のUI表示
  - ターン数、ステージ情報
  - 選択中ユニット、操作説明、攻撃プレビューの表示
  - 戦闘ログ表示
  - 勝敗表示

## Audio
- AudioManager.cs: オーディオ管理
  - BGM（タイトル、ステージ選択、バトル）
  - SE（効果音）
  - 音量設定機能