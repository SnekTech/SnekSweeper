using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record GameRunning : GridState, IGet<Input.PlayerInput>, IGet<Input.InputProcessed>
    {
        public GameRunning()
        {
            this.OnEnter(delegate
            {
                Context.HumbleGrid.TriggerInitEffects();

                // 消费初始化阶段暂存的首次输入：进入 Running 后直接处理。
                // 注意：不在此消费 PendingFirstInput——首次输入处理完成（InputProcessed）时才构造 startInfo 并创建 OngoingGame。
                var pendingFirstClick = Get<GridLogic.Data>().PendingFirstInput;
                if (pendingFirstClick != null)
                    Output(new Output.ProcessInput(pendingFirstClick));
            });
        }

        public Type On(in Input.PlayerInput input)
        {
            // 只发效果：异步输入处理交给 Godot 绑定层，完成后以 InputProcessed 回调回来
            Output(new Output.ProcessInput(input.GridInput));
            return ToSelf();
        }

        public Type On(in Input.InputProcessed input)
        {
            var data = Get<GridLogic.Data>();
            var judgedResult = Referee.Judge(input.ProcessResult);

            // 首次输入处理完成 = 本局真正开始：startInfo + snapshot 同时就绪，创建 OngoingGame。
            //（首击即负也走这里——先创建再走结束流程，与正常游玩完全一致）
            if (data.PendingFirstInput is { } firstClick)
            {
                data.PendingFirstInput = null;
                Context.RunRecorder.StartOngoingGame(Context.Grid.GetSnapshot(),
                    new RunStartInfo(DateTime.Now, firstClick.Index));
            }
            else if (judgedResult is Surviving && input.ProcessResult is not NothingHappens)
            {
                // 续局/后续：只更新快照，保留最初 startInfo
                Context.RunRecorder.UpdateOngoingGame(Context.Grid.GetSnapshot());
            }

            if (judgedResult is Surviving) return ToSelf();

            data.EndLevelResult = judgedResult;

            return judgedResult switch
            {
                GameWin => To<Win>(),
                GameLose => To<Lose>(),
                _ => throw new SwitchExpressionException(),
            };
        }
    }
}
