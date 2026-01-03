using SnekSweeperCore.CheatCodeSystem;
using Widgets.CustomExtensions;

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
            "Send message while playing.",
            "res://Art/relic_icon_alpha.png"
        ))
        {
            InitEffect = new SendMessage(),
        },
    ];

    static readonly Dictionary<CheatCodeKey, CheatCode> CheatCodeCache = [];

    public static IEnumerable<CheatCode> BuiltinCheatCodeList =>
        CheatCodeCache.Values.OrderBy(cheatCode => cheatCode.Key);

    static CheatCodeFactory() =>
        CheatCodeCache.AddRange(AvailableCheatCodes.Select(cheatCode => (cheatCode.Key, cheatCode)));
}