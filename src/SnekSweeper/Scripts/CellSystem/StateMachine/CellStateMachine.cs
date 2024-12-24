using System.Collections.Generic;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine : StateMachine<CellState, Cell>
{
    private readonly Dictionary<CellStateKey, CellState> _states = new();

    public CellStateMachine(Cell context) : base(context)
    {
        _states[CellStateKey.Covered] = new CoveredState(this);
        _states[CellStateKey.Revealed] = new RevealedState(this);
        _states[CellStateKey.Flagged] = new FlaggedState(this);
    }

    public void Reveal()
    {
        CurrentState?.Reveal();
    }

    public void SwitchFlag()
    {
        CurrentState?.SwitchFlag();
    }

    public void SetInitState(CellStateKey cellStateKey)
    {
        base.SetInitState(_states[cellStateKey]);
    }

    public void ChangeState(CellStateKey cellStateKey)
    {
        base.ChangeState(_states[cellStateKey]);
    }

    public bool IsAtState(CellStateKey cellStateKey)
    {
        if (CurrentState == null) return false;

        return CurrentState == _states[cellStateKey];
    }
}