using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;

namespace BasicTests.Utils;

public static class RunRecordFactory
{
    public static readonly FixedTimeProvider FirstClock = new(new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero));
    public static readonly FixedTimeProvider EndClock = new(new DateTimeOffset(2026, 1, 1, 10, 5, 0, TimeSpan.Zero));

    static readonly DateTime FirstAt = FirstClock.GetLocalNow().DateTime;
    static readonly DateTime EndAt = EndClock.GetLocalNow().DateTime;

    public static readonly RunDuration SampleDuration = new(FirstAt, EndAt);
    
    public static GameRunRecord GetSampleWinningRecord() => new(
        SampleDuration,
        true,
        new[,]
        {
            { false, false, true },
            { false, false, false },
            { false, false, false },
        },
        new GridIndex(0, 0));
}