namespace SnekSweeper.GridSystem;

public record GridDifficulty(int Id, string Name, GridSize Size, float BombPercent);

public static class DifficultyFactory
{
    public static readonly GridDifficulty Simple = new(0, nameof(Simple), new GridSize(5, 5), 0.1f);
    public static readonly GridDifficulty Medium = new(1, nameof(Medium), new GridSize(10, 10), 0.1f);
    public static readonly GridDifficulty Hard = new(2, nameof(Hard), new GridSize(10, 10), 0.2f);

    private static readonly Dictionary<int, GridDifficulty> BuiltinDifficulties = new()
    {
        [Simple.Id] = Simple,
        [Medium.Id] = Medium,
        [Hard.Id] = Hard,
    };

    public static List<GridDifficulty> Difficulties => BuiltinDifficulties.Values.ToList();

    public static GridDifficulty GetDifficultyById(int id)
    {
        BuiltinDifficulties.TryGetValue(id, out var difficulty);
        return difficulty ?? Medium;
    }
}