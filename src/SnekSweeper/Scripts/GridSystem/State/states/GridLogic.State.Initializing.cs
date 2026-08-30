using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    /// <summary>
    /// 新局/重试初始化：等待首次输入触发布雷（LayMinesAt）；
    /// 异步初始化完成后以 InitCompleted 进入 Running。（续局走 Resuming）
    /// </summary>
    public record Initializing : GridState, IGet<Input.PlayerInput>, IGet<Input.InitCompleted>
    {
        public Initializing()
        {
            this.OnEnter(delegate
            {
                var source = Get<GridLogic.Data>().LoadLevelSource;

                if (source is FromRunRecord fromRecord)
                    Context.HumbleGrid.GridCursor.LockTo(fromRecord.RunRecord.StartIndex, Context.Grid.Size);
            });

            this.OnExit(delegate
            {
                if (Get<GridLogic.Data>().LoadLevelSource is FromRunRecord)
                    Context.HumbleGrid.GridCursor.Unlock();
            });
        }

        public Type On(in Input.PlayerInput input)
        {
            var source = Get<GridLogic.Data>().LoadLevelSource;
            var firstInput = input.GridInput;

            if (!AcceptsFirstInput(source, firstInput))
                return ToSelf();

            Get<GridLogic.Data>().PendingFirstInput = firstInput;
            Output(new Output.LayMinesAt(source, firstInput));
            return ToSelf();
        }

        public Type On(in Input.InitCompleted input) => To<GameRunning>();

        /// <summary>首次输入接受策略：新局只收左键、重试只收记录开始格；续局与已接受后均忽略。</summary>
        bool AcceptsFirstInput(LoadLevelSource source, GridInput first) =>
            Get<GridLogic.Data>().PendingFirstInput is null && source switch
            {
                FromRunRecord fromRecord => first.Index == fromRecord.RunRecord.StartIndex,
                RegularStart => first is PrimaryReleased,
                _ => throw new SwitchExpressionException(),
            };
    }
}
