using System.Collections.Immutable;

namespace SnekSweeperCore.CheatCodeSystem;

public record ActivatedCheatCodeSet(ImmutableHashSet<CheatCodeKey> ActivatedSet)
{
    public static ActivatedCheatCodeSet Empty { get; } = new(ImmutableHashSet<CheatCodeKey>.Empty);

    public ActivatedCheatCodeSet Add(CheatCodeKey key) => new(ActivatedSet: ActivatedSet.Add(key));
    public ActivatedCheatCodeSet Remove(CheatCodeKey key) => new(ActivatedSet: ActivatedSet.Remove(key));
    public bool Contains(CheatCodeKey key) => ActivatedSet.Contains(key);
}
