using System.Text.Json.Serialization;
using SnekSweeper.Autoloads;

namespace SnekSweeper.CheatCodeSystem;

public class CheatCode
{
    public required CheatCodeId Id { get; init; }
    public required CheatCodeData Data { get; init; }
    public ICheatCodeGridEffect? InitEffect { get; init; }

    [JsonIgnore]
    public Texture2D Icon => SnekUtility.LoadTexture(Data.IconPath);

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

public readonly record struct CheatCodeId(Guid Value);

public readonly record struct CheatCodeData(string Name, string Description, string IconPath);