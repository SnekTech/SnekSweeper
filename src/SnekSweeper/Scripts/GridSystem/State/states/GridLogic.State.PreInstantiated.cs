using Chickensoft.LogicBlocks;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record PreInstantiated : GridState, IGet<Input.Init>
    {
        public Type On(in Input.Init input)
        {
            Get<GridLogic.Data>().LoadLevelSource = input.LoadLevelSource;
            return input.LoadLevelSource switch
            {
                FromOngoingGame => To<Resuming>(),
                _ => To<Initializing>(),
            };
        }
    }
}
