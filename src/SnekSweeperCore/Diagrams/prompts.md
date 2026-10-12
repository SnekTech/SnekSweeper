# Prompts

## 任务清单：SaveData 落盘并发修复 + Grid FSM 重构（2026-08-27）

### 背景
- `SafeWriteAllTextAsync` 临时文件名固定为 `temp_{name}`，被并发的 fire-and-forget 写任务共用 → `temp_xxx.json not found`（level 首次点击 / cheatCodePage 加载时触发）。
- 更隐蔽的问题：fire-and-forget 并发写会乱序，磁盘最终态可能不是最后一次 `Dispatch` 的状态。
- （历史）原 Grid FSM（`GridLogic`）用 bool 字段 + 重发首次输入，弥补「同步 FSM 里塞异步」的矛盾；该 FSM 已整体重写为 Core 的 `GridSession` DU，不再依赖 LogicBlocks。

### Todo
- [x] 修复 `FileOperations.SafeWriteAllTextAsync`：临时文件名加唯一随机后缀，杜绝跨任务冲突
- [x] `SaveData` 改单写者：新增 `SnekGameDevKit.SaveQueue<T>`（Channel 单消费者 + 突发合并），`Dispatch` 入队、`_Ready` 启动循环；保证写顺序 + 合并写，一次覆盖所有调用点
- [x] 澄清 `NotifySaved` 仅是反馈非落盘；确认退出路径 `SaveNow()` 同步刷最新快照兜底
- [x] Grid FSM 重构（**已整体重写，现已不依赖 LogicBlocks**）：现为 Core 的 `GridSession` DU +
  `GridSessionMachine.Start/Update` 纯总函数（`src/SnekSweeperCore/GridSystem/Session/GridSession.cs`），
  由宿主 `Level1` 同步消费返回的 `GridSession.Effect` 列表。原 `GridLogic`（Output+回调）那一版设计已被取代。
