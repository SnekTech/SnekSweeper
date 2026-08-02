using System.Runtime.CompilerServices;
using Chickensoft.LogicBlocks;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public abstract partial record GridState
{
    public record PreInstantiated : GridState, IGet<Input.Init>
    {
        public Type On(in Input.Init input)
        {
            var loadLevelSource = input.LoadLevelSource;
            Get<GridLogic.Data>().LoadLevelSource = loadLevelSource;
            return loadLevelSource switch
            {
                RegularStart regularStart => To<RegularInstantiated>(),
                FromRunRecord fromRunRecord => To<InstantiatedFromRecord>(),
                FromGridSnapshot fromGridSnapshot => To<InstantiatedFromSnapshot>(),
                _ => throw new SwitchExpressionException(),
            };
        }
    }
}