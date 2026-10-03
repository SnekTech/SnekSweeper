using Chickensoft.LogicBlocks;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

[StateDiagram]
public abstract partial record GridState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct Init(LoadLevelSource LoadLevelSource);
        public readonly record struct PlayerInput(GridInput GridInput);
        public readonly record struct InputProcessed(GridOutcome Outcome);
        public readonly record struct InitCompleted;
        public readonly record struct Undo;
    }

    public static class Output
    {
        public readonly record struct ProcessInput(GridInput GridInput);
        public readonly record struct LayMinesAt(LoadLevelSource Source, GridInput FirstInput);
        public readonly record struct RestoreGrid(GridSnapshot Snapshot);
        public readonly record struct EndGameChoiceOnWin(PopupChoiceOnWin Choice, GameRunRecord RecentRecord);
        public readonly record struct EndGameChoiceOnLose(PopupChoiceOnLose Choice, GameRunRecord RecentRecord);
        public readonly record struct UndoApplied(GridOutcome Outcome);
    }

    GridStateContext Context => Get<GridStateContext>();
    CancellationToken LevelExitToken => Get<GridLogic.Data>().CancellationTokenOnLevelExit;
}
