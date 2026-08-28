using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.Settings;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class DifficultySelect : HBoxContainer
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _EnterTree()
    {
        DifficultyOptionButton.ItemSelected += OnDifficultySelected;
    }

    public override void _ExitTree()
    {
        DifficultyOptionButton.ItemSelected -= OnDifficultySelected;
    }

    public void OnResolved()
    {
        DifficultyOptionButton.Clear();
        var difficulties = DifficultyFactory.Difficulties.ToList();
        foreach (var difficulty in difficulties)
        {
            DifficultyOptionButton.AddItem(difficulty.Name, difficulty.Key.ToInt());
        }

        var savedDifficultyIndex =
            difficulties.FindIndex(difficulty => difficulty.Key == SaveData.State.MainSetting.CurrentDifficultyKey);
        DifficultyOptionButton.Select(savedDifficultyIndex);
    }

    void OnDifficultySelected(long index)
    {
        SaveData.UpdateMainSetting(m => m with { CurrentDifficultyKey = GridDifficultyKey.FromLong(index) });
    }
}
