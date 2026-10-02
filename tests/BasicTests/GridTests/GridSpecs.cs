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
    public void init_covers_every_cell()
    {
        var (grid, _) = CreateInitializedBoard();

        grid.Indices.Select(index => grid.StateAt(index)).Should().AllBeOfType<CellState.Covered>();
        grid.BombCount.Should().Be(1);
    }

    [Test]
    public void restoring_a_snapshot_rebuilds_the_same_board()
    {
        var source = new Grid(new GridSize(3, 3), new GridEventBus(), new CommandInvoker());
        source.InitCells(Bombs);
        source.HandleInput(new RevealAt(new GridIndex(0, 1))); // 邻居雷数=1, 不会扩散
        source.HandleInput(new SwitchFlagAt(new GridIndex(1, 1)));
        var snapshot = source.GetSnapshot();

        var restored = new Grid(new GridSize(3, 3), new GridEventBus(), new CommandInvoker());
        restored.RestoreCellStates(snapshot);

        foreach (var index in restored.Indices)
        {
            restored.StateAt(index).Should().Be(source.StateAt(index));
            restored.InfoAt(index).Should().Be(source.InfoAt(index)); // 含邻居雷数重算
        }

        restored.BombCount.Should().Be(1);
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

        var batch = result.Should().BeOfType<GridOutcome.BatchRevealed>().Subject;
        batch.Cells.Select(c => c.Info.Index).Should().Contain(new GridIndex(0, 0));
        batch.Cells.Select(c => c.Event).Should().Contain(new CellEvent.CoverRevealed());
    }

    [Test]
    public void revealing_a_bomb_is_an_uncovered_bomb_and_judged_as_lose()
    {
        var board = CreateInitializedBoard();

        var result = board.Grid.HandleInput(new RevealAt(new GridIndex(0, 0)));

        board.Grid.StateAt(new GridIndex(0, 0)).IsRevealed.Should().BeTrue();
        board.Grid.InfoAt(new GridIndex(0, 0)).HasBomb.Should().BeTrue();
        Referee.Judge(board.Grid, result).Should().BeOfType<GameLose>();
    }

    [Test]
    public void switch_flag_toggles_the_flag()
    {
        var (grid, _) = CreateInitializedBoard();
        var index = new GridIndex(1, 1);

        var gridOutcome = grid.HandleInput(new SwitchFlagAt(index));
        grid.StateAt(index).IsFlagged.Should().BeTrue();
        gridOutcome.Should().BeOfType<GridOutcome.FlagToggled>().Which.IsRaised.Should().BeTrue();

        var gridOutcome2 = grid.HandleInput(new SwitchFlagAt(index));
        grid.StateAt(index).IsFlagged.Should().BeFalse();
        gridOutcome2.Should().BeOfType<GridOutcome.FlagToggled>().Which.IsRaised.Should().BeFalse();
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
        Referee.Judge(board.Grid, result).Should().BeOfType<GameWin>();
    }

    static Board CreateInitializedBoard()
    {
        var commandInvoker = new CommandInvoker();
        var grid = new Grid(new GridSize(3, 3), new GridEventBus(), commandInvoker);
        grid.InitCells(Bombs);

        return new Board(grid, commandInvoker);
    }

    readonly record struct Board(Grid Grid, CommandInvoker CommandInvoker);
}