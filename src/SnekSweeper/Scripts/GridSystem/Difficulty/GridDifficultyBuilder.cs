namespace SnekSweeper.GridSystem.Difficulty;

internal sealed class GridDifficultyBuilder
{
    private GridDifficultyKey _key;
    private GridSize _size;
    private int _bombCount;

    internal GridDifficultyBuilder WithKey(GridDifficultyKey key)
    {
        _key = key;
        return this;
    }

    internal GridDifficultyBuilder WithSize(GridSize size)
    {
        _size = size;
        return this;
    }

    internal GridDifficultyBuilder WithBombCount(int bombCount)
    {
        _bombCount = bombCount;
        return this;
    }

    private GridDifficulty Build() => new(_key, new GridDifficultyData(_size, _bombCount));

    public static implicit operator GridDifficulty(GridDifficultyBuilder builder) => builder.Build();
}

internal static class A
{
    internal static GridDifficultyBuilder GridDifficulty => new();
}