namespace SnekSweeper.CellSystem.StateMachine.States;

public class RevealedState : CellState
{
    public RevealedState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override CellStateValue Value => CellStateValue.Revealed;
}