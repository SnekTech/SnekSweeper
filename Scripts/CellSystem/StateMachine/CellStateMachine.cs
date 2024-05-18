using System;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine : StateMachine<CellState, Cell>
{
    public readonly CellState CachedCoveredState;
    public readonly CellState CachedRevealedState;
    public readonly CellState CachedFlaggedState;

    public CellStateMachine(Cell context) : base(context)
    {
        CachedCoveredState = new CoveredState(this);
        CachedRevealedState = new RevealedState(this);
        CachedFlaggedState = new FlaggedState(this);
    }

    public void Reveal()
    {
        CurrentState.Reveal();
    }

    public void Flag()
    {
        CurrentState.Flag();
    }
}