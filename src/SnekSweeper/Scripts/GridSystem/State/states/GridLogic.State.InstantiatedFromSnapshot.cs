namespace SnekSweeper.GridSystem.State;

public partial class GridLogic
{
    public partial record State
    {
        public record InstantiatedFromSnapshot : State
        {
        }
    }
}