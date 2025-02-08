using System;
using System.Linq;
using Godot;
using SnekSweeper.SaveLoad;
using CheatCodeDictionary = Godot.Collections.Dictionary<string, bool>;

namespace SnekSweeper.CheatCode;

public partial class CheatCodeSaveData : Resource
{
    [Export] private CheatCodeDictionary cheatCodeActivatingStatus = new();

    public void Init(CheatCodeCollection cheatCodeCollection)
    {
        foreach (var key in cheatCodeCollection.CheatCodeResources.Select(cheatCode => cheatCode.Name))
        {
            cheatCodeActivatingStatus[key] = false;
        }
    }

    public void SetCheatCodeStatus(string cheatCodeName, bool isActivated)
    {
        if (!cheatCodeActivatingStatus.ContainsKey(cheatCodeName))
        {
            throw new ArgumentOutOfRangeException($"cheat code: {cheatCodeName} does not exist");
        }

        cheatCodeActivatingStatus[cheatCodeName] = isActivated;
        SaveLoadEventBus.EmitSaveRequested();
    }

    public bool IsCheatCodeActivated(string cheatCodeName) =>
        cheatCodeActivatingStatus.ContainsKey(cheatCodeName) && cheatCodeActivatingStatus[cheatCodeName];
}