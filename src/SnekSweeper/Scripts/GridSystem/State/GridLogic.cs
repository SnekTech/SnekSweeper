using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using SnekSweeper.GameStateManagement;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class GridLogic:LogicBlock<GridLogic.State>
{
    public record Data
    {
        public LoadLevelSource LoadLevelSource { get; set; } = LoadLevelSource.CreateDefaultRegularStart();
        public required IAppRepo AppRepo { get; init; }
        
        // todo: figure out the cancellation
        public CancellationToken CancellationTokenOnLevelExit { get; set; }
    }
    
    public override Transition GetInitialState() => To<State.PreInstantiated>();
}