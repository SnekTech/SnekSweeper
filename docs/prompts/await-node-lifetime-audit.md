# 任务: 修复「await 之后访问 Godot 节点」的一类隐患

> 这是一个**自包含**的任务描述。把它贴给一个新对话即可开工。
> 目标仓库有两个（见下），改动跨仓；**不要替用户 commit**（见「工作约定」）。

---

## 0. 一句话任务

全仓扫了一遍「await 之后访问节点」的写法，共命中 **11 个文件 / 12 处**（原为 15 处：其中 4 处随
`FadingMask` 的删除合并成 1 处，见 §4 / §6）。请按「修法优先级」逐处修复，每修一处都要能说清
「这个窗口为什么消失 / 为什么不成立」，**不要只是加个 null 检查糊过去**。

建议顺序：`Tooltip` → `PaginationBinder` + `ExampleCard` → `PopupLayer` 两条 → 其余按风险排
（`SceneSwitcher` 那一条的可达性存疑，**先判断再决定**，见 §6）。

> **进度**：`Flag.cs` 已由用户按优先级 1 修复并提交；`SceneSwitcher` 的黑幕过渡已被整体删除
> （该组件已不存在，旧命中点随之作废）。其余待修。

---

## 1. 背景：这个问题是怎么被发现的

在 `Cover.RevealAsync`（格子揭盖动画）里出现了一个 `UnobservedTaskException`：

```
UnobservedTaskException → GodotObject.GetPtr → CanvasItem.Hide() → Cover.RevealAsync(Cover.cs:line 47)
```

诡异点：**await 正常完成了**（所以后一行 `Hide()` 才执行了），但 `Hide()` 时节点已经死了。

### 根因机制（本任务的核心）

GodotTask（GDTask 3.2.0）的 await 续体由 `GodotSynchronizationContext.ExecutePendingContinuations()`
在**下一帧**泵出。于是存在一个窗口：

```
帧 N   : tween 播完 → task 完成；同一帧内场景被切走/容器 ClearChildren() → 节点 QueueFree
帧 N+1 : 续体才被泵出 → 访问已释放的节点 → ObjectDisposedException
```

**关键认识**：所有现成的"取消机制"（`PlayAsyncUntilNodeDestroy`、`ct.LinkWithNodeDestroy(node)`、
`_ExitTree` 里 cancel）都只保护 **还在进行中的 await**。任务一旦完成、续体已排队，
这些机制**全部失效** —— 而 .NET 里**已排队的续体无法取消**。

→ 所以有且只有两条路：
- (a) 让续体自己检查取消/有效性；
- (b) **压根不产生续体**（首选，见「修法优先级」第 1 条）。

### 放大器：为什么这类 bug 很难被发现

这些调用方几乎都是 `.Forget()` / `.Fire()`（fire-and-forget），异常不会崩、不会弹窗，
只会变成一条 unobserved 日志。而且窗口只有一帧，本地几乎无法稳定复现。

---

## 2. 关键上下文 / 环境

| 项 | 值 |
|---|---|
| 引擎 | Godot **4.7.1** + C#（net10.0, LangVersion 14） |
| 主项目 | `d:\Dev\GodotProjects\SnekSweeper`（git 仓库 A） |
| 复用类库 | `d:\Dev\RiderProjects\GodotGadgets`（**独立 git 仓库 B**） |
| 异步 | GDTask **3.2.0**（`GDTask` / `.Forget()` / `.AsGDTask()`） |
| 缓动 | GTweensGodot **6.0.0**：`GTweenExtensions.Tween(...)`、`.OnComplete(Action)`、`.OnStart(...)`（`On*` = 回调族）、`.Kill()`、`PlayAsync(ct)`、`PlayAsyncUntilNodeDestroy(node, ct)` |
| 生成器 | GodotSharp.SourceGenerators 2.7.0（`[SceneTree]` → `_.节点`、`ISceneScript`、`SceneFactory.Instantiate<T>()`、`InstantiateOnParent(this)`） |

### 重要语义（已核实）

- `GTween.Kill()` **不会触发 `OnComplete`**；而自然播完会**同步**触发。
  ⇒ 把"收尾动作"放进 `OnComplete`，"该不该收尾"就完全由 tween 的生死决定，不再需要 token 守卫。
- `PlayAsyncUntilNodeDestroy(node, ct)` 只在 tween **运行期间**随节点销毁而取消，救不了"完成后"。
- **`GDTask.Yield()` / `GDTask.Delay()` 这类等待原语不接受 token**。
  ⇒ 修法优先级第 2 条（把 token 绑到节点生命周期）对它们**根本不可用**，只能走第 1 条（改结构）
  或第 3 条（`IsInstanceValid`）。看到这类 `await` 就别想着"绑个 token 就好了"。
- 若副作用不必发生在 await 之后，**把它搬到 await 之前**。比任何守卫都便宜。
- `GodotObject.IsInstanceValid(x)` 是兜底手段；它只能保护**那一次**访问，**不是结构性修复**
  （包括写在 `finally` 里 —— 窗口只是被缩小到一次判断，而它自身同样可能读到一个已释放的引用）。

### 架构约定（本仓库技能，务必遵守）

- 三层：**Layer 1 纯逻辑**（不引用 Godot 类型）/ **Layer 2 Godot-aware POCO 执行器**（不做决策）/ **Layer 3 薄场景脚本**。
- C# 风格：record、`required init`、primary constructor、`extension(T)` 块、collection expression、file-scoped namespace、面向接口。
- **不要过度设计**：用户明确偏好"最小改动 + 消除窗口"，而不是引入新的抽象层。

### 工作约定（用户已明确）

- **绝对不要替用户 `git commit` / `git push`**。只做代码修改，把 commit message 建议给用户。
- 改动跨两个仓库，注意它们是**独立 git 仓库**，不要假设 `git -C` 能跨仓操作。
- 用户要求：**一步一步来**，一次一个概念；先说清"为什么这么改 / 代价是什么"，再动手。

---

## 3. 已修好的范例（可直接照抄的形状）

### 范例 A：`Cover.RevealAsync`（最佳范例，窗口结构性消失）

```csharp
// S: src/SnekSweeper/Scripts/CellSystem/Components/Cover.cs
var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 1, AnimationDuration)
    .OnComplete(Hide);                       // ← 收尾搬进 OnComplete
await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
// ← await 之后**零**节点访问
```

它**顺手修掉了第二个隐藏 bug**：旧写法下，一个被 `PutOnAsync` 取代的 `RevealAsync`
会在 await 之后执行 `Hide()` —— 即"刚盖上又被藏起来"。用 `OnComplete` 后，被取代 = tween 被 Kill = 回调不跑。

### 范例 B：`Flag.PutDownAsync`（用户采用，同一形状的第二例）

```csharp
// S: src/SnekSweeper/Scripts/CellSystem/Components/Flag.cs:48-52
var tween = FlagSprite.TweenPositionY(StartPositionY, AnimationDuration).SetEasing(Easing.InQuad)
    .OnComplete(Hide);                                  // ← 收尾搬进 OnComplete，await 之后零节点访问
using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);
await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
```

（同文件 `RaiseAsync` 是另一条路：`Show()` 放在 await **之前**，await 之后就没有任何访问了 ——
即上文"能搬就搬"那条。）

### ⚠️ 已被删除、不要照抄的旧模板

`SceneSwitcher` 曾经有一个 `finally { if (IsInstanceValid(fadingMask)) fadingMask.QueueFree(); }`。
那是**尾守卫**而非结构性修复，而且 `FadingMask` 这个组件已被整体删除 —— 守卫本身连同它所掩盖的窗口
一起不复存在。不要再把"在 `finally` 里校验一下"当作通用模板。

### 其他已守卫的点

- `S: Scripts/Levels/Level1.cs:166,171` —— `ct.LinkWithNodeDestroy(this)`（只救 await 期间，单独用不够）
- `S: Scripts/UI/Common/Pagination.cs:77-86` —— `catch (OperationCanceledException) { return; }`
- `G: Tasks/TaskCancellationExtensions.cs` —— `CancelAndDispose` 已改为幂等（吞掉 `Cancel()` 的 ODE）

---

## 4. 命中清单（扫描结果）

> 路径前缀：`S:` = `d:\Dev\GodotProjects\SnekSweeper\src\`，`G:` = `d:\Dev\RiderProjects\GodotGadgets\GodotGadgets\`
> 行号在提交 `e7612e6` 附近有效，若漂移请按符号名重新定位。
> 例外：第 5 行（`SceneSwitcher`）对应"删除 `FadingMask`"之后的新版本。

| # | 文件:行 | await 的东西 | await 之后碰的节点 | 风险 | 状态 |
|---|---|---|---|---|---|
| 1 | `S:/SnekSweeper/Scripts/CellSystem/Components/Flag.cs:49` | `tween.PlayAsyncUntilNodeDestroy` | `Hide();` | 中高 | ✅ **已修（用户改的，已提交）** |
| 2 | `S:/SnekSweeper/Scripts/UI/TooltipSystem/Tooltip.cs:36` | `TweenModulateAlpha(0,…).PlayAsyncGD(token)` | `Hide();` | **高** | 待修 |
| 3 | `G:/UI/Pagination/PaginationBinder.cs:90-92` | `Task.WhenAll(_pendingContentTasks)` | `_ui.SetNavigationEnabled/ClearContent/AddContentItem` | **高** | 待修 |
| 4 | `S:/SnekSweeper/Scripts/UI/Tutorial/Example/ExampleCard.cs:33` | `grid.InitCellsAsync(snapshot, ct)` | `ApplyCoverStatus()` → `Cover.SetStatus` → `StatusIndicator.Modulate` | 中 | 待修 |
| 5 | `S:/SnekSweeper/Scripts/GameStateManagement/SceneSwitcher.cs:32` | `GDTask.Yield()` | `_currentScene.Free()` / `CurrentSceneHolder.AddChild(newScene)` / `onSceneEntered?.Invoke(newScene)`（→ `Level1.LoadLevel` 会碰 `TheGrid`/`SaveData`） | 低-中（可达性存疑） | 待判断，见 §6 |
| 6 | `S:/SnekSweeper/Scripts/UI/Level/Popup/PopupLayer.cs:23` | `WinPopup.ShowAndGetChoiceAsync(...)` | `IsInputBlocked = false;` → `InputMask.Visible = …` | 中高 | 待修 |
| 7 | `S:/SnekSweeper/Scripts/UI/Level/Popup/PopupLayer.cs:31` | `LosePopup.ShowAndGetChoiceAsync(...)` | 同上 | 中高 | 待修 |
| 8 | `S:/SnekSweeper/Scripts/GridSystem/State/states/GridLogic.State.Lose.cs:26` | `MarkPlayerErrorsAsync(...)` | `LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct)` → 内部碰 HUD/PopupLayer | 中 | 待修 |
| 9 | `S:/SnekSweeper/Scripts/UI/Level/Popup/WinPopup.cs:30` | `_popupChoiceListener.GetChoiceAsync(ct)` | `_animator.HideAsync(ct)` → `SlideOutAsync` → `target.TweenGlobalPosition` / `target.Hide()` | 中 | 待修 |
| 10 | `S:/SnekSweeper/Scripts/UI/Level/Popup/LosePopup.cs:30` | 同上 | 同上 | 中 | 待修 |
| 11 | `G:/TweenStuff/TweenExtensions.cs:44` | `target.TweenGlobalPosition(…).PlayAsyncUntilNodeDestroy(target, ct)` | `target.Hide();`（**库级**，所有调用方受影响） | 中 | 待修 |
| 12 | `S:/SnekSweeper/Scripts/Autoloads/MessageBox.cs:47-48` | `GDTask.Delay`、`messageLabel.FadeOutAsync` | `messageLabel.QueueFree();` | 低（autoload 与整棵树同命） | 可延后 |

### 4.1 三个最值得优先处理

1. **`Tooltip.cs:36`** —— 保护层级最薄：token 只来自 `TooltipLayer._currentActionCancellationSource`，
   **没有 `LinkWithNodeDestroy`**，`TooltipLayer` 也**没有 `_ExitTree` 取消** ⇒ 切场景时窗口完全裸奔。
   同文件 `ShowAsync` 里的 `Callable.From(UpdateTooltipPosition).CallDeferred()` 属**同一家族**（"排队到之后再执行"），建议一并处理。
2. **`PaginationBinder.RefreshAsync:90-92` + `ExampleCard.cs:33`** —— 走**常规路径**（翻页/数据变更必触发），
   且 `PaginationBar.ClearContent()` 会 `QueueFree()` 掉仍在 `InitAsync` 中的卡片 ⇒ 二者是**同一个窗口的两端**，
   修一处等于同时关掉两个命中点。注意 `PaginationBinder` 里的 `catch (OperationCanceledException)` 只覆盖"被取消"，不覆盖"完成之后节点才死"。
3. **`SceneSwitcher` 自身** —— 只剩一条，且**先判断可达性再决定**（很可能是"记录理由、不修"），见 §6。

### 4.2 类 D（**不同问题，别混进来**）

事件总线 / FSM 输出在**订阅者节点已死之后**仍然投递（`GridExtensions` → `HumbleCell` 绑定、
`Grid.cs` → `HUD.OnBombCountChanged` 等）。这不是 await 问题，而是"订阅者生命周期"问题，**本任务不处理**。

### 4.3 已确认干净（9 个文件）

`SnekGameDevKit/FileOperations.cs`、`SnekGameDevKit/SaveQueue.cs`、`CommandInvoker.cs`、
`JsonSerializationService.cs`、`GridLogic.State.Win.cs`、`G/Tasks/TaskFireAndForget.cs`、
`CellStateTests.cs`、`MatrixConverterSpecs.cs`、`PlayerSaveDataRoundTripSpecs.cs`。

### 4.4 需人工确认的间接路径（6 处）

`SnekGameDevKit/Messaging/MessageQueue.cs:17`、`GridExtensions.cs:35,44-45`、`Grid.cs:27,100,111`、
`Level1.cs:144,150,159`、`Pagination.cs:87`、`G/FSM/StateMachineV2.cs:16,31,33`（后者当前无引用）。

---

## 5. 修法优先级（模板）

1. **把收尾动作搬进 `tween.OnComplete`** —— 窗口**结构性消失**，代码最短。**首选。**
   （判据：await 之后的语句只有"收尾副作用"，例如 `Hide()` / `QueueFree()` / 置标志位。）
2. **让参与的 token 绑到节点生命周期**（`ct.LinkWithNodeDestroy(node)`）—— 只救"await 期间"，
   **单独用不够**，必须与 1 或 3 配合。
3. **`GodotObject.IsInstanceValid(x)` 兜底** —— 用于无法改结构、或访问的是**另一个**节点的场景
   （如 `PopupLayer` 访问 `Level1` 里的节点）。

⚠️ 反模式：`if (node != null)` —— C# 里 Godot 对象的 `!= null` 会走重载/引用比较，
对"已释放但引用还在"的节点**不可靠**，请用 `IsInstanceValid`。

---

## 6. 交付要求与验收

- 每处修改都要能回答："这个窗口是怎么消失的？" —— 答不上来就是没修好。
- 每改一个文件跑一次全工作区错误检查（`get_errors`），保持干净。
- **`SceneSwitcher` 的那一条**：`FadingMask` 已整体删除，旧的 4 处命中只剩 1 处 ——
  `await GDTask.Yield()` 之后的 `_currentScene.Free()` / `CurrentSceneHolder.AddChild` /
  `onSceneEntered?.Invoke`。
  **先判断可达性，别急着加守卫**：
  - `GDTask.Yield()` **不接受 token** ⇒ 修法优先级第 2 条在这里不可用，只剩 `IsInstanceValid`。
  - 命中条件是"`SceneSwitcher` 所在的整棵树在『续体已排队、尚未泵出』的那一帧内被拆掉"
    （例如退出游戏 / 卸载 root）。正常换场景路径**不命中**：换场是它自己做的，且单飞标志保护了同帧重入。
  - 因此这一步的合理产出可能是「判定为低危 / 不可达，记录理由后**不修**」。这正是本任务要求的
    "说清窗口为什么存在或不存在" —— 硬加一个守卫反而是倒退。判断完请把结论写进 commit message
    或代码注释，不要只在对话里说。
  - `finally { _isTransitioning = false; }` 是纯 C# 字段写入，**不碰 Godot 对象，天然安全**，不用动。
- **不要**顺手重构无关代码；一次一个概念。

### 怎么验证

1. 编译 + 全工作区错误检查干净。
2. 手动跑游戏：反复切场景（主菜单↔设置↔历史↔教程↔对局）、教程里反复翻页、
   进对局后反复胜负弹窗 + tooltip 反复悬停。
3. 观察输出面板里是否还有 `UnobservedTaskException` / `ObjectDisposedException`。

---

## 7. 建议的第一条回复

先不要动手改代码。先回复：
1. 你对根因机制的理解（用自己的话复述"为什么取消机制救不了已排队的续体"）；
2. 你打算从哪一处开始、用什么手段、为什么；
3. `Tooltip.cs` 那一处的具体修改方案（因为它的 token 结构最可疑，可能需要顺带补 `_ExitTree` 取消）。
等用户确认后再改。
