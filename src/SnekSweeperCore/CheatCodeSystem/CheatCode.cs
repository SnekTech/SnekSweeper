namespace SnekSweeperCore.CheatCodeSystem;

public record CheatCode(CheatCodeKey Key, CheatCodeData Data)
{
    public ICheatCodeGridEffect? InitEffect { get; init; }
}

public static class CheatCodeExtension
{
    extension(CheatCode cheatCode)
    {
        public bool IsActivatedIn(ActivatedCheatCodeSet activatedCheatCodeSet)
            => activatedCheatCodeSet.Contains(cheatCode.Key);

        public void SetActivatedIn(ActivatedCheatCodeSet activatedCheatCodeSet, bool activated)
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

public record CheatCodeData(string Name, string Description, string IconPath);

public enum CheatCodeKey
{
    TransparentCover,
    Messenger,
}