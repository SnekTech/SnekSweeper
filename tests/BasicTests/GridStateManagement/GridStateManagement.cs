using SnekSweeperCore.GameMode;

namespace BasicTests.GridStateManagement;

class GridStateManagement
{
    [Test]
    public async Task be_BeforeStart_state_on_creation()
    {
        var grid = new MockGrid();

        var state = grid.CurrentState;

        await Assert.That(state).IsTypeOf<BeforeStart>();
    }
}