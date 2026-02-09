using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class Win(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        HumbleGrid.PlayCongratulationEffects();

        // todo: figure out cancellation execution order
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