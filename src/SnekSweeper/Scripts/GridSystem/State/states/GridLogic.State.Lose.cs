using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.CellSystem;
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
                MarkPlayerErrors(gameLose);
                var choice = await Context.LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct);
                Output(new Output.EndGameChoiceOnLose(choice, recentRecord));
            }

            void MarkPlayerErrors(GameLose gameLose)
            {
                foreach (var index in gameLose.CellsInThisBatch)
                {
                    Context.Grid.ApplyCommand(index, new CellCommand.MarkError());
                }
            }
        }
    }
}
