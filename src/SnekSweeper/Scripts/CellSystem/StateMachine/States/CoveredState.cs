namespace SnekSweeper.CellSystem.StateMachine.States;

public class CoveredState : CellState
{
    public CoveredState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnExit()
    {
        Cell.Cover.Reveal();
    }

    public override void Reveal()
    {
        StateMachine.ChangeState(CellStateKey.Revealed);
    }

    public override void SwitchFlag()
    {
        StateMachine.ChangeState(CellStateKey.Flagged);
    }
}