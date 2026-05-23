using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public static class Input
    {
        public readonly record struct Init(LoadLevelSource LoadLevelSource);
        public readonly record struct InitComplete;
        public readonly record struct PlayerInput(GridInput GridInput);
    }
}