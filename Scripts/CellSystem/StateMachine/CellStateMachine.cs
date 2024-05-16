using System;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine : StateMachine<CellState, Cell>
{
    public readonly CoveredState CachedCoveredState;
    public readonly RevealedState CachedRevealedState;

    public CellStateMachine(Cell context) : base(context)
    {
        CachedCoveredState = new CoveredState(this);
        CachedRevealedState = new RevealedState(this);
    }

    public CellStateValue CurrentStateValue => CurrentState.Value;
}