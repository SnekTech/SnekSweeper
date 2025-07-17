namespace SnekSweeper.SkinSystem;

public record SkinData(string Name, string TexturePath) : ISkin
{
    public Texture2D Texture => SnekUtility.LoadTexture(TexturePath);
}