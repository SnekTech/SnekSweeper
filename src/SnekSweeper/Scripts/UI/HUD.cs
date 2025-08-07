using SnekSweeper.Autoloads;
using SnekSweeper.Combo;
using SnekSweeper.GridSystem;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI;

[SceneTree]
public partial class HUD : CanvasLayer, ISceneScript
{
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;
    private readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    public override void _EnterTree()
    {
        _gridEventBus.BombCountChanged += OnBombCountChanged;
        _gridEventBus.FlagCountChanged += OnFlagCountChanged;
        _gridEventBus.BatchRevealed += OnBatchRevealed;
        UndoButton.Pressed += OnUndoPressed;
    }

    public override void _Ready()
    {
        GridComboComponent.ComboDisplay = new BasicComboDisplay(ComboLevelTextLabel, ComboProgressBar);
    }

    public override void _ExitTree()
    {
        _gridEventBus.BombCountChanged -= OnBombCountChanged;
        _gridEventBus.FlagCountChanged -= OnFlagCountChanged;
        _gridEventBus.BatchRevealed -= OnBatchRevealed;
        UndoButton.Pressed -= OnUndoPressed;
    }

    private void OnBombCountChanged(int bombCount) => BombCountLabel.Text = $"{bombCount} bombs";
    private void OnFlagCountChanged(int flagCount) => FlagCountLabel.Text = $"{flagCount} flags";

    private void OnUndoPressed() => _hudEventBus.EmitUndoPressed();

    private void OnBatchRevealed() => GridComboComponent.IncreaseComboLevel();
}