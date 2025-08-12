namespace SnekSweeper.CheatCodeSystem;

public static class CheatCodeFactory
{
    public static readonly CheatCode TransparentCover = new()
    {
        Id = new CheatCodeId(new Guid("ccda89f7-86f6-462d-a318-6f1b3b4a3690")),
        Data = new CheatCodeData(
            "Transparent Cover",
            "Make cells transparent",
            "res://Art/relic_icon_alpha.png"),
    };

    private static readonly Dictionary<CheatCodeId, CheatCode> CheatCodeCollection = new()
    {
        [TransparentCover.Id] = TransparentCover,
    };

    public static IEnumerable<CheatCode> BuiltinCheatCodeList => CheatCodeCollection.Values;

    public static CheatCode? GetCheatCodeById(CheatCodeId id)
    {
        CheatCodeCollection.TryGetValue(id, out var cheatCode);
        return cheatCode;
    }
}