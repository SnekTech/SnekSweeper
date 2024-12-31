using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.GridSystem;

namespace SnekSweeper.UI;

[Scene]
public partial class HUD : CanvasLayer
{
    [Node] private Label flagCountLabel = null!;
    [Node] private Label bombCountLabel = null!;
    [Node] private Button undoButton = null!;
    
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;
    private readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _EnterTree()
    {
        _gridEventBus.BombCountChanged += OnBombCountChanged;
        _gridEventBus.FlagCountChanged += OnFlagCountChanged;
        undoButton.Pressed += OnUndoPressed;
    }

    public override void _ExitTree()
    {
        _gridEventBus.BombCountChanged -= OnBombCountChanged;
        _gridEventBus.FlagCountChanged -= OnFlagCountChanged;
        undoButton.Pressed -= OnUndoPressed;
    }

    private void OnBombCountChanged(int bombCount)
    {
        bombCountLabel.Text = $"{bombCount} bombs";
    }

    private void OnFlagCountChanged(int flagCount)
    {
        flagCountLabel.Text = $"{flagCount} flags";
    }

    private void OnUndoPressed()
    {
        _hudEventBus.EmitUndoPressed();
    }
}