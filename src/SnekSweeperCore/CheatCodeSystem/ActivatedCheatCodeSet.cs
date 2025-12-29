namespace SnekSweeperCore.CheatCodeSystem;

public class ActivatedCheatCodeSet
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public HashSet<CheatCodeKey> ActivatedSet { get; set; } = [];

    public void Add(CheatCodeKey cheatCodeKey)
    {
        ActivatedSet.Add(cheatCodeKey);
    }

    public void Remove(CheatCodeKey cheatCodeKey)
    {
        ActivatedSet.Remove(cheatCodeKey);
    }

    public bool Contains(CheatCodeKey cheatCodeKey) => ActivatedSet.Contains(cheatCodeKey);
}