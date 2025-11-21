using System.Text.Json.Serialization;

namespace SnekSweeper.SkinSystem;

public record GridSkin(SkinKey Key, string TexturePath)
{
    [JsonIgnore]
    public string Name => Key.ToString();

    [JsonIgnore]
    public Texture2D Texture => SnekUtility.LoadTexture(TexturePath);
}

public enum SkinKey
{
    Classic,
    Mahjong,
}