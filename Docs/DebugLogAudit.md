# Debug Log Audit

## Summary

- TODO / FIXME / HACK は現時点で該当なし。
- Debug.LogWarning は StageManager / PlayerController / GridManager に存在する。
- Debug.LogError は Editor 検証用コードに存在する。
- Debug.Log は戦闘、ステージ遷移、敵AI、UI、ビルド検証などに多数存在する。
- リリース前には、開発中ログと残すべき異常系ログを分けて整理する。

## Keep

- Editor検証用ログ。
- BuildDemo系ログ。
- GridManagerのスポーン失敗や地形範囲外などのWarning。
- Victory / Defeat / ALL CLEAR / 評価ログ。

## Cleanup Candidates

- PlayerController.cs の選択、ハイライト、移動範囲表示ログ。
- TurnManager.cs の敵AI行動ログ。
- StageManager.cs の画面遷移ログ。
- AudioManager.cs のmute切替ログ。
- BattleUI.cs の操作説明ログ。
- Unit.cs の被ダメージや撃破ログ。

## Current Findings

### TODO / FIXME / HACK

該当なし。

### Debug.LogWarning

- Assets/Scripts/Stage/StageManager.cs:45, 52, 91
- Assets/Scripts/Battle/PlayerController.cs:399
- Assets/Scripts/Grid/GridManager.cs:176, 183, 219

### Debug.LogError

- Assets/Editor/V04DemoPolishVerification.cs:76

### Debug.Log

- Assets/Scripts/Stage/StageManager.cs:83, 109, 122, 243, 258, 303, 318, 451, 461
- Assets/Scripts/Stage/StageLoader.cs:16, 22, 39
- Assets/Scripts/Battle/PlayerController.cs:255, 283, 299, 329, 373, 410, 419
- Assets/Scripts/Battle/TurnManager.cs:92, 132, 176, 195, 220, 258, 274, 289, 303, 359, 516, 526, 542, 550, 644
- Assets/Scripts/Units/Unit.cs:214, 323
- Assets/Scripts/Audio/AudioManager.cs:164
- Assets/Scripts/UI/BattleUI.cs:1113
- Assets/Editor/BuildDemo.cs:30, 34, 48
- Assets/Editor/V04DemoPolishVerification.cs:44, 49, 53, 61, 65

## Recommended Next Task

1. Codexに、リリースビルドでは開発ログを抑制できるLogger導入を依頼する。
2. Codexに、頻繁に出る操作ログや敵AIログを条件付きログへ置き換える作業を依頼する。
3. OpenHands/Qwen3に、リリース前チェックリストをDocsへ作成させる。

## Logger Status

- DevLogger.cs を導入済み。
- DevLogger.Log は UNITY_EDITOR または DEVELOPMENT_BUILD のときのみ Debug.Log を出力する。
- Release build では開発用ログを抑制できる。
- Debug.LogWarning / Debug.LogError は原則として残す方針。
- 初回置換では PlayerController / TurnManager / StageManager の一部開発ログのみ DevLogger.Log に置き換え済み。
- Victory / Defeat / ALL CLEAR / 評価ログは未置換で残している。
