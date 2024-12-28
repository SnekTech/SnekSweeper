using System;
using System.Collections.Generic;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine : StateMachine<CellState, Cell>
{
    public CellStateMachine(Cell context) : base(context)
    {
        StateInstances[typeof(CoveredState)] = new CoveredState(this);
        StateInstances[typeof(RevealedState)] = new RevealedState(this);
        StateInstances[typeof(FlaggedState)] = new FlaggedState(this);
    }

    public void Reveal()
    {
        CurrentState?.Reveal();
    }

    public void SwitchFlag()
    {
        CurrentState?.SwitchFlag();
    }
}