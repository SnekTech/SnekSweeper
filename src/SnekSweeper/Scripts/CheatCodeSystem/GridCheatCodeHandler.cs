using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.CheatCodeSystem;

public static class GridCheatCodeHandler
{
    public static void TriggerCheatCodeInitEffects(this IHumbleGrid humbleGrid, ActivatedCheatCodeSet activatedCheatCodeSet)
    {
        var initGridEffects = CheatCodeFactory.BuiltinCheatCodeList
            .Where(cheatCode => cheatCode.IsActivatedIn(activatedCheatCodeSet))
            .Select(cheatCode => cheatCode.InitEffect);

        foreach (var initEffect in initGridEffects)
        {
            initEffect?.Trigger(humbleGrid);
        }
    }
}