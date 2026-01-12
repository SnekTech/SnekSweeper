using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace BasicTests.CellTests;

public class CellStateTests
{
    static readonly MockHumbleCell MockHumbleCell = new();

    [Test]
    public async Task is_at_covered_state_after_initAsync()
    {
        var cell = await CreateAndInitCellAsync();

        await Assert.That(cell.IsCovered).IsTrue();
    }

    [Test]
    public async Task is_at_revealed_state_after_revealing()
    {
        var cell = await CreateAndInitCellAsync();

        await cell.RevealAsync();

        await Assert.That(cell.IsRevealed).IsTrue();
    }

    [Test]
    public async Task is_flagged_if_switch_flag_on_covered_cell()
    {
        var cell = await CreateAndInitCellAsync();

        await cell.SwitchFlagAsync();

        await Assert.That(cell.IsFlagged).IsTrue();
    }

    [Test]
    public async Task is_covered_if_switch_flag_on_flagged_cell()
    {
        var cell = await CreateAndInitCellAsync();

        await cell.SwitchFlagAsync();
        await Assert.That(cell.IsFlagged).IsTrue();
        await cell.SwitchFlagAsync();
        await Assert.That(cell.IsCovered).IsTrue();
    }

    [Test]
    public async Task is_still_revealed_if_switch_flag_on_revealed_cell()
    {
        var cell = await CreateAndInitCellAsync();

        await cell.RevealAsync();
        await Assert.That(cell.IsRevealed).IsTrue();

        await cell.SwitchFlagAsync();
        await Assert.That(cell.IsRevealed).IsTrue();
    }

    [Test]
    public async Task is_still_revealed_if_reveal_on_revealed_cell()
    {
        var cell = await CreateAndInitCellAsync();

        await cell.RevealAsync();
        await Assert.That(cell.IsRevealed).IsTrue();
        
        await cell.RevealAsync();
        await Assert.That(cell.IsRevealed).IsTrue();
    }

    [Test]
    public async Task is_still_flagged_if_reveal_on_flagged_cell()
    {
        var cell = await CreateAndInitCellAsync();

        await cell.SwitchFlagAsync();
        await Assert.That(cell.IsFlagged).IsTrue();
        await cell.RevealAsync();
        await Assert.That(cell.IsFlagged).IsTrue();
    }

    static async Task<Cell> CreateAndInitCellAsync()
    {
        var cell = new Cell(MockHumbleCell, GridIndex.Zero);
        await cell.InitAsync(0);
        return cell;
    }
}