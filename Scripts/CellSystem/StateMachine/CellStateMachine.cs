using System;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine : StateMachine<CellState, Cell>
{
    public readonly CoveredState CachedCoveredState;
    public readonly RevealedState CacheRevealedState;

    public CellStateMachine(Cell context) : base(context)
    {
        CachedCoveredState = new CoveredState(this);
        CacheRevealedState = new RevealedState(this);
    }

    public CellStateValue CurrentStateValue => CurrentState.Value;
}