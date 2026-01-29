using SnekSweeperCore.GameMode;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class GameStart(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken ct = default)
    {
        HumbleGrid.TriggerInitEffects();
        return Task.CompletedTask;
    }

    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        var processResult = await Grid.HandleInputAsync(gridInput, ct);
        var judgedResult = Referee.Judge(processResult);

        await (judgedResult switch
        {
            GameWin win => HandleWinAsync(win),
            GameLose lose => HandleLoseAsync(lose),
            _ => Task.CompletedTask,
        });
        return;

        Task HandleWinAsync(GameWin gameWin)
        {
            RunRecorder.CreateAndSaveRecord(true, gameWin.Bombs);
            return ChangeStateAsync<Win>(ct);
        }

        Task HandleLoseAsync(GameLose gameLose)
        {
            RunRecorder.CreateAndSaveRecord(false, gameLose.Bombs);
            return ChangeStateAsync<Lose>(ct);
        }
    }
}