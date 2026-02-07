namespace SnekSweeperCore.GridSystem.Difficulty;

public static class DifficultyFactory
{
    static readonly GridDifficulty[] BuiltinDifficulties =
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
        A.GridDifficulty
            .WithKey(GridDifficultyKey.Test)
            .WithSize(new GridSize(9, 9))
            .WithBombCount(3),
    ];
    static readonly Dictionary<GridDifficultyKey, GridDifficulty> DifficultyCache =
        BuiltinDifficulties.ToDictionary(difficulty => difficulty.Key);

    public static IEnumerable<GridDifficulty> Difficulties =>
        DifficultyCache.Values.OrderBy(difficulty => difficulty.Key);

    extension(GridDifficultyKey key)
    {
        public static GridDifficultyKey FromLong(long index) => (GridDifficultyKey)index;
        public int ToInt() => (int)key;

        public GridDifficulty ToDifficulty()
        {
            DifficultyCache.TryGetValue(key, out var difficulty);
            return difficulty ?? DifficultyCache[GridDifficultyKey.Intermediate];
        }
    }
}