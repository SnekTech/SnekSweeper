using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace BasicTests.CellTests;

public class CellStateTests
{
    static readonly MockHumbleCell MockHumbleCell = new();

    [Test]
    public async Task is_at_covered_state_after_initAsync()
    {
        var cell = new Cell(MockHumbleCell, GridIndex.Zero);
        await cell.InitAsync(0);

        await Assert.That(cell.IsCovered).IsTrue();
    }
}