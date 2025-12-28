using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.SkinSystem;

public static class GridSkinExtensions
{
    extension(GridSkin skin)
    {
        public string Name => skin.Key.ToString();

        public Texture2D Texture => SnekUtility.LoadTexture(skin.TexturePath);
    }

    extension(SkinKey skinKey)
    {
        public static SkinKey FromLong(long index) => (SkinKey)index;
    }
}