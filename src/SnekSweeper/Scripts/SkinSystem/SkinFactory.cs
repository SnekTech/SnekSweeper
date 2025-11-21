namespace SnekSweeper.SkinSystem;

public static class SkinFactory
{
    private static readonly GridSkin[] BuiltinSkins =
    [
        new(SkinKey.Classic, "res://Art/SnekSweeperSpriteSheet.png"),
        new(SkinKey.Mahjong, "res://Art/SnekSweeperSpriteSheet02.png"),
    ];
    private static readonly Dictionary<SkinKey, GridSkin> SkinCache = [];

    static SkinFactory()
    {
        foreach (var builtinSkin in BuiltinSkins)
        {
            builtinSkin.CacheToDict();
        }
    }

    public static IEnumerable<GridSkin> Skins => SkinCache.Values.OrderBy(skin => skin.Key);

    extension(SkinKey key)
    {
        public static SkinKey FromInt(int index) => (SkinKey)index;
        public static SkinKey FromLong(long index) => (SkinKey)index;

        public GridSkin ToSkin()
        {
            SkinCache.TryGetValue(key, out var skin);
            if (skin is not null)
                return skin;

            GD.Print($"grid skin with key {key} not found, use classic instead");
            return SkinCache[SkinKey.Classic];
        }
    }

    extension(GridSkin skin)
    {
        private void CacheToDict() => SkinCache.Add(skin.Key, skin);
    }
}