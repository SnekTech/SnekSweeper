namespace SnekSweeper.CellSystem.StateMachine.States;

public class RevealedState : CellState
{
    public RevealedState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnEnter()
    {
        Cell.Cover.Reveal();
    }
}