using SnekSweeper.Autoloads;

namespace SnekSweeper.CheatCodeSystem;

record CheatCode(CheatCodeKey Key, CheatCodeData Data)
{
    internal ICheatCodeGridEffect? InitEffect { get; init; }
}

static class CheatCodeExtension
{
    extension(CheatCode cheatCode)
    {
        internal Texture2D Icon => SnekUtility.LoadTexture(cheatCode.Data.IconPath);
        
        internal bool IsActivated
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

record CheatCodeData(string Name, string Description, string IconPath);

enum CheatCodeKey
{
    TransparentCover,
    Messenger,
}