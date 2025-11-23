namespace SnekSweeper.SkinSystem;

public record GridSkin(SkinKey Key, string TexturePath);

public static class GridSkinExtensions
{
    extension(GridSkin skin)
    {
        public string Name => skin.Key.ToString();

        public Texture2D Texture => SnekUtility.LoadTexture(skin.TexturePath);
    }
}

public enum SkinKey
{
    Classic,
    Mahjong,
}