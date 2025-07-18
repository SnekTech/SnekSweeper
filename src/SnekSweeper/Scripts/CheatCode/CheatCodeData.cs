using System.Text.Json.Serialization;
using SnekSweeper.Autoloads;

namespace SnekSweeper.CheatCode;

public class CheatCodeData
{
    public required CheatCodeId Id { get; init; } = CheatCodeId.Empty;
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string IconPath { get; init; }

    [JsonIgnore]
    public Texture2D Icon => SnekUtility.LoadTexture(IconPath);

    [JsonIgnore]
    public bool IsActivated
    {
        get => HouseKeeper.ActivatedCheatCodeSet.Contains(this);
        set
        {
            if (value)
            {
                HouseKeeper.ActivatedCheatCodeSet.Add(this);
            }
            else
            {
                HouseKeeper.ActivatedCheatCodeSet.Remove(this);
            }
        }
    }
}

public readonly record struct CheatCodeId(Guid Value)
{
    public static CheatCodeId Empty => new(Guid.Empty);
}