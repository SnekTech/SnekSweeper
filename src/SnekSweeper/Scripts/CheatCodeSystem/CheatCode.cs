using SnekSweeper.Autoloads;

namespace SnekSweeper.CheatCodeSystem;

public record CheatCode(CheatCodeKey Key, CheatCodeData Data)
{
    public ICheatCodeGridEffect? InitEffect { get; init; }
}

public static class CheatCodeExtension
{
    extension(CheatCode cheatCode)
    {
        public Texture2D Icon => SnekUtility.LoadTexture(cheatCode.Data.IconPath);
        
        public bool IsActivated
        {
            get => HouseKeeper.ActivatedCheatCodeSet.Contains(cheatCode.Key);
            set
            {
                if (value)
                {
                    HouseKeeper.ActivatedCheatCodeSet.Add(cheatCode.Key);
                }
                else
                {
                    HouseKeeper.ActivatedCheatCodeSet.Remove(cheatCode.Key);
                }
            }
        }
    }
}

public record CheatCodeData(string Name, string Description, string IconPath);

public enum CheatCodeKey
{
    TransparentCover,
}