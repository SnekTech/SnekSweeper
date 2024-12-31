using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine : StateMachine<CellState>
{
    public readonly Cell Cell;
    public CellStateMachine(Cell cell)
    {
        Cell = cell;
    }

    protected override void PopulateStateInstances()
    {
        StateInstances[typeof(CoveredState)] = new CoveredState(this);
        StateInstances[typeof(RevealedState)] = new RevealedState(this);
        StateInstances[typeof(FlaggedState)] = new FlaggedState(this);
    }

    public void Reveal()
    {
        CurrentState?.Reveal();
    }

    public void PutOnCover()
    {
       CurrentState?.PutOnCover();
    }

    public void SwitchFlag()
    {
        CurrentState?.SwitchFlag();
    }
}