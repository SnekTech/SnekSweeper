using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotTask;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class HUD : CanvasLayer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    public void OnResolved()
    {
        UndoButton.Pressed += OnUndoPressed;
    }

    public override void _ExitTree()
    {
        UndoButton.Pressed -= OnUndoPressed;
    }

    public GDTask<PopupChoiceOnWin> ShowAndGetChoiceOnWinAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnWinAsync(ct);

    public GDTask<PopupChoiceOnLose> ShowAndGetChoiceOnLoseAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnLoseAsync(ct);

    void OnUndoPressed() => UndoRequested?.Invoke();

    public event Action? UndoRequested;

    public void UpdateBombCount(int bombCount) => BombCountLabel.Text = $"{bombCount} bombs";
    public void UpdateFlagCount(int flagCount) => FlagCountLabel.Text = $"{flagCount} flags";
    public void IncreaseCombo() => ComboRankCard.IncreaseCombo();
}