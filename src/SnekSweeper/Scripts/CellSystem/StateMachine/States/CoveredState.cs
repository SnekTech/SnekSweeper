namespace SnekSweeper.CellSystem.StateMachine.States;

public class CoveredState : CellState
{
    public CoveredState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Reveal()
    {
        ChangeState<RevealedState>();
    }

    public override void SwitchFlag()
    {
        ChangeState<FlaggedState>();
    }
}