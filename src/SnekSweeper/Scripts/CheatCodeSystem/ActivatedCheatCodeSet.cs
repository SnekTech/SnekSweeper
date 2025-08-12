using SnekSweeper.SaveLoad;

namespace SnekSweeper.CheatCodeSystem;

public class ActivatedCheatCodeSet
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public HashSet<CheatCodeId> ActivatedSet { get; set; } = [];

    public void Add(CheatCode cheatCode)
    {
        ActivatedSet.Add(cheatCode.Id);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public void Remove(CheatCode cheatCode)
    {
        ActivatedSet.Remove(cheatCode.Id);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public bool Contains(CheatCode cheatCode) => ActivatedSet.Contains(cheatCode.Id);
}