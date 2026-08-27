using Chickensoft.LogicBlocks;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    /// <summary>
    /// 关卡初始化：RegularStart/FromRunRecord 等待首次输入触发布雷；
    /// FromGridSnapshot 续局在进入时即恢复棋盘。记录起始信息并发出 InitializeGrid 效果，
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
                    Output(new Output.InitializeGrid(source, FirstInput: null));
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

            // 续局已提前初始化，此状态不再接收输入（点击由 Running 处理）
            if (source is FromGridSnapshot)
                return ToSelf();

            // 首次输入已接受，等待初始化完成；期间忽略后续输入
            if (Get<GridLogic.Data>().PendingFirstInput is not null)
                return ToSelf();

            if (source is FromRunRecord fromRecord && firstInput.Index != fromRecord.RunRecord.StartIndex)
                return ToSelf();
            if (source is RegularStart && firstInput is not PrimaryReleased)
                return ToSelf();

            Get<GridLogic.Data>().PendingFirstInput = firstInput;
            Context.RunRecorder.MarkRunStartInfo(new RunStartInfo(DateTime.Now, firstInput.Index));
            Output(new Output.InitializeGrid(source, firstInput));
            return ToSelf();
        }

        public Type On(in Input.InitCompleted input) => To<GameRunning>();
    }
}
