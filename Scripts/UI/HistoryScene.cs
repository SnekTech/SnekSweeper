using System.Globalization;
using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper.UI;

public partial class HistoryScene : Node2D
{
    [Export]
    private VBoxContainer _recordsContainer = null!;

    [Export]
    private Button _backToMain = null!;

    public override void _Ready()
    {
        _backToMain.Pressed += () => SceneManager.Instance.GotoScene(ScenePaths.MainScene);

        PopulateRecords();
    }

    private void PopulateRecords()
    {
        foreach (var record in HouseKeeper.History.Records)
        {
            var culture = CultureInfo.CurrentCulture;
            var (startAt, endAt, winning) =
                (record.StartAt.ToString(culture), record.EndAt.ToString(culture), record.Winning);
            var recordString = $"from {startAt} to {endAt}, {winning}";

            var label = new Label();
            label.Text = recordString;
            _recordsContainer.AddChild(label);
        }
    }
}