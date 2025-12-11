namespace SnekSweeper.CheatCodeSystem;

public static class CheatCodeFactory
{
    static CheatCodeFactory()
    {
        foreach (var availableCheatCode in AvailableCheatCodes)
        {
            availableCheatCode.CacheToDict();
        }
    }

    private static readonly CheatCode[] AvailableCheatCodes =
    [
        new(CheatCodeKey.TransparentCover, new CheatCodeData(
            "Transparent Cover",
            "Make cells transparent Make cells transparent Make cells transparent Make cells transparent  ",
            "res://Art/relic_icon_alpha.png"))
        {
            InitEffect = new SetGridCoverAlpha(0.5f),
        },
    ];

    private static readonly Dictionary<CheatCodeKey, CheatCode> CheatCodeCollection = [];

    public static IEnumerable<CheatCode> BuiltinCheatCodeList =>
        CheatCodeCollection.Values.OrderBy(cheatCode => cheatCode.Key);

    extension(CheatCode cheatCode)
    {
        private void CacheToDict() => CheatCodeCollection[cheatCode.Key] = cheatCode;
    }
}