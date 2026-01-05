namespace SnekSweeperCore.SkinSystem;

public static class SkinFactory
{
    static readonly GridSkin[] BuiltinSkins =
    [
        new(SkinKey.Classic, "res://Art/SnekSweeperSpriteSheet.png"),
        new(SkinKey.Mahjong, "res://Art/SnekSweeperSpriteSheet02.png"),
    ];

    static readonly Dictionary<SkinKey, GridSkin> SkinCache = BuiltinSkins.ToDictionary(skin => skin.Key);

    public static IEnumerable<GridSkin> Skins => SkinCache.Values.OrderBy(skin => skin.Key);

    extension(SkinKey key)
    {
        public GridSkin ToSkin()
        {
            SkinCache.TryGetValue(key, out var skin);
            return skin ?? SkinCache[SkinKey.Classic];
        }
    }
}