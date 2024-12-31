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

    public override void OnExit()
    {
        Cell.Cover.PutOn();
    }

    public override void PutOnCover()
    {
        ChangeState<CoveredState>();
    }
}