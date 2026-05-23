using System.Runtime.CompilerServices;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record PreInstantiated : State, IGet<Input.Init>
        {
            public Transition On(in Input.Init input)
            {
                var loadLevelSource = input.LoadLevelSource;
                Get<Data>().LoadLevelSource = loadLevelSource;
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
}