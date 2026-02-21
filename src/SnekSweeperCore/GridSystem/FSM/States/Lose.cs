using SnekSweeperCore.GameMode;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public class Lose(GridStateMachine stateMachine, GameLose gameLose) : GridState(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        var recentRecord = RunRecorder.GenerateRecentRecord(false, gameLose.Bombs);
        RunRecorder.SaveRecord(recentRecord);

        await MarkPlayerErrorsAsync();

        var choice = await Context.LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct);
        Action handleChoiceAction = choice switch
        {
            PopupChoiceOnLose.Retry => HandleRetry,
            PopupChoiceOnLose.NewGame => Context.LevelOrchestrator.NewGame,
            PopupChoiceOnLose.Leave => Context.LevelOrchestrator.BackToMainMenu,
            _ => delegate { },
        };
        handleChoiceAction();
        return;


        void HandleRetry()
        {
            Context.LevelOrchestrator.Retry(recentRecord);
        }
    }

    Task MarkPlayerErrorsAsync() => Task.WhenAll(gameLose.CellsInThisBatch.Select(cell => cell.MarkErrorAsync()));
}