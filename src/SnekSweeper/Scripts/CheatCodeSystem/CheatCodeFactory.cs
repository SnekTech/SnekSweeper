using SnekSweeperCore.CheatCodeSystem;

namespace SnekSweeper.CheatCodeSystem;

public static class CheatCodeFactory
{
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
            "发消息",
            "res://Art/relic_icon_alpha.png"
        ))
        {
            InitEffect = new SendMessage(),
        },
    ];

    static readonly Dictionary<CheatCodeKey, CheatCode> CheatCodeCache =
        AvailableCheatCodes.ToDictionary(cheatCode => cheatCode.Key);

    public static IEnumerable<CheatCode> BuiltinCheatCodeList =>
        CheatCodeCache.Values.OrderBy(cheatCode => cheatCode.Key);
}