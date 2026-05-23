using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public abstract partial record State : StateLogic<State>;
}