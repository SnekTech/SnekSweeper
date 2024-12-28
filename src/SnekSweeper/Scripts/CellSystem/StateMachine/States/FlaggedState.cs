namespace SnekSweeper.CellSystem.StateMachine.States;

public class FlaggedState : CellState
{
    public FlaggedState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnEnter()
    {
        Cell.Flag.Raise();
    }

    public override void OnExit()
    {
        Cell.Flag.PutDown();
    }

    public override void SwitchFlag()
    {
        StateMachine.ChangeState<CoveredState>();
    }
}