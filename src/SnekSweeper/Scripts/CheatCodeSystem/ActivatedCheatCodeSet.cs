using SnekSweeper.SaveLoad;

namespace SnekSweeper.CheatCodeSystem;

public class ActivatedCheatCodeSet
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public HashSet<CheatCodeKey> ActivatedSet { get; set; } = [];

    public void Add(CheatCodeKey cheatCodeKey)
    {
        ActivatedSet.Add(cheatCodeKey);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public void Remove(CheatCodeKey cheatCodeKey)
    {
        ActivatedSet.Remove(cheatCodeKey);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public bool Contains(CheatCodeKey cheatCodeKey) => ActivatedSet.Contains(cheatCodeKey);
}