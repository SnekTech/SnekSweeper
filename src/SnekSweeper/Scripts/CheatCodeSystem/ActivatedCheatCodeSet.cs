using SnekSweeper.Autoloads;

namespace SnekSweeper.CheatCodeSystem;

class ActivatedCheatCodeSet
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public HashSet<CheatCodeKey> ActivatedSet { get; set; } = [];

    public void Add(CheatCodeKey cheatCodeKey)
    {
        ActivatedSet.Add(cheatCodeKey);
        HouseKeeper.SaveCurrentPlayerData();
    }

    public void Remove(CheatCodeKey cheatCodeKey)
    {
        ActivatedSet.Remove(cheatCodeKey);
        HouseKeeper.SaveCurrentPlayerData();
    }

    public bool Contains(CheatCodeKey cheatCodeKey) => ActivatedSet.Contains(cheatCodeKey);
}