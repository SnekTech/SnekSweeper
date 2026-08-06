using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotTask;
using SnekSweeper.Levels;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class HUD : CanvasLayer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    LevelData LevelData => this.DependOn<LevelData>();

    public void OnResolved()
    {
        var gridEvents = LevelData.GridEventBus;
        gridEvents.BombCountChanged += OnBombCountChanged;
        gridEvents.FlagCountChanged += OnFlagCountChanged;
        UndoButton.Pressed += OnUndoPressed;
    }

    public override void _ExitTree()
    {
        var gridEvents = LevelData.GridEventBus;
        gridEvents.BombCountChanged -= OnBombCountChanged;
        gridEvents.FlagCountChanged -= OnFlagCountChanged;
        UndoButton.Pressed -= OnUndoPressed;
    }

    public GDTask<PopupChoiceOnWin> ShowAndGetChoiceOnWinAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnWinAsync(ct);

    public GDTask<PopupChoiceOnLose> ShowAndGetChoiceOnLoseAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnLoseAsync(ct);

    void OnBombCountChanged(int bombCount) => BombCountLabel.Text = $"{bombCount} bombs";
    void OnFlagCountChanged(int flagCount) => FlagCountLabel.Text = $"{flagCount} flags";

    void OnUndoPressed() => LevelData.GridCommandInvoker.UndoCommandAsync().AsGDTask().Forget();
}