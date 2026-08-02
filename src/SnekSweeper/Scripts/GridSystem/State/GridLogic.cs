using Chickensoft.LogicBlocks;
using SnekSweeper.GameStateManagement;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic : LogicBlock
{
    public record Data
    {
        public LoadLevelSource LoadLevelSource { get; set; } = LoadLevelSource.CreateDefaultRegularStart();
        public required IAppRepo AppRepo { get; init; }

        public CancellationToken CancellationTokenOnLevelExit { get; init; }

        public JudgedResult EndLevelResult { get; set; } = Surviving.Instance;
    }

    public GridLogic()
    {
        Set(new GridState.GameRunning());
        Set(new GridState.InstantiatedFromRecord());
        Set(new GridState.InstantiatedFromSnapshot());
        Set(new GridState.Lose());
        Set(new GridState.PreInstantiated());
        Set(new GridState.RegularInstantiated());
        Set(new GridState.Win());
    }
}