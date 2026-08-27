using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    /// <summary>
    /// 关卡初始化：RegularStart/FromRunRecord 等待首次输入触发布雷（LayMinesAt）；
    /// FromGridSnapshot 续局在进入时即恢复棋盘（RestoreGrid）。
    /// 异步初始化完成后以 InitCompleted 进入 Running。
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

                // 续局：布雷矩阵固定，进入即恢复棋盘，避免「全部盖着 + 盲点踩雷」的体验
                if (source is FromGridSnapshot snapshot)
                {
                    Context.RunRecorder.MarkRunStartInfo(snapshot.StartInfo);
                    Output(new Output.RestoreGrid(snapshot));
                }
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
            Context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, firstInput.Index));
            Output(new Output.LayMinesAt(source, firstInput));
            return ToSelf();
        }

        public Type On(in Input.InitCompleted input) => To<GameRunning>();

        /// <summary>首次输入接受策略：新局只收左键、重试只收记录开始格；续局与已接受后均忽略。</summary>
        bool AcceptsFirstInput(LoadLevelSource source, GridInput first) =>
            Get<GridLogic.Data>().PendingFirstInput is null && source switch
            {
                FromGridSnapshot => false,
                FromRunRecord fromRecord => first.Index == fromRecord.RunRecord.StartIndex,
                RegularStart => first is PrimaryReleased,
                _ => throw new SwitchExpressionException(),
            };
    }
}
