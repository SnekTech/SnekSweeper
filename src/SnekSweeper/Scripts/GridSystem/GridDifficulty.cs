namespace SnekSweeper.GridSystem;

public record GridDifficulty(int Id, string Name, GridDifficultyData DifficultyData);

public readonly record struct GridDifficultyData(GridSize Size, int BombCount);

public static class DifficultyFactory
{
    static DifficultyFactory()
    {
        Beginner.AddToBuiltinCollection();
        Intermediate.AddToBuiltinCollection();
        Expert.AddToBuiltinCollection();
    }
    
    public static readonly GridDifficulty Beginner = new(0, nameof(Beginner),
        new GridDifficultyData(new GridSize(9, 9), 10));
    public static readonly GridDifficulty Intermediate = new(1, nameof(Intermediate),
        new GridDifficultyData(new GridSize(16, 16), 40));
    public static readonly GridDifficulty Expert = new(2, nameof(Expert),
        new GridDifficultyData(new GridSize(16, 30), 99));

    private static readonly Dictionary<int, GridDifficulty> BuiltinDifficulties = [];

    public static List<GridDifficulty> Difficulties => BuiltinDifficulties.Values.ToList();

    public static GridDifficulty GetDifficultyById(int id)
    {
        BuiltinDifficulties.TryGetValue(id, out var difficulty);
        return difficulty ?? Beginner;
    }

    private static void AddToBuiltinCollection(this GridDifficulty difficulty) =>
        BuiltinDifficulties.Add(difficulty.Id, difficulty);
}