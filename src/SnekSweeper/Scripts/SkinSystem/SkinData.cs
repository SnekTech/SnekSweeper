using System.Text.Json.Serialization;

namespace SnekSweeper.SkinSystem;

public record SkinData(int Id, string Name, string TexturePath)
{
    [JsonIgnore]
    public Texture2D Texture => SnekUtility.LoadTexture(TexturePath);
}