using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.Level;

[SceneTree]
public partial class HUD : CanvasLayer, ISceneScript
{
    readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;
    readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    public override void _EnterTree()
    {
        _gridEventBus.BombCountChanged += OnBombCountChanged;
        _gridEventBus.FlagCountChanged += OnFlagCountChanged;
        UndoButton.Pressed += OnUndoPressed;
    }

    public override void _ExitTree()
    {
        _gridEventBus.BombCountChanged -= OnBombCountChanged;
        _gridEventBus.FlagCountChanged -= OnFlagCountChanged;
        UndoButton.Pressed -= OnUndoPressed;
    }

    public Task<PopupChoiceOnWin> ShowAndGetChoiceOnWinAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnWinAsync(ct);

    public Task<PopupChoiceOnLose> ShowAndGetChoiceOnLoseAsync(CancellationToken ct = default) =>
        _.PopupLayer.ShowAndGetChoiceOnLoseAsync(ct);

    void OnBombCountChanged(int bombCount) => BombCountLabel.Text = $"{bombCount} bombs";
    void OnFlagCountChanged(int flagCount) => FlagCountLabel.Text = $"{flagCount} flags";

    void OnUndoPressed() => _hudEventBus.EmitUndoPressed();
}