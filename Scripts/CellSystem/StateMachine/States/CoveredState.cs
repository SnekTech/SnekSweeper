﻿namespace SnekSweeper.CellSystem.StateMachine.States;

public class CoveredState : CellState
{
    public CoveredState(CellStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Reveal()
    {
        StateMachine.ChangeState(StateMachine.CachedRevealedState);
    }

    public override void Flag()
    {
        StateMachine.ChangeState(StateMachine.CachedFlaggedState);
    }
}