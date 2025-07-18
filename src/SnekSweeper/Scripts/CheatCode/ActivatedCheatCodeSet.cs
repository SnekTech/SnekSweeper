using SnekSweeper.SaveLoad;

namespace SnekSweeper.CheatCode;

public class ActivatedCheatCodeSet
{
    private HashSet<CheatCodeId> ActivatedSet { get; } = [];

    public void Add(CheatCodeData cheatCode)
    {
        ActivatedSet.Add(cheatCode.Id);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public void Remove(CheatCodeData cheatCode)
    {
        ActivatedSet.Remove(cheatCode.Id);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public bool Contains(CheatCodeData cheatCode) => ActivatedSet.Contains(cheatCode.Id);
}