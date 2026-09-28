using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;

namespace SnekSweeperCore.GridSystem;

/// <summary>
/// 棋盘：唯一持有格子状态的地方（<see cref="CellState"/> 本身是不可变的值）。
/// 每次转移完成后当场交给 <see cref="ICellRenderer"/> 渲染，所以不需要把"变化"传出去。
/// </summary>
public class Grid
{
    readonly GridEventBus _gridEventBus;
    readonly ICommandRecorder _commandRecorder;
    readonly ICellRenderer _renderer;

    readonly bool[,] _bombs;
    readonly int[,] _neighborBombCounts;
    readonly CellState[,] _states;

    // todo: 有没有某种方法把 gridEventBus、commandRecorder、renderer 都去掉，grid就是逻辑上的grid
    //   或者说，这么做值得吗？优缺点在于什么？
    public Grid(GridSize size, GridEventBus gridEventBus, ICommandRecorder commandRecorder, ICellRenderer renderer)
    {
        Size = size;
        _gridEventBus = gridEventBus;
        _commandRecorder = commandRecorder;
        _renderer = renderer;

        _bombs = new bool[size.Rows, size.Columns];
        _neighborBombCounts = new int[size.Rows, size.Columns];
        _states = MatrixExtensions.Create(size, _ => CellState.Initial);
    }

    public GridSize Size { get; }

    public bool[,] BombMatrix => _bombs;

    public IEnumerable<GridIndex> Indices => _states.Indices();

    public int BombCount => _bombs.Elements.Count(hasBomb => hasBomb);

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

            _renderer.Render(InfoAt(index), new CellOutcome(CellState.Initial, null));
        }

        _gridEventBus.EmitBombCountChanged(BombCount);
    }

    public GridInputProcessResult HandleInput(GridInput input)
    {
        return input switch
        {
            RevealAt => ProcessRevealAt(input.Index),
            ChordAt => ProcessRevealAround(input.Index),
            SwitchFlagAt => ProcessSwitchFlagAt(input.Index),
            _ => throw new SwitchExpressionException(),
        };
    }

    public CellOutcome ApplyCommand(GridIndex index, CellCommand command)
    {
        var info = InfoAt(index);
        var outcome = _states.At(index).Apply(info, command);
        _states.SetAt(index, outcome.NextState);

        if (outcome.Event is not null) _renderer.Render(info, outcome);

        return outcome;
    }

    GridInputProcessResult ProcessRevealAt(GridIndex index)
    {
        var cellsToReveal = new HashSet<GridIndex>();
        FindCellsToReveal(index, cellsToReveal);
        return RevealCells(cellsToReveal);
    }

    GridInputProcessResult ProcessRevealAround(GridIndex index)
    {
        if (!CanRevealAround()) return NothingHappens.Instance;

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

    GridInputProcessResult ProcessSwitchFlagAt(GridIndex index)
    {
        ApplyCommand(index, new CellCommand.ToggleFlag());
        _gridEventBus.EmitFlagCountChanged(FlagCount);

        return FlagSwitched.Instance;
    }

    GridInputProcessResult RevealCells(HashSet<GridIndex> cellsToReveal)
    {
        if (cellsToReveal.Count == 0) return NothingHappens.Instance;

        _commandRecorder.ExecuteAndRecord(this,
            new CompoundCommand(cellsToReveal.Select(index => new RevealCellCommand(index))));
        _gridEventBus.EmitBatchRevealed();

        return new BatchRevealed(this, cellsToReveal.ToList());
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

    int FlagCount => _states.Elements.Count(state => state.IsFlagged);
    int NeighborBombCount(GridIndex index) => _neighborBombCounts.At(index);
    int NeighborFlagCount(GridIndex index) => index.GetNeighborIndicesWithin(Size).Count(i => _states.At(i).IsFlagged);
}
