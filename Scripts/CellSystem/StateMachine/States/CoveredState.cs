namespace SnekSweeper.CellSystem.StateMachine.States;

public class CoveredState : CellState
{
    public CoveredState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnPrimaryReleased()
    {
        StateMachine.ChangeState(StateMachine.CachedRevealedState);
    }
}