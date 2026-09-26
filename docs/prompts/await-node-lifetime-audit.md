# 任务: 修复「await 之后访问 Godot 节点」的一类隐患

> 这是一个**自包含**的任务描述。把它贴给一个新对话即可开工。
> 目标仓库有两个（见下），改动跨仓；**不要替用户 commit**（见「工作约定」）。

---

## 0. 一句话任务

全仓扫了一遍「await 之后访问节点」的写法，共命中 **11 个文件 / 12 处**（原为 15 处：其中 4 处随
`FadingMask` 的删除合并成 1 处，见 §4 / §6）。请按「修法优先级」逐处修复，每修一处都要能说清
「这个窗口为什么消失 / 为什么不成立」，**不要只是加个 null 检查糊过去**。

建议顺序：见 §4.1「当前处置」（早期版本的"`Tooltip` → `PaginationBinder` → …"顺序已作废：前两项已搁置）。
`SceneSwitcher` 那一条的可达性已判完（**不可达，不修**，见 §6）。

> **进度（2026-09-24 更新）**：
> - `Flag.cs` 已修（用户提交）；`FadingMask` 已整体删除（旧命中点作废）。
> - **Tooltip（#2）搁置**：整个 tooltip feature 将重新设计，不在旧实现上做修。
> - **翻页（#3 `PaginationBinder` + #4 `ExampleCard`）—— 已随 DU 重写关闭（2026-09-26）**：核心改为
>   `PaginationState` + `PageNav`（纯，无 async / ct / 锁 / 事件），binder 不再 await 内容任务
>   （`Task.WhenAll` + `catch(OCE)` 一并删除），`ExampleCard` 的封面状态搬到 await 之前。
> - **`SceneSwitcher`（#5）判定为不可达，维持现状、不修**（理由见 §6）。
> - **#11 `SlideOutAsync` 已按 §5 第 2 档（token 绑节点 + 续体自检）修**，#9/#10 随之关闭。
> - **#6/#7 `PopupLayer`、#8 `Lose`、#12 `MessageBox` 判定不可达，不修**（理由见 §4.1）。
> - **`Level1.cs:144,150,159`（§4.4 间接路径）判定不可达**（时序论证见 §4.1 第 7 条）。
> - **至此清单结清**：已修 4 处（#1 `Flag`；#11 `SlideOutAsync` 连带 #9/#10；#3/#4 随翻页 DU 重写）；
>   随重写处理 2 项（#2 tooltip、`InputMask` 输入拦截）；判定不修 6 处（#5/#6/#7/#8/#12 + `Level1` 三处）。
> - **重写开工前的遗留总览（含验证缺口）见 §4.5。**

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

**关键认识**：`CancellationToken` 只能作用于**尚未完成**的操作。被等待的操作一旦正常完成、续体已排入
同步上下文，就没有任何接口能撤回它 —— .NET 如此，Godot 的 `CallDeferred` 队列也如此。

同时要纠正一个容易走偏的说法：这里**不是**"续体假装没被取消"。真实时序是 —— 取消发生在
**节点死的那一刻**（`Free()`/`QueueFree` 触发的 `_ExitTree` → `TreeExited` → token 被取消），
而续体要到**下一帧**才被泵出。所以只要 token 与那棵树**同命**，续体执行时就能看见取消，
从而在碰节点之前退出。

→ 于是有两条路，优先级见「修法优先级」：
- (a) **不产生续体**（消除窗口）：把副作用搬到 await 之前 / 不 await / tween 的 `OnComplete`
  （仅限终结性的**同步**副作用，见 §5）；
- (b) **续体自检**（检测窗口，Godot 侧的通用默认）：token 绑节点生命周期 + 在碰节点前检查它。

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
- `PlayAsyncUntilNodeDestroy(node, ct)` 只在 tween **运行期间**随节点销毁而取消 —— 它在**内部**建的节点链接
  **不暴露给调用方**，所以续体里无从检查它。要自检就得自己在使用点建链接（见下面的通用约定），
  并且用 `PlayAsyncGD(linked.Token)`，不要再叠一层 `PlayAsyncUntilNodeDestroy`。
- **`GDTask.Yield()` / 不传 token 的 `GDTask.Delay()` 根本不接受 token** ⇒ 没有 ct 可查，
  第 2 档对它们**不可用**，只能走第 1 档（改结构）或第 3 档（`IsInstanceValid`）。
  看到这类 `await` 就别想着"绑个 token 就好了"。
- 若副作用不必发生在 await 之后，**把它搬到 await 之前**。比任何守卫都便宜。
- `GodotObject.IsInstanceValid(x)` 是兜底手段；它只能保护**那一次**访问，**不是结构性修复**
  （包括写在 `finally` 里 —— 窗口只是被缩小到一次判断，而它自身同样可能读到一个已释放的引用）。
  它仍有明确的存在价值：问的是"这个对象现在还活着吗"，不依赖任何 token 图不变式。

### 通用约定：await 之后要碰节点时

两个半边**缺一不可**：

1. **token 必须与"你要碰的那棵树"同命** —— 靠 `ct.LinkWithNodeDestroy(node)` / `node.GetCancellationTokenOnTreeExit()`，
   或在 `_ExitTree()` 里 cancel 自己的 `_tweenCts`。没有这半边，下半边就是空转。
2. **续体在碰节点的前一行检查它** —— 检查必须放在**使用点**，且链接要在**使用点自建**：
   签名里的 `CancellationToken ct = default` 在调用方不传时不可取消，只查调用方传来的 token 是
   **真空的假安全**。

```csharp
using var linked = ct.LinkWithNodeDestroy(node);   // 自建, 不依赖调用方
await tween.PlayAsyncGD(linked.Token);             // 不再叠一层 PlayAsyncUntilNodeDestroy
linked.Token.ThrowIfCancellationRequested();       // 完成之后节点才死 → 在这里退出
Hide();                                           // 这一行起才允许碰节点
```

取消时抛 `OCE`，与既有约定一致（`GDTask.Forget()` / `Task.Fire()` 默认吞 `OCE`）。

### 架构约定（本仓库技能，务必遵守）

- 三层：**Layer 1 纯逻辑**（不引用 Godot 类型）/ **Layer 2 Godot-aware POCO 执行器**（不做决策）/ **Layer 3 薄场景脚本**。
- C# 风格：record、`required init`、primary constructor、`extension(T)` 块、collection expression、file-scoped namespace、面向接口。
- **不要过度设计**：用户明确偏好"最小改动 + 消除窗口"，而不是引入新的抽象层。

### 工作约定（用户已明确）

- **绝对不要替用户 `git commit` / `git push`**。只做代码修改，把 commit message 建议给用户。
- 改动跨两个仓库，注意它们是**独立 git 仓库**，不要假设 `git -C` 能跨仓操作。
- 用户要求：**一步一步来**，一次一个概念；先说清"为什么这么改 / 代价是什么"，再动手。

---

## 3. 已修好的范例（**只适用于 tween**，通用做法见 §5）

> ⚠️ 下面两例都是 §5 第 1 档"消除续体"在 **tween 场景**下的特例。tween 是唯一带 `OnComplete` 钩子的
> 异步源，所以这两个形状**不能**当通用模板照抄到别的 `await` 上（那正是本文档早期版本走偏之处：
> `OnComplete` 只能收同步 `Action`，传 async 就是 `async void` + 回调内部同样有窗口；
> 普通 `Task`/`GDTask` 也根本没有这种钩子）。

### 范例 A：`Cover.RevealAsync`（tween 场景下窗口结构性消失）

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

- `S: Scripts/Levels/Level1.cs:166,171` —— `ct.LinkWithNodeDestroy(this)`（这是通用约定的**上半边**；
  要配上下半边"续体在碰节点前自检 ct"才完整）
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
| 2 | `S:/SnekSweeper/Scripts/UI/TooltipSystem/Tooltip.cs:36` | `TweenModulateAlpha(0,…).PlayAsyncGD(token)` | `Hide();` | **高** | ⏸ 搁置（tooltip 重设计中） |
| 3 | `G:/UI/Pagination/PaginationBinder.cs:90-92` | `Task.WhenAll(_pendingContentTasks)` | `_ui.SetNavigationEnabled/ClearContent/AddContentItem` | **高** | ✅ 随翻页 DU 重写关闭 |
| 4 | `S:/SnekSweeper/Scripts/UI/Tutorial/Example/ExampleCard.cs:33` | `grid.InitCellsAsync(snapshot, ct)` | `ApplyCoverStatus()` → `Cover.SetStatus` → `StatusIndicator.Modulate` | 中 | ✅ 随翻页 DU 重写关闭 |
| 5 | `S:/SnekSweeper/Scripts/GameStateManagement/SceneSwitcher.cs:32` | `GDTask.Yield()` | `_currentScene.Free()` / `CurrentSceneHolder.AddChild(newScene)` / `onSceneEntered?.Invoke(newScene)`（→ `Level1.LoadLevel` 会碰 `TheGrid`/`SaveData`） | 低-中 | 🚫 判定不可达，不修（见 §6） |
| 6 | `S:/SnekSweeper/Scripts/UI/Level/Popup/PopupLayer.cs:23` | `WinPopup.ShowAndGetChoiceAsync(...)` | `IsInputBlocked = false;` → `InputMask.Visible = …` | 中高 | 🚫 判定不可达（见 §4.1） |
| 7 | `S:/SnekSweeper/Scripts/UI/Level/Popup/PopupLayer.cs:31` | `LosePopup.ShowAndGetChoiceAsync(...)` | 同上 | 中高 | 🚫 判定不可达（见 §4.1） |
| 8 | `S:/SnekSweeper/Scripts/GridSystem/State/states/GridLogic.State.Lose.cs:26` | `MarkPlayerErrorsAsync(...)` | `LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct)` → 内部碰 HUD/PopupLayer | 中 | 🚫 判定不可达（见 §4.1） |
| 9 | `S:/SnekSweeper/Scripts/UI/Level/Popup/WinPopup.cs:30` | `_popupChoiceListener.GetChoiceAsync(ct)` | `_animator.HideAsync(ct)` → `SlideOutAsync` → `target.TweenGlobalPosition` / `target.Hide()` | 中 | ✅ 随 #11 关闭 |
| 10 | `S:/SnekSweeper/Scripts/UI/Level/Popup/LosePopup.cs:30` | 同上 | 同上 | 中 | ✅ 随 #11 关闭 |
| 11 | `G:/TweenStuff/TweenExtensions.cs:38-45`（`SlideOutAsync`） | `target.TweenGlobalPosition(…).PlayAsync(…)` | `target.Hide();`（**库级**，本工作区唯一调用方是 `PopupAnimator`） | 中 | ✅ **已修（§5 第 2 档）** |
| 12 | `S:/SnekSweeper/Scripts/Autoloads/MessageBox.cs:47-48` | `GDTask.Delay`、`messageLabel.FadeOutAsync` | `messageLabel.QueueFree();` | 低（autoload 与整棵树同命） | 🚫 判定不可达（见 §4.1） |

### 4.1 当前处置（2026-09-24，09-25 更新）

1. **`Tooltip`（#2，含同文件的 `CallDeferred` 家族）—— 搁置**：整个 tooltip feature 将重新设计。
2. **`PaginationBinder` + `ExampleCard`（#3/#4）—— 已随翻页 DU 重写关闭（2026-09-26）。**
   旧实现里 `PaginationBar.ClearContent()` 会 `QueueFree()` 掉仍在 `InitAsync` 中的卡片，而
   `catch (OperationCanceledException)` 只覆盖"被取消"、不覆盖"完成之后节点才死" —— 这正是同一个窗口的两端。
   重写后：binder 是"导航 → 纯转移 → 取数 → 渲染"的同步流程（`await Task.WhenAll` + `catch(OCE)` 一并删除），
   卡片的封面状态搬到 await 之前，两处的续体都不再碰节点。
3. **`SceneSwitcher`（#5）—— 判定不可达，不修**，见 §6。
4. **`SlideOutAsync`（#11）—— 已按 §5 第 2 档修**，`WinPopup`/`LosePopup`（#9/#10）随之关闭
   （它俩 await 之后只有 `return choice`，真正的尾巴在库里）。
5. **`PopupLayer`（#6/#7）与 `GridLogic.State.Lose`（#8）—— 判定不可达，不修。**
   理由（2026-09-24 复核）：`ct` = `LevelExitToken` = `Level1.GetCancellationTokenOnTreeExit()`，
   而能拆掉 `Level1` 的**唯一**入口是 `SceneSwitcher.GotoSceneAsync`，它由 `AppLogic` 的 output 驱动，
   而 level 内的 output 又只在**拿到弹窗选择之后**才产生：
   - #6/#7：`IsInputBlocked = false` 发生在 choice 交回逻辑块**之前** ⇒ 那一帧不可能有换场在飞；
   - #8：`Lose` 的续体是弹窗流程的**起点** ⇒ 它执行时更不可能有换场在飞。
   换场本身还要再等 `await GDTask.Yield()` 一个帧边界才 `Free()`，所以即便整条链在同一帧的泵里内联跑完，
   `Free()` 也至少晚一帧。
   剩下的唯一路径是**整个应用退出**（`QuitHandler` → `GetTree().Quit()`）：主循环在当次迭代后停止，
   树是在循环结束后的 `finalize` 才被拆 ⇒ 已排队的续体要么在当次迭代里（树还活着）跑完，要么永远不会被执行。
   **注意**：这个结论依赖 `GDTask.Yield()` 是真正的帧边界（也正是 `SceneSwitcher` 那个设计能成立的原因）。
6. **`MessageBox`（#12）—— 判定不可达，不修**：`messageLabel` 的唯一所有者是 autoload `MessageBox`
   （`MessageContainer` 不做 `ClearChildren`，`MessageQueue` 也不碰 label），所以它只在应用退出时死；
   退出时按第 5 条同理，续体不会再撞上已释放的树。
7. **`Level1.cs:144,150,159`（§4.4 的间接路径）—— 判定不可达，不修（2026-09-25）。**
   依据是**时序**（与 #8 同源），不是"引用不会悬空"：
   - 能拆 `Level1` 的只有 `SceneSwitcher`，而 level 内的换场由弹窗 choice 之后的 output 驱动；
   - `:144/:150`（`InitCellsAsync` 之后的 `CompleteInit()`）必然早于弹窗出现；
   - `:159`（`HandleInputAsync` 之后的 `GridLogic.Input(InputProcessed)`）同样早于弹窗；
     换场帧也不可能**新起**一次输入驱动的续体 —— 输入监听节点随树销毁，死节点不会再发事件。
   **不依赖 `InputMask` 是否拦得住**：即便输入漏过去，最坏也只是"弹窗期间还能操作棋盘"（行为问题），
   不会产生撞死节点的续体。
   附带的结构前提（**假设，须长期保持**）：`TheGrid`(HumbleGrid) 与 `Level1` 同生死、`GridLogic` 由 `Level1` 持有
   ⇒ `Context.HumbleGrid` / `Context.LevelOrchestrator` 这类访问**不需要**防御式检查。
   若将来支持"不换场地原地重建网格"（复用池 / 中途重开一局），这条前提失效，上述访问点必须重新审。
8. **`InputMask` 的输入拦截 —— 不属于本清单，标记为待重写**（随输入会话 / 弹窗重做）：
   现状是 `ColorRect` + `MOUSE_FILTER_STOP`，**只影响鼠标**；键盘/手柄不经 GUI 命中测试
   （`GridInputListener._UnhandledInput` 收得很宽）。要设备无关，正确位置是在**输入适配层**放显式闸门
   （例如 `GridInputListener` 先问 `IsInputBlocked`），而不是依赖 GUI 命中测试。
   另：`InputMask` 只覆盖屏幕一部分时，未覆盖区域的鼠标事件会漏过去。

### 4.2 类 D（**不同问题，别混进来**）

事件总线 / FSM 输出在**订阅者节点已死之后**仍然投递（`GridExtensions` → `HumbleCell` 绑定、
`Grid.cs` → `HUD.OnBombCountChanged` 等）。这不是 await 问题，而是"订阅者生命周期"问题，**本任务不处理**。

**处置（2026-09-25）**：不做专门排查，等后续遇到再单独处理（开发过程中已按"及时退订"执行）。
若将来要扫，判据就两条：① **emitter 的生命周期 > 订阅者**；② 订阅没有在 `_ExitTree` 退订。
（已核对的 `HUD.OnBombCountChanged` 两端同生共死且 `_ExitTree` 退订 ✓）

### 4.3 已确认干净（9 个文件）

`SnekGameDevKit/FileOperations.cs`、`SnekGameDevKit/SaveQueue.cs`、`CommandInvoker.cs`、
`JsonSerializationService.cs`、`GridLogic.State.Win.cs`、`G/Tasks/TaskFireAndForget.cs`、
`CellStateTests.cs`、`MatrixConverterSpecs.cs`、`PlayerSaveDataRoundTripSpecs.cs`。

### 4.4 需人工确认的间接路径（6 处）

`SnekGameDevKit/Messaging/MessageQueue.cs:17`、`GridExtensions.cs:35,44-45`、`Grid.cs:27,100,111`、
`Level1.cs:144,150,159`、`Pagination.cs:87`、`G/FSM/StateMachineV2.cs:16,31,33`（后者当前无引用）。

**复核状态（2026-09-24，09-25 更新）**：
- 已确认干净：`MessageQueue`（只转发字符串、不碰 label）、`GridExtensions.InitCellsAsync`（await 之后无节点访问）、
  Core `Grid`（纯逻辑，碰不到节点）。`Pagination.cs:87`、`G/FSM/StateMachineV2.cs` 随翻页 / FSM 重构处理。
- **`Level1.cs:144,150,159` —— 判定不可达**（2026-09-25，论证见 §4.1 第 7 条）。
- 残余的不确定项（**不影响上面任何结论**）：① LogicBlocks 在 `Stop()` 之后收到 `Input` 的确切行为未确认；
  ② `InputMask` 是否拦得住键盘/手柄输入未确认（已单列为待重写项，见 §4.1 第 8 条）。
- 另外，全仓 `Callable.From` / `CallDeferred` / `SetDeferred`（"排队到之后执行"的另一套家族）只有 1 处，
  即 `Tooltip.cs:19` —— 已随 tooltip 搁置。

### 4.5 重写开工前的遗留总览（2026-09-25）

| 桶 | 内容 |
|---|---|
| ✅ 已修 | #1 `Flag`；#11 `SlideOutAsync`（连带 #9/#10）；#3/#4 随翻页 DU 重写；`TaskCancellationExtensions.GetCancellationTokenOnTreeExit` 改为按节点缓存 |
| 🚫 判定不修（有论证） | #5 `SceneSwitcher`；#6/#7 `PopupLayer`；#8 `Lose`；#12 `MessageBox`；`Level1.cs:144/150/159` |
| ⏸ 随重写处理 | #2 tooltip（含 `CallDeferred` 家族）；`InputMask` 输入拦截（§4.1 第 8 条） |
| 🕓 另立一类不处理 | 类 D 订阅者生命周期（判据见 §4.2） |

**验证缺口（唯一没闭合的一环）**：以上结论除 #1/#11/缓存那次改动外，**全是静态推理**，没有运行时验证。
按 §6 跑一遍手动回归（反复切场景 / 翻页 / tooltip 悬停 / 胜负弹窗），并确认输出面板里没有
`UnobservedTaskException` / `ObjectDisposedException`。`GetCancellationTokenOnTreeExit` 的缓存改动最值得先跑一次
（它影响每次动画、每次输入、每条消息的 token）。

> **🏗️ 重构提醒（pagination / tooltip 开工时主动提）**：那两个领域重写时，凡 `await` 之后要碰节点的写法
> 都按 §5 三档处理（token 绑节点生命周期 + 在使用点自建链接 + 续体自检；当心 `ct = default` 的假安全陷阱）。


---

## 5. 修法优先级（模板）

按"能不能**消除**窗口"排序，逐档降级：

1. **不产生续体（消除窗口，首选）**
   - 副作用能搬到 await **之前** → 搬。比任何守卫都便宜。
   - 不需要等 → 不 await。
   - 只有"终结性的**同步**副作用"、且恰好有 tween 钩子 → 挂 `tween.OnComplete`。
     ⚠️ 这是第 1 档的**特例，不是通用范式**：`OnComplete` 是 tween 独有的，而且只能收同步 `Action`
     （传 async = `async void` = 新的 fire-and-forget，回调内部同样有窗口）；`Task`/`GDTask` 根本没有这种钩子。
2. **续体自检 ct（检测窗口，**Godot 侧的通用默认**）**
   - token 必须与"你要碰的那棵树"同命，且**在使用点自建链接**（不能依赖调用方传 token）。
   - 形状与理由见 §2「通用约定：await 之后要碰节点时」。
   - 取消时抛 `OCE`，与既有约定一致。
3. **`GodotObject.IsInstanceValid(x)` 兜底** —— 只在 1/2 覆盖不到时用：
   - **没有 token 的等待**：`GDTask.Yield()`、`CallDeferred` 的 flush、不传 token 的 `GDTask.Delay`；
   - **非 Node 的 Godot 对象**：`Resource`/`ShaderMaterial`/`Tween` 不是 Node，`TreeExited` 不会替它们取消；
   - 要碰的是**另一条生命周期**、token 串不进来的节点（例如 `PaginationBinder` 的 `_ui`）。
   - 它只保护**那一次**访问，不是结构性修复。

⚠️ 反模式：`if (node != null)` —— C# 里 Godot 对象的 `!= null` 会走重载/引用比较，
对"已释放但引用还在"的节点**不可靠**，请用 `IsInstanceValid`。

---

## 6. 交付要求与验收

- 每处修改都要能回答："这个窗口是怎么消失的？" —— 答不上来就是没修好。
- 用第 2 档（自检 ct）时还要能回答："这个 token 凭什么会在节点死时被取消？"，
  以及"链接是不是在使用点自建的（而不是依赖调用方传进来的 `ct`）？" —— 两个都是否，就是真空的假安全。
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
