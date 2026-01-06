using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

/**
 * 1. start from menu button:
 * solvable or classic?
 * lay mine after first click
 *
 * 2. start from game run record:
 * bombs are fixed
 * lay mine on Grid creation
 * mark the start index from the run record, lock input to it, unlock after first click
 */
public class GridInitializer(LoadLevelSource loadLevelSource)
{
    bool _hasInitialized;

    public async Task TryHandleFirstPrimaryClickAsync(Grid grid, GridIndex startIndex,
        CancellationToken cancellationToken = default)
    {
        if (_hasInitialized)
            return;

        _hasInitialized = true;
        var bombs = loadLevelSource.LayMineFn(startIndex);

        await grid.InitCellsAsync(startIndex, bombs, cancellationToken);
    }
}