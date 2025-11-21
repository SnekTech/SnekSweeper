using System.Text.Json.Serialization;

namespace SnekSweeper.GridSystem;

public record GridDifficulty(GridDifficultyKey Key, GridDifficultyData DifficultyData)
{
    [JsonIgnore]
    public string Name => Key.ToString();
}

public readonly record struct GridDifficultyData(GridSize Size, int BombCount);

public static class DifficultyFactory
{
    private static readonly GridDifficulty[] BuiltinDifficulties =
    [
        new(GridDifficultyKey.Beginner, new GridDifficultyData(new GridSize(9, 9), 10)),
        new(GridDifficultyKey.Intermediate, new GridDifficultyData(new GridSize(16, 16), 40)),
        new(GridDifficultyKey.Expert, new GridDifficultyData(new GridSize(16, 30), 99)),
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

            GD.Print($"difficulty key {key.ToString()} not found, use {nameof(GridDifficultyKey.Intermediate)} instead");
            return DifficultyCache[GridDifficultyKey.Intermediate];
        }
    }

    extension(GridDifficulty difficulty)
    {
        private void CacheToDict() => DifficultyCache.Add(difficulty.Key, difficulty);
    }
}

public enum GridDifficultyKey
{
    Beginner,
    Intermediate,
    Expert,
}