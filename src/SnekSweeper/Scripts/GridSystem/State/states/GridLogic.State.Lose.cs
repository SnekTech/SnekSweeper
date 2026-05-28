using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record Lose : End
        {
            public GameLose GameLose { get; set; } = null!;

            public Lose()
            {
                this.OnEnter(() =>
                {
                    var recentRecord = SaveRunRecord(false, GameLose.Bombs);
                    TriggerLoseTasksAsync(recentRecord).Forget();
                });
                return;

                async GDTaskVoid TriggerLoseTasksAsync(GameRunRecord recentRecord, CancellationToken ct = default)
                {
                    await MarkPlayerErrorsAsync(ct);
                    var choice = await Context.LevelOrchestrator.GetPopupChoiceOnLoseAsync(ct);
                    Output(new Output.EndGameChoiceOnLose(choice, recentRecord));
                }
            }

            GDTask MarkPlayerErrorsAsync(CancellationToken ct = default) =>
                GDTask.WhenAll(GameLose.CellsInThisBatch.Select(cell => cell.MarkErrorAsync(ct).AsGDTask()));
        }
    }
}