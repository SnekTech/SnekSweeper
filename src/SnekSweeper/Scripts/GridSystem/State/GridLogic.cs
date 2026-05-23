using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class GridLogic:LogicBlock<GridLogic.State>
{
    public record Data
    {
        public LoadLevelSource LoadLevelSource { get; set; } = LoadLevelSource.CreateDefaultRegularStart();
    }
    
    public override Transition GetInitialState() => To<State.PreInstantiated>();
}