using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameMode;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record GameRunning : GridState, IGet<Input.PlayerInput>, IGet<Input.EndGame>
    {
        bool _isProcessingInput;

        public GameRunning()
        {
            this.OnEnter(delegate { Context.HumbleGrid.TriggerInitEffects(); });
        }

        public Type On(in Input.PlayerInput input)
        {
            if (_isProcessingInput)
                return ToSelf();

            var gridInput = input.GridInput;

            TriggerHandleInputAsync(LevelExitToken).Forget();
            return ToSelf();

            async GDTaskVoid TriggerHandleInputAsync(CancellationToken ct = default)
            {
                _isProcessingInput = true;

                var processResult = await Context.Grid.HandleInputAsync(gridInput, ct);
                var judgedResult = Referee.Judge(processResult);
                Input(new Input.EndGame(judgedResult));

                _isProcessingInput = false;
            }
        }

        public Type On(in Input.EndGame input)
        {
            var judgedResult = input.JudgedResult;
            if (judgedResult is Surviving)
            {
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