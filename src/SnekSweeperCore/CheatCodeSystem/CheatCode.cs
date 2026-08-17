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

        public ActivatedCheatCodeSet SetActivatedIn(ActivatedCheatCodeSet activatedCheatCodeSet, bool activated) =>
            activated
                ? activatedCheatCodeSet.Add(cheatCode.Key)
                : activatedCheatCodeSet.Remove(cheatCode.Key);
    }
}

public record CheatCodeData(string Name, string Description, string IconPath);

public enum CheatCodeKey
{
    TransparentCover,
    Messenger,
}
