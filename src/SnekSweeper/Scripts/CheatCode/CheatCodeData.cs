using System.Text.Json.Serialization;

namespace SnekSweeper.CheatCode;

public class CheatCodeData
{
    public required CheatCodeId Id { get; init; } = CheatCodeId.Empty;
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string IconPath { get; init; }

    [JsonIgnore]
    public Texture2D Icon => SnekUtility.LoadTexture(IconPath);
}

public readonly record struct CheatCodeId(Guid Value)
{
    public static CheatCodeId Empty => new(Guid.Empty);
    public static CheatCodeId NewCheatCodeId => new(Guid.NewGuid());
}