using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record GameRunning : GridState, IGet<Input.PlayerInput>, IGet<Input.InputProcessed>
    {
        public GameRunning()
        {
            this.OnEnter(delegate { Context.HumbleGrid.TriggerInitEffects(); });
        }

        public Type On(in Input.PlayerInput input)
        {
            // 只发效果：异步输入处理交给 Godot 绑定层，完成后以 InputProcessed 回调回来
            Output(new Output.ProcessInput(input.GridInput));
            return ToSelf();
        }

        public Type On(in Input.InputProcessed input)
        {
            var judgedResult = Referee.Judge(input.ProcessResult);
            if (judgedResult is Surviving)
            {
                // 处理期间到达的冗余输入返回 NothingHappens，跳过无意义的快照更新
                if (input.ProcessResult is not NothingHappens)
                    Context.RunRecorder.UpdateGridSnapshot(Context.Grid);
                return ToSelf();
            }

            Get<GridLogic.Data>().EndLevelResult = judgedResult;

            return judgedResult switch
            {
                GameWin => To<Win>(),
                GameLose => To<Lose>(),
                _ => throw new SwitchExpressionException(),
            };
        }
    }
}
