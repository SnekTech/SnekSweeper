using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameMode;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record GameRunning : State, IGet<Input.PlayerInput>, IGet<Input.EndGame>
        {
            bool _isProcessingInput;

            public GameRunning()
            {
                this.OnEnter(delegate
                {
                    Context.HumbleGrid.TriggerInitEffects();
                });
            }

            public Transition On(in Input.PlayerInput input)
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

            public Transition On(in Input.EndGame input)
            {
                var judgedResult = input.JudgedResult;
                if (judgedResult is Surviving)
                {
                    Context.RunRecorder.UpdateGridSnapshot(Context.Grid);
                    return ToSelf();
                }

                return judgedResult switch
                {
                    GameWin gameWin => To<Win>().With(win => ((Win)win).GameWin = gameWin),
                    GameLose gameLose => To<Lose>().With(lose => ((Lose)lose).GameLose = gameLose),
                    _ => throw new SwitchExpressionException(),
                };
            }
        }
    }
}