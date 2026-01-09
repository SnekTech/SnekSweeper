namespace SnekSweeperCore.GameMode;

public class MockGrid
{
    public GridState CurrentState { get; private set; } = new BeforeStart();
}

public abstract record GridState;

public sealed record BeforeStart : GridState;

public sealed record Playing : GridState;

public sealed record End(bool IsWinning) : GridState;

// 
