namespace SnekSweeper.GridSystem.Difficulty;

public static class DifficultyFactory
{
    private static readonly GridDifficulty[] BuiltinDifficulties =
    [
        A.GridDifficulty
            .WithKey(GridDifficultyKey.Beginner)
            .WithSize(new GridSize(9, 9))
            .WithBombCount(10),
        A.GridDifficulty
            .WithKey(GridDifficultyKey.Intermediate)
            .WithSize(new GridSize(16, 16))
            .WithBombCount(40),
        A.GridDifficulty
            .WithKey(GridDifficultyKey.Expert)
            .WithSize(new GridSize(16, 30))
            .WithBombCount(99),
    ];
    private static readonly Dictionary<GridDifficultyKey, GridDifficulty> DifficultyCache = [];

    static DifficultyFactory()
    {
        foreach (var builtinDifficulty in BuiltinDifficulties)
        {
            builtinDifficulty.CacheToDict();
        }
    }

    public static IEnumerable<GridDifficulty> Difficulties =>
        DifficultyCache.Values.OrderBy(difficulty => difficulty.Key);

    extension(GridDifficultyKey key)
    {
        public static GridDifficultyKey FromLong(long index) => (GridDifficultyKey)index;
        public int ToInt() => (int)key;

        public GridDifficulty ToDifficulty()
        {
            DifficultyCache.TryGetValue(key, out var difficulty);
            if (difficulty is not null)
                return difficulty;

            GD.Print(
                $"difficulty key {key.ToString()} not found, use {nameof(GridDifficultyKey.Intermediate)} instead");
            return DifficultyCache[GridDifficultyKey.Intermediate];
        }
    }

    extension(GridDifficulty difficulty)
    {
        private void CacheToDict() => DifficultyCache.Add(difficulty.Key, difficulty);
    }
}