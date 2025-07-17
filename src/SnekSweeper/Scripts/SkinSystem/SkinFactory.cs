namespace SnekSweeper.SkinSystem;

public static class SkinFactory
{
    public static readonly SkinData Classic = new(0, nameof(Classic), "res://Art/SnekSweeperSpriteSheet.png");
    public static readonly SkinData Mahjong = new(1, nameof(Mahjong), "res://Art/SnekSweeperSpriteSheet02.png");

    private static readonly Dictionary<int, SkinData> BuiltinSkins = new()
    {
        [Classic.Id] = Classic,
        [Mahjong.Id] = Mahjong,
    };

    public static List<SkinData> Skins => BuiltinSkins.Values.ToList();

    public static SkinData GetSkinById(int id)
    {
        BuiltinSkins.TryGetValue(id, out var skin);
        return skin ?? Classic;
    }
}