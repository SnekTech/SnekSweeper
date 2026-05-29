using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public abstract record End : State
        {
            protected End()
            {
                this.OnEnter(() =>
                {
                    Context.RunRecorder.ClearSnapshot();
                    Get<Data>().AppRepo.InvokeGameEnded();
                });
            }

            protected GameRunRecord SaveRunRecord(bool winning, bool[,] bombs)
            {
                var runRecord = Context.RunRecorder.GenerateRecentRecord(winning, bombs);
                Context.RunRecorder.SaveRecord(runRecord);
                return runRecord;
            }
        }

        public record Win : End
        {
            public GameWin GameWin { get; set; } = null!;

            public Win()
            {
                this.OnEnter(() =>
                {
                    var recentRecord = SaveRunRecord(true, GameWin.Bombs);

                    Context.HumbleGrid.PlayCongratulationEffects();

                    TriggerWinPopupAsync(recentRecord, LevelExitToken).Forget();
                });
                return;

                async GDTaskVoid TriggerWinPopupAsync(GameRunRecord recentRecord, CancellationToken ct = default)
                {
                    var choice = await Context.LevelOrchestrator.GetPopupChoiceOnWinAsync(ct);
                    Output(new Output.EndGameChoiceOnWin(choice, recentRecord));
                }
            }
        }
    }
}