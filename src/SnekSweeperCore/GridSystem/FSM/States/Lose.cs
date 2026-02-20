using SnekSweeperCore.GameMode;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public class Lose(GridStateMachine stateMachine, GameLose gameLose) : GridState(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        var recentRecord = RunRecorder.GenerateRecentRecord(false, gameLose.Bombs);
        RunRecorder.SaveRecord(recentRecord);

        MarkPlayerErrors();

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

    void MarkPlayerErrors()
    {
        var wrongFlaggedCells = gameLose.CellsInThisBatch.Where(cell => cell is { IsFlagged: true, HasBomb: false });
        foreach (var bombCell in wrongFlaggedCells)
        {
            bombCell.HumbleCell.MarkAsWrongFlagged();
        }

        var revealedBombCells = gameLose.CellsInThisBatch.Where(cell => cell.HasBomb);
        foreach (var revealedBombCell in revealedBombCells)
        {
            revealedBombCell.HumbleCell.MarkAsRevealedBomb();
        }
    }
}