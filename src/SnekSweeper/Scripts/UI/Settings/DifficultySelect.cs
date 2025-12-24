using SnekSweeper.Autoloads;
using SnekSweeperCore.GridSystem.Difficulty;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class DifficultySelect : HBoxContainer
{
    public override void _Ready()
    {
        InitDifficultyOptions();
    }

    public override void _EnterTree()
    {
        DifficultyOptionButton.ItemSelected += OnDifficultySelected;
    }

    public override void _ExitTree()
    {
        DifficultyOptionButton.ItemSelected -= OnDifficultySelected;
    }

    void InitDifficultyOptions()
    {
        DifficultyOptionButton.Clear();
        var difficulties = DifficultyFactory.Difficulties.ToList();
        foreach (var difficulty in difficulties)
        {
            DifficultyOptionButton.AddItem(difficulty.Name, difficulty.Key.ToInt());
        }

        var savedDifficultyIndex =
            difficulties.FindIndex(difficulty => difficulty.Key == HouseKeeper.MainSetting.CurrentDifficultyKey);
        DifficultyOptionButton.Select(savedDifficultyIndex);
    }

    static void OnDifficultySelected(long index)
    {
        HouseKeeper.MainSetting.CurrentDifficultyKey = GridDifficultyKey.FromLong(index);
        HouseKeeper.SaveCurrentPlayerData();
    }
}