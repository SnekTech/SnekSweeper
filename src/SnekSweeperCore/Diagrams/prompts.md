# Prompts

## 任务清单：SaveData 落盘并发修复 + GridLogic 状态机重构（2026-08-27）

### 背景
- `SafeWriteAllTextAsync` 临时文件名固定为 `temp_{name}`，被并发的 fire-and-forget 写任务共用 → `temp_xxx.json not found`（level 首次点击 / cheatCodePage 加载时触发）。
- 更隐蔽的问题：fire-and-forget 并发写会乱序，磁盘最终态可能不是最后一次 `Dispatch` 的状态。
- `GridLogic` 用 bool 字段 + 重发首次输入，弥补「同步 LogicBlocks FSM 里塞异步」的根本矛盾。

### Todo
- [x] 修复 `FileOperations.SafeWriteAllTextAsync`：临时文件名加唯一随机后缀，杜绝跨任务冲突
- [x] `SaveData` 改单写者：新增 `SnekGameDevKit.SaveQueue<T>`（Channel 单消费者 + 突发合并），`Dispatch` 入队、`_Ready` 启动循环；保证写顺序 + 合并写，一次覆盖所有调用点
- [x] 澄清 `NotifySaved` 仅是反馈非落盘；确认退出路径 `SaveNow()` 同步刷最新快照兜底
- [x] `GridLogic` 状态机重构（已落地，Output+回调机制）：
  - `GameRunning` 移除 `_isProcessingInput`：输入处理改为 `Output.ProcessInput` → 绑定层 `await HandleInputAsync` → 回调 `Input.InputProcessed`，并跳过 `NothingHappens` 的冗余快照
  - 三个 instantiated 状态合并为 `Initializing`：`Data.PendingFirstInput` 显式存首次输入，按 `LoadLevelSource` 决定接受规则/光标锁定/StartInfo，发 `Output.InitializeGrid` → 绑定层初始化（FromGridSnapshot 走完整棋盘恢复）→ 回调 `Input.InitCompleted` → `Running.OnEnter` 消费 pending
  - 续局（`FromGridSnapshot`）在进入 `Initializing` 时即恢复棋盘（早于首次点击），避免「全部盖着 + 盲点踩雷」
  - 移除 `Input.StartLevel` 与 `OnReadyToHandleFirstInput` 重发 hack
  - 删除 `RegularInstantiated / InstantiatedFromRecord / InstantiatedFromSnapshot` 三个旧状态文件
- [ ] （可选）F# 试点评估：GridLogic 决策核心可作 CoreFS 第二个试点；SaveData 不迁移 F#
