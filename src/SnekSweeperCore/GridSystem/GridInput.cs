using SnekSweeperCore.CellSystem;

namespace SnekSweeperCore.GridSystem;

public abstract record GridInput(GridIndex Index);

public sealed record PrimaryReleased(GridIndex Index) : GridInput(Index);

public sealed record PrimaryDoubleClicked(GridIndex Index) : GridInput(Index);

public sealed record SecondaryReleased(GridIndex Index) : GridInput(Index);

public abstract record GridInputProcessResult;

public sealed record BatchRevealed(Grid Grid, List<Cell> BombCellsRevealed) : GridInputProcessResult;

public sealed record FlagSwitched : GridInputProcessResult
{
    public static FlagSwitched Instance { get; } = new();
}

public sealed record NothingHappens : GridInputProcessResult
{
    public static NothingHappens Instance { get; } = new();
}