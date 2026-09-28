using AwesomeAssertions;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;

namespace BasicTests.GridTests;

public class GridSpecs
{
    // 3x3，雷只在 (0,0)
    static readonly bool[,] Bombs = new[,]
    {
        { true, false, false },
        { false, false, false },
        { false, false, false },
    };

    [Test]
    public void init_renders_every_cell_as_covered()
    {
        var board = CreateInitializedBoard(keepInitRenders: true);

        board.Renderer.Outcomes.Should().HaveCount(9);
        board.Renderer.Outcomes.Select(outcome => outcome.NextState).Should().AllBeOfType<CellState.Covered>();
        board.Renderer.Outcomes.Select(outcome => outcome.Event).Should().AllSatisfy(@event => @event.Should().BeNull());
        board.Grid.BombCount.Should().Be(1);
    }

    [Test]
    public void info_carries_bomb_and_neighbor_bomb_count()
    {
        var board = CreateInitializedBoard();

        board.Grid.InfoAt(new GridIndex(0, 0)).HasBomb.Should().BeTrue();
        board.Grid.InfoAt(new GridIndex(0, 1)).NeighborBombCount.Should().Be(1);
        board.Grid.InfoAt(new GridIndex(2, 2)).NeighborBombCount.Should().Be(0);
    }

    [Test]
    public void reveal_at_reports_a_batch_and_animates_the_cover()
    {
        var board = CreateInitializedBoard();

        var result = board.Grid.HandleInput(new RevealAt(new GridIndex(0, 0)));

        var batch = result.Should().BeOfType<BatchRevealed>().Subject;
        batch.CellsInThisBatch.Should().Contain(new GridIndex(0, 0));
        board.Renderer.LastEvent.Should().BeOfType<CellEvent.CoverRevealed>();
    }

    [Test]
    public void revealing_a_bomb_is_an_uncovered_bomb_and_judged_as_lose()
    {
        var board = CreateInitializedBoard();

        var result = board.Grid.HandleInput(new RevealAt(new GridIndex(0, 0)));

        board.Grid.StateAt(new GridIndex(0, 0)).IsRevealed.Should().BeTrue();
        board.Grid.InfoAt(new GridIndex(0, 0)).HasBomb.Should().BeTrue();
        Referee.Judge(result).Should().BeOfType<GameLose>();
    }

    [Test]
    public void switch_flag_toggles_the_flag()
    {
        var board = CreateInitializedBoard();
        var index = new GridIndex(1, 1);

        board.Grid.HandleInput(new SwitchFlagAt(index));
        board.Grid.StateAt(index).IsFlagged.Should().BeTrue();
        board.Renderer.LastEvent.Should().BeOfType<CellEvent.FlagRaised>();

        board.Grid.HandleInput(new SwitchFlagAt(index));
        board.Grid.StateAt(index).IsCovered.Should().BeTrue();
        board.Renderer.LastEvent.Should().BeOfType<CellEvent.FlagPutDown>();
    }

    [Test]
    public void undo_puts_the_cover_back()
    {
        var board = CreateInitializedBoard();
        var index = new GridIndex(1, 1);

        board.Grid.HandleInput(new RevealAt(index));
        board.Grid.StateAt(index).IsRevealed.Should().BeTrue();

        board.CommandInvoker.UndoCommand(board.Grid);

        board.Grid.StateAt(index).IsCovered.Should().BeTrue();
    }

    [Test]
    public void revealing_every_safe_cell_is_judged_as_win()
    {
        var board = CreateInitializedBoard();

        var result = board.Grid.HandleInput(new RevealAt(new GridIndex(2, 2)));

        board.Grid.IsResolved.Should().BeTrue();
        Referee.Judge(result).Should().BeOfType<GameWin>();
    }

    // todo: Board 为什么放到测试中了？按原计划，Board应该也是immutable cell设计的一部分
    static Board CreateInitializedBoard(bool keepInitRenders = false)
    {
        var renderer = new RecordingCellRenderer();
        var commandInvoker = new CommandInvoker();
        var grid = new Grid(new GridSize(3, 3), new GridEventBus(), commandInvoker, renderer);
        grid.InitCells(Bombs);
        if (!keepInitRenders) renderer.Clear();

        return new Board(grid, commandInvoker, renderer);
    }

    readonly record struct Board(Grid Grid, CommandInvoker CommandInvoker, RecordingCellRenderer Renderer);

    class RecordingCellRenderer : ICellRenderer
    {
        readonly List<CellOutcome> _outcomes = [];

        public IReadOnlyList<CellOutcome> Outcomes => _outcomes;
        public CellEvent? LastEvent => _outcomes.Count == 0 ? null : _outcomes[^1].Event;

        public void Render(CellInfo info, CellOutcome outcome) => _outcomes.Add(outcome);
        public void Clear() => _outcomes.Clear();
    }
}
