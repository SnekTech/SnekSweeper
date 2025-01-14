using System.Threading.Tasks;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.CellSystem.StateMachine;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell
{
    private readonly IHumbleCell _humbleCell;
    private readonly CellStateMachine _stateMachine;

    public Cell(IHumbleCell humbleCell, GridIndex gridIndex, bool hasBomb = false)
    {
        _humbleCell = humbleCell;
        _stateMachine = new CellStateMachine(this);

        GridIndex = gridIndex;
        HasBomb = hasBomb;

        _humbleCell.SetPosition(gridIndex);
    }

    public ICover Cover => _humbleCell.Cover;
    public IFlag Flag => _humbleCell.Flag;

    public GridIndex GridIndex { get; }
    public bool HasBomb { get; set; }

    public bool IsCovered => _stateMachine.IsAtState<CoveredState>();
    public bool IsRevealed => _stateMachine.IsAtState<RevealedState>();
    public bool IsFlagged => _stateMachine.IsAtState<FlaggedState>();

    public int NeighborBombCount { get; private set; }

    public async Task InitAsync(int neighborBombCount)
    {
        NeighborBombCount = neighborBombCount;
        _humbleCell.SetContent(HasBomb, NeighborBombCount);
        await _stateMachine.SetInitStateAsync<CoveredState>();
    }

    public Task Reveal() => _stateMachine.HandleCellRequestAsync(CellRequest.RevealCover);

    public Task PutOnCover() => _stateMachine.HandleCellRequestAsync(CellRequest.PutOnCover);

    public Task SwitchFlag() =>
        _stateMachine.HandleCellRequestAsync(IsFlagged ? CellRequest.PutDownFlag : CellRequest.RaiseFlag);
}