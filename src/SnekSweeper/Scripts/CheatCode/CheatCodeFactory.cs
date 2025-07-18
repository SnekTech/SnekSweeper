namespace SnekSweeper.CheatCode;

public static class CheatCodeFactory
{
    public static readonly CheatCodeData Alpha = new()
    {
        Name = nameof(Alpha),
        Description = "Make cells transparent",
        IconPath = "res://Art/relic_icon_alpha.png",
        Id = new CheatCodeId(new Guid("ccda89f7-86f6-462d-a318-6f1b3b4a3690")),
    };

    private static readonly Dictionary<CheatCodeId, CheatCodeData> CheatCodeCollection = new()
    {
        [Alpha.Id] = Alpha,
    };

    public static IEnumerable<CheatCodeData> BuiltinCheatCodeList => CheatCodeCollection.Values;

    public static CheatCodeData? GetCheatCodeById(CheatCodeId id)
    {
        CheatCodeCollection.TryGetValue(id, out var cheatCode);
        return cheatCode;
    }
}