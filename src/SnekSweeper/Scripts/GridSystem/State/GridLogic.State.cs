using Chickensoft.LogicBlocks;
using SnekSweeperCore.GridSystem.FSM;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public abstract partial record State : StateLogic<State>
    {
        protected GridStateContext Context => Get<GridStateContext>();
    }
}