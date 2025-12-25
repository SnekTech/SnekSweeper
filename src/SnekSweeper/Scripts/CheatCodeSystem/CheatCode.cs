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

        internal bool IsActivatedIn(ActivatedCheatCodeSet activatedCheatCodeSet)
            => activatedCheatCodeSet.Contains(cheatCode.Key);

        internal void SetActivatedIn(ActivatedCheatCodeSet activatedCheatCodeSet, bool activated)
        {
            if (activated)
            {
                activatedCheatCodeSet.Add(cheatCode.Key);
            }
            else
            {
                activatedCheatCodeSet.Remove(cheatCode.Key);
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