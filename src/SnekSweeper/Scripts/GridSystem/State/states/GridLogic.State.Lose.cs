using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;

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

            async GDTaskVoid TriggerLoseTasksAsync(GameLose gameLose, GameRunRecord recentRecord,
                CancellationToken ct = default)
            {
                MarkPlayerErrors(gameLose);
                var choice = await Context.LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct);
                Output(new Output.EndGameChoiceOnLose(choice, recentRecord));
            }

            void MarkPlayerErrors(GameLose gameLose)
            {
                var outcomeList = gameLose.CellsInThisBatch
                    .Select(index => Context.Grid.ApplyCommand(index, new CellCommand.MarkError()))
                    .OfType<CellOutcome>();
                Context.HumbleGrid.ApplyGridOutcome(new GridOutcome.ErrorsMarked([.. outcomeList]));
            }
        }
    }
}