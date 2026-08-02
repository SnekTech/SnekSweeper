using Chickensoft.LogicBlocks;
using GodotTask;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public abstract record End : GridState
    {
        protected JudgedResult EndLevelResult { get; private set; } = null!;
        
        protected End()
        {
            this.OnEnter(() =>
            {
                Context.RunRecorder.ClearSnapshot();
                Get<GridLogic.Data>().AppRepo.InvokeGameEnded();
                EndLevelResult = Get<GridLogic.Data>().EndLevelResult;
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
        public Win()
        {
            this.OnEnter(() =>
            {
                var gameWin = (GameWin)EndLevelResult;
                var recentRecord = SaveRunRecord(true, gameWin.Bombs);

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