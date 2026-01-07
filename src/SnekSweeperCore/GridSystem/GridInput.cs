namespace SnekSweeperCore.GridSystem;

public abstract record GridInput(GridIndex Index);

public record PrimaryReleased(GridIndex Index) : GridInput(Index);

public record PrimaryDoubleClicked(GridIndex Index) : GridInput(Index);

public record SecondaryReleased(GridIndex Index) : GridInput(Index);