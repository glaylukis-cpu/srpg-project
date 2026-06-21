# Unit Visual Legibility Check

## 目的

BattleVisualPolishPlan.md の Priority 3: Unit Visual Legibility に基づき、
Unity Editor での Stage1 / Stage2 / Stage4 / Stage5 目視確認を行う前に、
現在のユニット表示実装構造と変更候補を整理する。

**Unit.cs を直接変更する前にこのドキュメントを作成している。**

---

## 現在の Unit.cs 構造（grep/sed 調査結果）

以下は Assets/Scripts/Units/Unit.cs から確認した値である。
推測ではなく、実際の行番号とコードを根拠にしている。

### メインスプライトの共通 scale

- `transform.localScale = Vector3.one * gridManager.CellSize * 1.06f` (行122)
- UnitType 別分岐なし。Soldier / Knight / Archer / Rogue すべて同一scale。

### Resource sprite の PPU / pivot

- `GetResourcePixelsPerUnitScale` (行542-551):
  - Soldier / Knight: default `1.08f`
  - Archer: `1.04f`
  - Rogue: `1.02f`
- PPU計算: `pixelsPerUnit = texture.height * scale` (行536)
- pivot: `(0.5f, 0.22f)`

### Fallback pixel sprite

- size 16x16 (行570)
- pivot: `(0.5f, 0.42f)` (行672)
- Resource sprite と pivot が異なるため、表示位置比較時に注意が必要

### HP text

- `hpTextOffset = new Vector3(0f, -0.5f, -0.1f)` (行51)
- HP text localPosition: `hpTextOffset` → `(0, -0.5, -0.1)` (行340)
- fontSize: 28
- color: white

### HP text shadow

- localPosition: `hpTextOffset + Vector3(0.018f, -0.018f, 0.01f)` → `(0.018, -0.518, -0.09)` (行362)
- fontSize: 28
- color: `(0f, 0f, 0f, 0.9f)`

### HP text background

- localPosition: `hpTextOffset + new Vector3(0f, 0f, 0.02f)` → `(0, -0.5, -0.08)` (行344)
- localScale: `(0.62f, 0.2f, 1f)`
- color: black alpha `0.72`
- sortingOrder: 6

### Selection ring

- localScale: `(1.2f, 0.44f, 1f)` (行384)
- localPosition: `(0f, -0.25f, 0.03f)`
- selected player color: `(0.46f, 0.86f, 1f, 0.9f)` — bright blue
- selected enemy color: `(1f, 0.3f, 0.8f, 0.9f)` — pink/purple
- unselected: fully transparent `(0f, 0f, 0f, 0f)`
- sortingOrder: 7

### Shadow (under unit)

- localScale: `(1.08f, 0.36f, 1f)` (行378)
- localPosition: `(0f, -0.24f, 0.04f)`
- color: black alpha `0.38`
- sortingOrder: 6

### Top light

- localScale: `(0.78f, 0.2f, 1f)` (行390)
- localPosition: `(0f, 0.24f, -0.02f)`
- unselected color: `(1f, 0.92f, 0.62f, 0.18f)` — alpha 0.18
- selected color: `(1f, 0.95f, 0.68f, 0.32f)` — alpha 0.32
- sortingOrder: 9

### Acted state colors

- `GetActedVisualColor` (行462):
  - Resource sprite actors: gray tint `(0.62f, 0.66f, 0.72f, 1f)`
  - fallback sprite players: `actedPlayerColor = (0.07f, 0.16f, 0.28f, 1f)` — dark blue
- `GetNormalVisualColor` (行457):
  - Resource sprite actors: `Color.white`
  - fallback sprite players: `baseColor`
- actuation判定は `IsPlayerControlled && hasActed` による (行440)

### Sorting layer hierarchy (unit z-order)

| レイヤー | sortingOrder | z-position |
|---|---|---|
| HP text | - | -0.09 (shadow) / -0.1 (text) |
| Top light | 9 | -0.02 |
| Main sprite | 8 (Player)=8 / 7 (Enemy)=7 | ~0 (z=0 or z=-0.5f enemy offset) |
| HP background | 6 | -0.08 |
| Shadow | 6 | 0.04 |

---

## Stage別 Unity Editor 目視確認項目

### Stage1: Soldier + Archer vs Soldier + Archer

- [ ] Player Soldier / Player Archer の HPテキスト表示位置が適切か
- [ ] Selection ring が unit sprite に重ならないか、囲む具合は適切か
- [ ] acted状態（gray tint）で見分けがつくか
- [ ] 敵Soldier / 敵Archer の区分けでHPバーやリングを読み混同しないか

### Stage2: Soldier + Archer + Rogue vs Archer x2 + Soldier + Rogue

- [ ] Player Rogue が密集配置でも読みやすいか
- [ ] Player Rogue (col 0, row 1) と Player Soldier (col 1, row 1) の間隔でHPテキストが重ならないか
- [ ] 敵Rogue (Guardianなし、AggressiveAI) が Enemy Threat / HP で読めるか
- [ ] Knight不使用なのでKnight特有の確認はスキップ

### Stage4: Archer + Rogue vs Rogue + Archer x2 + Rogue × 密集パターン

- [ ] Archer / Rogue のサイズ差が小さく見える場合の確認（PPU倍率差分が少ないため）
- [ ] multiple unit type が混在する場合の視認性
- [ ] Guard range や reaction highlight が HP / ring / shadow を隠さないか → Unity Editorで要確認

### Stage5: Soldier + Rogue vs Archer x2 + Soldier + Knight × 3

- [ ] 敵3体密集時の HPテキスト可読性
- [ ] Guardian の reaction highlight と HP text background の重なりで潰れないか → Unity Editorで要確認
- [ ] acted状態とselected状態が色で見分けついているか
- [ ] HP text background (alpha 0.72) がうるさすぎないか

---

## まだ変更しない実装候補（Unity Editor を通じて判断前の案）

> **まだ Unit.cs は変更していない。** 以下の候補は Unity Editor 確認後に検討する。

### Low-risk / Localized 優先候補

| # | 項目 | 対象行 | リスク評価 |
|---|---|---|---|
| 1 | HP text Y offset の微調整 (`-0.5f` → `-0.48f` など) | 行51 / 340 | Low。他要素とz独立 |
| 2 | HP background scale の微調整 `(0.62, 0.2, 1)` | 行345 | Low。テキストサイズに比例 |
| 3 | Selection ring alpha 90% → 80% | 行384/386/388 | Low。単色変更のみ |
| 4 | Shadow alpha の微調整 `0.38` | 行378 | Low。透過値のみ |

### Medium-risk / UnitType に影響する可能性あり

| # | 項目 | 対象行 | リスク評価 |
|---|---|---|---|
| 5 | Archer PPU倍率 `1.04f` の微調整 | 行542-551 | Medium。Archer全体のscaleに影響 |
| 6 | Rogue PPU倍率 `1.02f` の微調整 | 行542-551 | Medium。Rogue全体のscaleに影響 |

### High-risk / 全体またはpivot変更は未着手

| # | 項目 | 対象行 | リスク評価 |
|---|---|---|---|
| 7 | Resource sprite pivot `(0.5, 0.22)` の変更 | 行537 | **High**。全unit型の足元位置が変動。最後に検討 |

---

## Manual Check Result

**Date:** 2026-06-21

### Unity Editor Launch

- Unity Editor launched successfully
- Runtime auto-bootstrap from empty scene: confirmed

### Per-Stage Verification

| Stage | Focus | Verdict |
|---|---|---|
| Stage 1 | Soldier / Archer HP text, selection ring, acted state | OK |
| Stage 2 | Dense unit placement (Soldier / Rogue), HP text overlap check | OK |
| Stage 4 | Archer / Rogue / Guardian + Threat ON legibility | OK |
| Stage 5 | Multi-enemy dense placement, selected / acted state colors | OK |

### Elements Verified

- [x] HP text readability
- [x] HP background visibility
- [x] Selection ring clarity (selected player/enemy)
- [x] Shadow under units
- [x] Acted state color distinction

### Result

No major unit visual legibility regression found.

### Scope

Documentation only. No C#, image, StageData, battle logic, enemy AI, victory/defeat, or UI layout changes.
