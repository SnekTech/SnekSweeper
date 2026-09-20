using SnekSweeperCore.CellSystem;

namespace SnekSweeperCore.GridSystem;

/// <summary>
/// 玩家在网格上的一次意图。注意这里描述的是<b>想做什么</b>，不是"鼠标做了什么"——
/// "按下/抬起/双击"是设备事件，由表现层翻译成下面这些意图。
/// </summary>
public abstract record GridInput(GridIndex Index);

/// <summary>揭示这一格（左键抬起，作用在抬起所在的格）。</summary>
public sealed record RevealAt(GridIndex Index) : GridInput(Index);

/// <summary>以这一格为中心 chord：若它是已翻开的数字且周围旗数等于雷数，则揭开邻居（左键双击）。</summary>
public sealed record ChordAt(GridIndex Index) : GridInput(Index);

/// <summary>切换这一格的旗子（右键按下）。</summary>
public sealed record SwitchFlagAt(GridIndex Index) : GridInput(Index);

public abstract record GridInputProcessResult;

public sealed record BatchRevealed(Grid Grid, List<Cell> CellsInThisBatch) : GridInputProcessResult;

public sealed record FlagSwitched : GridInputProcessResult
{
    public static FlagSwitched Instance { get; } = new();
}

public sealed record NothingHappens : GridInputProcessResult
{
    public static NothingHappens Instance { get; } = new();
}