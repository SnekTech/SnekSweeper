using SnekSweeper.GridSystem;

namespace SnekSweeper.CheatCodeSystem;

static class GridCheatCodeHandler
{
    internal static void TriggerCheatCodeInitEffects(this IHumbleGrid humbleGrid)
    {
        var initGridEffects = CheatCodeFactory.BuiltinCheatCodeList
            .Where(cheatCode => cheatCode.IsActivated)
            .Select(cheatCode => cheatCode.InitEffect);

        foreach (var initEffect in initGridEffects)
        {
            initEffect?.Trigger(humbleGrid);
        }
    }
}