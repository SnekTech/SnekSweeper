using System.Collections.Immutable;

namespace SnekSweeperCore.GameHistory;

public record History(ImmutableList<GameRunRecord> Records)
{
    public static History Empty { get; } = new(ImmutableList<GameRunRecord>.Empty);

    public History Add(GameRunRecord record) => new(Records: Records.Add(record));
    public History Clear() => new(Records: Records.Clear());
}
