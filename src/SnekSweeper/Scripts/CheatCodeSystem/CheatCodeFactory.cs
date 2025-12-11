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

    static readonly CheatCode[] AvailableCheatCodes =
    [
        new(CheatCodeKey.TransparentCover, new CheatCodeData(
            "Transparent Cover",
            "Make cells transparent Make cells transparent Make cells transparent Make cells transparent  ",
            "res://Art/relic_icon_alpha.png"))
        {
            InitEffect = new SetGridCoverAlpha(0.5f),
        },
        new(CheatCodeKey.Messenger, new CheatCodeData(
            "Messenger",
            "Send message while playing.",
            "res://Art/relic_icon_alpha.png"
        ))
        {
            InitEffect = new SendMessage(),
        },
    ];

    static readonly Dictionary<CheatCodeKey, CheatCode> CheatCodeCollection = [];

    public static IEnumerable<CheatCode> BuiltinCheatCodeList =>
        CheatCodeCollection.Values.OrderBy(cheatCode => cheatCode.Key);

    extension(CheatCode cheatCode)
    {
        private void CacheToDict() => CheatCodeCollection[cheatCode.Key] = cheatCode;
    }
}