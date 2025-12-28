namespace SnekSweeperCore.SkinSystem;

public record GridSkin(SkinKey Key, string TexturePath);

public enum SkinKey
{
    Classic,
    Mahjong,
}