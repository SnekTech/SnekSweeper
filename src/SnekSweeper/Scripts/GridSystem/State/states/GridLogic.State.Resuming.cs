using Chickensoft.LogicBlocks;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    /// <summary>
    /// 续局（FromOngoingGame）：进入即从存档 OngoingGame 恢复棋盘，不拦截首击。
    /// 开局信息全程来自存档 OngoingGame（最初 StartInfo），不新生成。
    /// </summary>
    public record Resuming : GridState, IGet<Input.InitCompleted>
    {
        public Resuming()
        {
            this.OnEnter(delegate
            {
                // PreInstantiated 已按来源路由：只以 FromOngoingGame 进入本状态，直接解构。
                // 违反协议则抛 InvalidCastException（不该发生，直接炸）。
                var ongoing = ((FromOngoingGame)Get<GridLogic.Data>().LoadLevelSource).OngoingGame;
                Output(new Output.RestoreGrid(ongoing.GridSnapshot));
            });
        }

        public Type On(in Input.InitCompleted input) => To<GameRunning>();
    }
}
