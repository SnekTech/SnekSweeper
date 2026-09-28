using AwesomeAssertions;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace BasicTests.CellTests;

public class CellApplySpecs
{
    static readonly CellInfo BombCell = new(new GridIndex(0, 0), HasBomb: true, NeighborBombCount: 0);
    static readonly CellInfo SafeCell = new(new GridIndex(0, 1), HasBomb: false, NeighborBombCount: 1);

    [Test]
    public void initial_state_is_covered()
    {
        CellState.Initial.IsCovered.Should().BeTrue();
    }

    [Test]
    public void reveal_cover_moves_covered_to_revealed()
    {
        var outcome = CellState.Initial.Apply(SafeCell, new CellCommand.RevealCover());

        outcome.NextState.IsRevealed.Should().BeTrue();
        outcome.Event.Should().BeOfType<CellEvent.CoverRevealed>();
    }

    [Test]
    public void toggle_flag_raises_flag_on_covered()
    {
        var outcome = CellState.Initial.Apply(SafeCell, new CellCommand.ToggleFlag());

        outcome.NextState.IsFlagged.Should().BeTrue();
        outcome.Event.Should().BeOfType<CellEvent.FlagRaised>();
    }

    [Test]
    public void toggle_flag_puts_down_flag_on_flagged()
    {
        var outcome = new CellState.Flagged().Apply(SafeCell, new CellCommand.ToggleFlag());

        outcome.NextState.IsCovered.Should().BeTrue();
        outcome.Event.Should().BeOfType<CellEvent.FlagPutDown>();
    }

    [Test]
    public void toggle_flag_is_ignored_on_revealed()
    {
        var outcome = new CellState.Revealed().Apply(SafeCell, new CellCommand.ToggleFlag());

        outcome.NextState.IsRevealed.Should().BeTrue();
        outcome.Event.Should().BeNull();
    }

    [Test]
    public void reveal_cover_is_ignored_on_flagged()
    {
        var outcome = new CellState.Flagged().Apply(SafeCell, new CellCommand.RevealCover());

        outcome.NextState.IsFlagged.Should().BeTrue();
        outcome.Event.Should().BeNull();
    }

    [Test]
    public void put_on_cover_moves_revealed_to_covered()
    {
        var outcome = new CellState.Revealed().Apply(SafeCell, new CellCommand.PutOnCover());

        outcome.NextState.IsCovered.Should().BeTrue();
        outcome.Event.Should().BeOfType<CellEvent.CoverPutOn>();
    }

    [Test]
    public void mark_error_reveals_bomb_on_bomb_cell()
    {
        var outcome = new CellState.Revealed().Apply(BombCell, new CellCommand.MarkError());

        outcome.NextState.IsRevealedBomb.Should().BeTrue();
        outcome.Event.Should().BeOfType<CellEvent.RevealedBomb>();
    }

    [Test]
    public void mark_error_is_ignored_on_revealed_safe_cell()
    {
        var outcome = new CellState.Revealed().Apply(SafeCell, new CellCommand.MarkError());

        outcome.NextState.IsRevealed.Should().BeTrue();
        outcome.Event.Should().BeNull();
    }

    [Test]
    public void mark_error_marks_wrong_flag_on_flagged_safe_cell()
    {
        var outcome = new CellState.Flagged().Apply(SafeCell, new CellCommand.MarkError());

        outcome.NextState.IsWrongFlagged.Should().BeTrue();
        outcome.Event.Should().BeOfType<CellEvent.FlagTurnedOutWrong>();
    }

    [Test]
    public void mark_error_is_ignored_on_flagged_bomb_cell()
    {
        var outcome = new CellState.Flagged().Apply(BombCell, new CellCommand.MarkError());

        outcome.NextState.IsFlagged.Should().BeTrue();
        outcome.Event.Should().BeNull();
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void terminal_states_ignore_every_command(bool bombRevealed)
    {
        CellState terminal = bombRevealed ? new CellState.BombRevealed() : new CellState.WrongFlagged();
        CellCommand[] commands =
        [
            new CellCommand.RevealCover(),
            new CellCommand.PutOnCover(),
            new CellCommand.ToggleFlag(),
            new CellCommand.MarkError(),
        ];

        foreach (var command in commands)
        {
            var outcome = terminal.Apply(BombCell, command);

            outcome.NextState.Should().Be(terminal);
            outcome.Event.Should().BeNull();
        }
    }
}
