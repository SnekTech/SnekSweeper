namespace SnekSweeperCore.CheatCodeSystem;

public class ActivatedCheatCodeSet(HashSet<CheatCodeKey> activatedSet)
{
    // ReSharper disable once MemberCanBePrivate.Global
    public IReadOnlySet<CheatCodeKey> ActivatedSet => activatedSet;

    public void Add(CheatCodeKey cheatCodeKey)
    {
        activatedSet.Add(cheatCodeKey);
    }

    public void Remove(CheatCodeKey cheatCodeKey)
    {
        activatedSet.Remove(cheatCodeKey);
    }

    public bool Contains(CheatCodeKey cheatCodeKey) => ActivatedSet.Contains(cheatCodeKey);
}