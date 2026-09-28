using System.Runtime.CompilerServices;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.CellSystem;

/// <summary>格子的事实：位置 + 有没有雷 + 邻居雷数。种完雷之后不再改变。</summary>
public readonly record struct CellInfo(GridIndex Index, bool HasBomb, int NeighborBombCount);

/// <summary>
/// 格子的状态。非法组合不可表示：翻开过的格子不能插旗，插旗的格子不能直接揭开（必须先放旗）。
/// </summary>
public abstract record CellState
{
    public sealed record Covered : CellState;
    public sealed record Revealed : CellState;
    public sealed record Flagged : CellState;
    public sealed record BombRevealed : CellState;
    public sealed record WrongFlagged : CellState;
}

/// <summary>
/// 对格子的一次意图。注意是意图而不是转移：<see cref="ToggleFlag"/> 的含义随状态而定，由 <c>Apply</c> 解释。
/// </summary>
public abstract record CellCommand
{
    public sealed record RevealCover : CellCommand;
    public sealed record PutOnCover : CellCommand;
    public sealed record ToggleFlag : CellCommand;
    public sealed record MarkError : CellCommand;
}

/// <summary>
/// 一次转移的<b>事实</b>（发生了什么），不是动画指令 —— 由视图把它映射成自己的表现。
/// 每条合法转移恰好产出一个事实（6 条边 6 个事件）。
/// </summary>
public abstract record CellEvent
{
    public sealed record CoverRevealed : CellEvent;
    public sealed record CoverPutOn : CellEvent;
    public sealed record FlagRaised : CellEvent;
    public sealed record FlagPutDown : CellEvent;
    public sealed record RevealedBomb : CellEvent;
    public sealed record FlagTurnedOutWrong : CellEvent;
}

/// <summary>转移的产出：新状态 + 发生了什么。事实为 null 表示这次意图在当前状态不成立（状态不变）。</summary>
public readonly record struct CellOutcome(CellState NextState, CellEvent? Event);

public static class CellStateExtensions
{
    extension(CellState state)
    {
        public static CellState Initial => new CellState.Covered();

        public bool IsCovered => state is CellState.Covered;
        public bool IsRevealed => state is CellState.Revealed;
        public bool IsFlagged => state is CellState.Flagged;
        public bool IsRevealedBomb => state is CellState.BombRevealed;
        public bool IsWrongFlagged => state is CellState.WrongFlagged;

        /// <summary>
        /// 纯全函数：意图在当前状态不成立时原样返回（不抛异常、不改状态）。
        /// 守卫用到的事实全部来自 <paramref name="info"/>，不读任何外部可变数据。
        /// </summary>
        public CellOutcome Apply(CellInfo info, CellCommand command) => (state, command) switch
        {
            (CellState.Covered, CellCommand.RevealCover) =>
                new(new CellState.Revealed(), new CellEvent.CoverRevealed()),
            (CellState.Covered, CellCommand.ToggleFlag) =>
                new(new CellState.Flagged(), new CellEvent.FlagRaised()),
            (CellState.Revealed, CellCommand.PutOnCover) =>
                new(new CellState.Covered(), new CellEvent.CoverPutOn()),
            (CellState.Revealed, CellCommand.MarkError) when info.HasBomb =>
                new(new CellState.BombRevealed(), new CellEvent.RevealedBomb()),
            (CellState.Flagged, CellCommand.ToggleFlag) =>
                new(new CellState.Covered(), new CellEvent.FlagPutDown()),
            (CellState.Flagged, CellCommand.MarkError) when !info.HasBomb =>
                new(new CellState.WrongFlagged(), new CellEvent.FlagTurnedOutWrong()),
            _ => new(state, null),
        };
    }
}
