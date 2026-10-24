# Camera Zoom 设计（Living Doc）

> 状态：**已暂缓（deferred）** —— 2026-10-24 决定暂不实现，理由见下。
> 本文档是决策日志：`✅ 已定` / `🔶 建议（待确认）` / `❓ 待定`。
> 最后更新：2026-10-24

## 0. 结论（2026-10-24）

**暂缓 zoom，先做手柄输入。**

- 实测：桌面 PC + 手柄、当前 640×360 逻辑分辨率下，**30×16 的 expert 关卡不需要缩放也能正常游玩**
  （16px 格 → 48 物理 px，整盘 480 逻辑 px 落在 1920 宽窗口里）。
- ⇒ 关卡**可操作窗口暂定为 30×16**；将来若做更大的 grid，再回到本文档
  （届时优先考虑「窗口 30×16 + 边缘跟随/其他导航」，而不是 zoom）。
- 本文档的 D1/D2/D3 属于「一旦做 zoom 就必须遵守」的技术约束，保留；D4–D7 仍是待验证的建议。
- 下一步：`docs/design/gamepad-input.md`。

## 1. 目标

- 手柄 + 电视观看距离（10-foot UI）下，30×16 高级关卡可玩。
- 同时支持键鼠（鼠标滚轮），不牺牲现有体验（现有 expert 关卡 zoom 1 就整盘可见）。
- 与将来的「手柄 play 输入」解耦，各自独立设计；camera 不反向依赖 play。

## 2. 技术事实（已核实）

| 项 | 值 | 来源 |
|---|---|---|
| 逻辑视口 | 640×360；窗口 1920×1080（3×） | `project.godot [display]` |
| 拉伸 | `stretch/mode=viewport` + `scale_mode=integer` | 同上 |
| 纹理过滤 / 像素对齐 | `default_texture_filter=0`(nearest)、`snap_2d_transforms_to_pixel=true` | `project.godot [rendering]` |
| 单元格尺寸 | 16 px | `HumbleCell.CellSizeInPixels` |
| 盘面尺寸 | 9×9 / 16×16 / **16 行 × 30 列（480×256 px）** | `DifficultyFactory` |
| 现有相机 | `Camera2D` @ (320,180)，zoom 1 → **world 坐标当前恰好等于屏幕坐标** | `Level1.tscn` |
| HUD | `CanvasLayer`（不吃相机变换），**左侧竖栏**，占约 128px | `HUD.tscn` |
| 实际 play area | ≈ 512×296（x∈[128,640], y∈[64,360]） | grid 起点 (128,64) |

## 3. 决策日志

### D1 ✅ 只做整数 zoom 挡位

物理放大率 = `zoom × 3`。非整数 zoom 会让 nearest 采样出现 4/5 交替的条纹与抖动。
⇒ `Zoom ∈ {1,2,3,…}`，不做无级调节。

### D2 ✅ 相机位置每帧取整

`snap_2d_transforms_to_pixel` 只取整 canvas item 自身 transform（世界空间），相机是渲染阶段
单独叠加的 canvas transform。相机停在 0.5 世界 px 时整块棋盘照样亚像素抖动。
⇒ `Camera.Position = 期望值.Round()`，每帧。

副作用（可接受）：整数 zoom 下相机最小位移量子 = 1 世界 px = `zoom × 3` 物理 px。
手动慢速平移时要保证最低速度 ≥ 1 世界 px/帧，否则出现「一跳一停」。

### D3 ✅ zoom 与 play 分开设计/实现

- 接口方向：`play → camera`（光标目标变化）；`camera → play` 无依赖。
- camera 只暴露「导演」API（`FocusOn` / `SuspendFollow` 等），不知道 play 的内部结构。

### D4 🔶 挡位由难度推导，不写死

```
fitZoom = floor(min(playW / boardW, playH / boardH))                      // 整盘塞得下的最大整数档
maxZoom = floor(min(playW / (16 × minCols), playH / (16 × minRows)))      // 视野下限
ladder  = [fitZoom … maxZoom]，若 fitZoom > maxZoom 则单档（该难度不需要 zoom）
```

以 `minCols=12, minRows=7`、play area 512×296 试算：

| 难度 | 盘面 | fitZoom | maxZoom | 挡位 | zoom 最高档可见 |
|---|---|---|---|---|---|
| Expert 16×30 | 480×256 | 1 | 2 | [1, 2] | 16 × 9.25 格 |
| Intermediate 16×16 | 256×256 | 1 | 2 | [1, 2] | 16 × 9.25 格 |
| Beginner 9×9 | 144×144 | 2 | 2 | [2]（无 zoom） | 整盘 |

要点：「局部 focus 范围不大不小」用 `minVisibleCells`（12×7 ≈ 2.5 个 3×3 邻域）表达，
**相机可见格数在任意难度下基本恒定**，不需要逐难度调参。9×9 自动无 zoom 是正确行为。

### D5 🔶 相机跟随：安全框 + 最小位移

- 相机只在 cursor 越过 play area 内缩 ~25% 的「安全框」时移动；
- **只移动到刚好让 cursor 贴住安全框边界**（最小位移，不清屏居中）；
- 小范围清扫时相机完全不动（视觉稳定），跨区时世界在 cursor 下方连续滚动；
- 到盘边 clamp 后自动停住（能回头看板边）；
- fit 挡时整盘可见 ⇒ clamp 数学上自动锁定相机，无需特判。
- pan 用 ~100ms 补间（非瞬移），位移量取「光标移动量」而非「居中位移」。

### D6 🔶 大范围转移 = 「overview 弹簧」+ overview 期光标加速

- 按住 LT：临时把 zoom 拉回 **fitZoom**（= 看全盘），跟随暂停；松开：回到原挡位并以 cursor 为锚。
- overview 期间 **cursor 移动速率显著提高**（概览不需要精度，要速度）。
  ⚠️ 单靠「拉远看全盘」不会让 cursor 走得更快，必须配合提速，否则 30 格横穿依然是 3s。
- 理由：扫雷玩家本来就要频繁看全盘（每十几步重读盘面），这不是为长距离移动额外加的功能，
  而是**本来就必须存在的功能**；顺带解决长距离转移，且不引入新的模态 UI。

### D7 🔶 不做 4/8 分区跳转，与将来的「cell 标签跳转」合并

- 分区跳转（LT 高亮 4/8 分区 → 右摇杆选 → 松手跳转）与首条需求里设想的 vim-jump
  （给可疑格打标签、按键直达）**是同一个机制**：绝对选目标 → cursor + camera 跳过去。
- 差异只在目标集合：固定空间划区 vs 有意义的 cell（前沿格 = 已覆盖且有已开邻居）。
- cell 标签版严格更强（目标有意义、无任意边界、任意 zoom 可用）；分区版唯一优势是
  不需要候选格检测，而前沿格检测是几行代码。
- 量化：zoom 2 下 4 分区每个约 15×8 格 ≈ 0.94 屏，8 分区 ≈ 半屏 ⇒ 跳转只省约 1 屏滚动，收益有上限。
- ⇒ 只保留一套，归属 **play 讨论**；camera 侧只需保证 `FocusOn(index, zoom)` 存在。

## 4. 输入映射（草案，未定稿）

| 输入 | 作用 | 状态 |
|---|---|---|
| 鼠标滚轮 | zoom 一档（锚点 = 鼠标/cursor） | 🔶 |
| 右摇杆 ← / → | zoom 一档（长推 350ms 连发） | 🔶（右摇杆是否留给他用待定） |
| LT 按住 | overview（拉回 fitZoom + 光标加速 + 跟随暂停） | 🔶 |
| 左摇杆 | cursor（play 范畴） | — |

## 5. 相机 API 契约（草案）

```
Input:  SetBoard(worldRect)                      // 盘面世界矩形
        SetZoomLadder(ints)                      // 由 D4 推导
        CursorTargetChanged(GridIndex?)          // from play
        FocusOn(GridIndex, zoomLevel?)           // 绝对跳转（标签跳转 / 连锁后归位）
        SuspendFollow() / ResumeFollow()
        SetOverviewHeld(bool)                    // LT 弹簧
Output: ZoomLevelChanged(int)                    // HUD 显示档位 pips
```

## 6. 待定问题

- **Q1** 主交互：D5 跟随为默认是否接受？（替代方案：按住 LT 双摇杆手动平移）
- **Q2** HUD 左竖栏：短期让相机避开（play area 512×296），还是中期改成顶/底条（play area ≈ 全屏，
  zoom 2 可见 20×11 格）？
- **Q3** zoom 过渡：150ms 平滑（过渡期亚像素抖动）/ 一刀切 / 整数棘轮？
- **Q4** 右摇杆 ←/→ 做 zoom 可以吗？play 是否要留？
- **Q5** 纯函数放 `GodotGadgets`（与 `TooltipPlacement` 同性质）还是先留 SnekSweeper？
- **Q6** LT 是否可用于 overview？（play 是否要占 LT）
- **Q7** 松开 LT 是「弹回原挡位」还是「锁在 fit」？
- **Q8** overview 期光标加速倍率（2×？连续快移？）
- **Q9** zoom 到底是核心机制还是可选可及性功能？（取决于电视实机观感：
  expert 在 1080p 上 16px 格 = 48 物理 px，3 米外是否够读）

## 7. 前置修复（与 zoom 无关，但必须做）

- `GridInputListener.TargetAt`：`mouseEvent.Position - GlobalPosition` 是「屏幕坐标 − 世界坐标」，
  现在成立只因「相机在视口中心 + zoom 1」这个巧合。应改为
  `GetGlobalMousePosition() - GlobalPosition`（或 `MakeCanvasPositionLocal`）。
- `HUD` 是否在所有 zoom 下都保持可读/不遮挡（取决于 Q2）。

## 8. 数值附录

- zoom 1：可见 40 × 22.5 格（整视口）/ 32 × 18.5 格（play area）→ expert 整盘可见。
- zoom 2：一个 cell = 2×2 buffer px = 6×6 物理 px；可见 16 × 9.25 格。
- expert 在 zoom 2 下整盘 ≈ 1.9 × 1.7 屏；最坏横向 14 格 ≈ 224 世界 px 的连续滚动。
- 相机最小位移量子：zoom 1 → 3 物理 px；zoom 2 → 6；zoom 3 → 9。
