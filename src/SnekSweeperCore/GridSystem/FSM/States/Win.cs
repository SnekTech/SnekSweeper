using SnekSweeperCore.GameMode;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class Win(GridStateMachine stateMachine, GameWin gameWin) : GridState(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        var record = RunRecorder.GenerateRecentRecord(true, gameWin.Bombs);
        RunRecorder.SaveRecord(record);

        HumbleGrid.PlayCongratulationEffects();

        var choice = await Context.LevelOrchestrator.GetPopupChoiceOnWinAsync(ct);
        Action handleChoiceAction = choice switch
        {
            NewGame => Context.LevelOrchestrator.NewGame,
            Leave => Context.LevelOrchestrator.BackToMainMenu,
            _ => delegate { },
        };

        handleChoiceAction();
    }
}