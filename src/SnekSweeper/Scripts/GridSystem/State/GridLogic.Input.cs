using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public static class Input
    {
        public readonly record struct Init(LoadLevelSource LoadLevelSource);
        public readonly record struct StartLevel;
        public readonly record struct PlayerInput(GridInput GridInput);
        public readonly record struct EndGame(JudgedResult JudgedResult);
    }

    public static class Output
    {
        public readonly record struct EndGameChoiceOnWin(PopupChoiceOnWin Choice, GameRunRecord RecentRecord);
        public readonly record struct EndGameChoiceOnLose(PopupChoiceOnLose Choice, GameRunRecord RecentRecord);
    }
}