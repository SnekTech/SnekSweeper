namespace SnekSweeperCore.GridSystem;

public abstract record GridInput(GridIndex Index);

public sealed record PrimaryReleased(GridIndex Index) : GridInput(Index);

public sealed record PrimaryDoubleClicked(GridIndex Index) : GridInput(Index);

public sealed record SecondaryReleased(GridIndex Index) : GridInput(Index);