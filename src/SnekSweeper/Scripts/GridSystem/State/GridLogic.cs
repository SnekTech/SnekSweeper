using Chickensoft.LogicBlocks;
using SnekSweeper.GameStateManagement;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;
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

        /// <summary>初始化阶段暂存的首次输入，进入 Running 后消费。</summary>
        public GridInput? PendingFirstInput { get; set; }
    }

    public GridLogic()
    {
        Set(new GridState.GameRunning());
        Set(new GridState.Initializing());
        Set(new GridState.Lose());
        Set(new GridState.PreInstantiated());
        Set(new GridState.Win());
    }
}
