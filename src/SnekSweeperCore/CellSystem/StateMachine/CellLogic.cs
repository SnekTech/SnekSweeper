using Chickensoft.LogicBlocks;

namespace SnekSweeperCore.CellSystem.StateMachine;

public class CellLogic : LogicBlock
{
    public record Data
    {
        public required Cell Cell { get; init; }
    }

    public CellLogic()
    {
        Set(new CellState.Covered());
        Set(new CellState.Revealed());
        Set(new CellState.Flagged());
        Set(new CellState.BombRevealed());
        Set(new CellState.WrongFlagged());
    }

    // 取代旧 CellStateMachine 的 IsAtState<T>() / Cell.IsCovered 等查询。
    // LogicBlock.State 是当前状态（LogicBlockState?，未启动/已停止时为 null）。
    public bool IsCovered => State is CellState.Covered;
    public bool IsRevealed => State is CellState.Revealed;
    public bool IsFlagged => State is CellState.Flagged;
    public bool IsBombRevealed => State is CellState.BombRevealed;
    public bool IsWrongFlagged => State is CellState.WrongFlagged;

    // 取代旧 Cell.InitAsync 中的 SetInitStateAsync<CoveredState>()
    public void Init(Cell cell)
    {
        Set(new Data { Cell = cell });
        Start<CellState.Covered>();
    }

    // 取代旧 HandleCellRequestAsync(CellRequest.Xxx, ct)
    public void Reveal() => Input(new CellState.Input.RevealCover());
    public void PutOnCover() => Input(new CellState.Input.PutOnCover());
    public void SwitchFlag()
    {
        if (IsFlagged)
        {
            Input(new CellState.Input.PutDownFlag());
        }
        else
        {
            Input(new CellState.Input.RaiseFlag());
        }
    }

    public void MarkError() => Input(new CellState.Input.MarkError());
}

[StateDiagram]
public abstract record CellState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct RevealCover;
        public readonly record struct PutOnCover;
        public readonly record struct RaiseFlag;
        public readonly record struct PutDownFlag;
        public readonly record struct MarkError;
    }

    public static class Output
    {
        public readonly record struct CoverRevealed;
        public readonly record struct CoverPutOn;
        public readonly record struct FlagRaised;
        public readonly record struct FlagPutDown;
        public readonly record struct MarkedAsBombRevealed;
        public readonly record struct MarkedAsWrongFlagged;
    }

    // 状态机只做决策：守卫用到的领域数据从黑板取；所有演示副作用一律发 Output，
    // 由 Godot 层的 HumbleCell 通过 Bind().OnOutput() 执行（那里有 GDTask 安全处理 fire-and-forget）。
    Cell Context => Get<CellLogic.Data>().Cell;

    public record Covered : CellState, IGet<Input.RevealCover>, IGet<Input.RaiseFlag>
    {
        public Type On(in Input.RevealCover input)
        {
            Output(new Output.CoverRevealed());
            return To<Revealed>();
        }

        public Type On(in Input.RaiseFlag input)
        {
            Output(new Output.FlagRaised());
            return To<Flagged>();
        }
    }

    public record Revealed : CellState, IGet<Input.PutOnCover>, IGet<Input.MarkError>
    {
        public Type On(in Input.PutOnCover input)
        {
            Output(new Output.CoverPutOn());
            return To<Covered>();
        }

        public Type On(in Input.MarkError input)
        {
            // 守卫：仅在引爆的是雷时才进入 BombRevealed；守卫不过则无输出、不切换
            if (!Context.HasBomb)
                return ToSelf();

            Output(new Output.MarkedAsBombRevealed());
            return To<BombRevealed>();
        }
    }

    public record Flagged : CellState, IGet<Input.PutDownFlag>, IGet<Input.MarkError>
    {
        public Type On(in Input.PutDownFlag input)
        {
            Output(new Output.FlagPutDown());
            return To<Covered>();
        }

        public Type On(in Input.MarkError input)
        {
            // 守卫：错旗（无雷）才进入 WrongFlagged；守卫不过则无输出、不切换
            if (Context.HasBomb)
                return ToSelf();

            Output(new Output.MarkedAsWrongFlagged());
            return To<WrongFlagged>();
        }
    }

    public record BombRevealed : CellState;

    public record WrongFlagged : CellState;
}
