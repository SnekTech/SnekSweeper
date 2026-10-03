using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem;

/// <summary>
/// 关卡运行时注入给 Grid 状态机的领域上下文（从旧 FSM 提取，供 LogicBlocks 版 GridLogic 使用）。
/// </summary>
public record GridStateContext(
    Grid Grid,
    IHumbleGrid HumbleGrid,
    GameRunRecorder RunRecorder,
    ILevelOrchestrator LevelOrchestrator
);
