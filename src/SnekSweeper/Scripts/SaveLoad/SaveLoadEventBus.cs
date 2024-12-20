using System;

namespace SnekSweeper.SaveLoad;

public static class SaveLoadEventBus
{
    public static event Action? SaveRequested;

    public static void EmitSaveRequested() => SaveRequested?.Invoke();
}