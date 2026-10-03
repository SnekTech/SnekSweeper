using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;

namespace SnekSweeperCore.GridSystem;

/// <summary>
/// 棋盘：唯一持有格子状态的地方（<see cref="CellState"/> 本身是不可变的值）。
/// </summary>
public class Grid
{
    readonly ICommandRecorder _commandRecorder;

    readonly bool[,] _bombs;
    readonly int[,] _neighborBombCounts;
    readonly CellState[,] _states;

    // todo: 把 commandRecorder 去掉，grid就是逻辑上的grid
    public Grid(GridSize size, ICommandRecorder commandRecorder)
    {
        Size = size;
        _commandRecorder = commandRecorder;

        _bombs = new bool[size.Rows, size.Columns];
        _neighborBombCounts = new int[size.Rows, size.Columns];
        _states = MatrixExtensions.Create(size, _ => CellState.Initial);
    }

    public GridSize Size { get; }

    public bool[,] BombMatrix => _bombs;

    public IEnumerable<GridIndex> Indices => _states.Indices();

    public int BombCount => _bombs.Elements.Count(hasBomb => hasBomb);
    public int FlagCount => _states.Elements.Count(state => state.IsFlagged);

    /// <summary>所有非雷格都翻开了 = 这一局通了。</summary>
    public bool IsResolved => Indices.Where(index => !_bombs.At(index)).All(index => _states.At(index).IsRevealed);

    public CellInfo InfoAt(GridIndex index) => new(index, _bombs.At(index), _neighborBombCounts.At(index));

    public CellState StateAt(GridIndex index) => _states.At(index);

    public void InitCells(bool[,] bombs)
    {
        foreach (var index in Indices)
        {
            _bombs.SetAt(index, bombs.At(index));
            _neighborBombCounts.SetAt(index, index.GetNeighborIndicesWithin(Size).Count(bombs.At));
            _states.SetAt(index, CellState.Initial);
        }
    }

    // 遇到 Irrelevant（结算期瞬时状态）会 throw；当前不可达，因为 GetSnapshot() 只在 Running 期间被调用
    public void RestoreCellStates(GridSnapshot snapshot)
    {
        foreach (var index in Indices)
        {
            _bombs.SetAt(index, snapshot.BombMatrix.At(index));
            _neighborBombCounts.SetAt(index, index.GetNeighborIndicesWithin(Size).Count(snapshot.BombMatrix.At));

            CellState restoredState = snapshot.SnapshotStates.At(index) switch
            {
                CellSnapshotState.Covered => new CellState.Covered(),
                CellSnapshotState.Revealed => new CellState.Revealed(),
                CellSnapshotState.Flagged => new CellState.Flagged(),
                _ => throw new SwitchExpressionException(),
            };

            _states.SetAt(index, restoredState);
        }
    }

    public GridOutcome HandleInput(GridInput input)
    {
        return input switch
        {
            RevealAt => ProcessRevealAt(input.Index),
            ChordAt => ProcessRevealAround(input.Index),
            SwitchFlagAt => ProcessSwitchFlagAt(input.Index),
            _ => throw new SwitchExpressionException(),
        };
    }

    public CellOutcome? ApplyCommand(GridIndex index, CellCommand command)
    {
        var info = InfoAt(index);
        var (nextState, outcome) = _states.At(index).Apply(info, command);
        _states.SetAt(index, nextState);

        return outcome;
    }

    GridOutcome ProcessRevealAt(GridIndex index)
    {
        var cellsToReveal = new HashSet<GridIndex>();
        FindCellsToReveal(index, cellsToReveal);
        return RevealCells(cellsToReveal);
    }

    GridOutcome ProcessRevealAround(GridIndex index)
    {
        if (!CanRevealAround()) return new GridOutcome.NothingHappens();

        var cellsToReveal = new HashSet<GridIndex>();
        foreach (var neighborIndex in index.GetNeighborIndicesWithin(Size))
        {
            FindCellsToReveal(neighborIndex, cellsToReveal);
            cellsToReveal.Add(neighborIndex);
        }

        return RevealCells(cellsToReveal);

        bool CanRevealAround() =>
            _states.At(index).IsRevealed
            && !_bombs.At(index)
            && NeighborFlagCount(index) == NeighborBombCount(index);
    }

    GridOutcome ProcessSwitchFlagAt(GridIndex index)
    {
        var cellOutcome = ApplyCommand(index, new CellCommand.ToggleFlag());

        return cellOutcome is null
            ? new GridOutcome.NothingHappens()
            : new GridOutcome.FlagToggled(cellOutcome);
    }

    GridOutcome RevealCells(HashSet<GridIndex> cellsToReveal)
    {
        if (cellsToReveal.Count == 0) return new GridOutcome.NothingHappens();

        var gridOutcome = _commandRecorder.ExecuteAndRecord(this,
            new CompoundCommand(cellsToReveal.Select(index => new RevealCellCommand(index))));

        return gridOutcome;
    }

    void FindCellsToReveal(GridIndex index, ICollection<GridIndex> cellsToReveal)
    {
        var visited = cellsToReveal.Contains(index);
        if (visited || !_states.At(index).IsCovered) return;

        cellsToReveal.Add(index);

        if (_bombs.At(index) || NeighborBombCount(index) > 0) return;

        foreach (var neighborIndex in index.GetNeighborIndicesWithin(Size))
        {
            FindCellsToReveal(neighborIndex, cellsToReveal);
        }
    }

    int NeighborBombCount(GridIndex index) => _neighborBombCounts.At(index);
    int NeighborFlagCount(GridIndex index) => index.GetNeighborIndicesWithin(Size).Count(i => _states.At(i).IsFlagged);
}