using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record Lose : End
    {

        public Lose()
        {
            this.OnEnter(() =>
            {
                var gameLose = (GameLose)EndLevelResult;
                var recentRecord = SaveRunRecord(false, gameLose.Bombs);
                TriggerLoseTasksAsync(gameLose, recentRecord, LevelExitToken).Forget();
            });
            return;

            async GDTaskVoid TriggerLoseTasksAsync(GameLose gameLose,GameRunRecord recentRecord, CancellationToken ct = default)
            {
                await MarkPlayerErrorsAsync(gameLose, ct);
                var choice = await Context.LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct);
                Output(new Output.EndGameChoiceOnLose(choice, recentRecord));
            }
        }

        static GDTask MarkPlayerErrorsAsync(GameLose gameLose,CancellationToken ct = default) =>
            GDTask.WhenAll(gameLose.CellsInThisBatch.Select(cell => cell.MarkErrorAsync(ct).AsGDTask()));
    }
}