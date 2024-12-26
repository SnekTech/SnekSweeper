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
    
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;

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
    }

    public override void _ExitTree()
    {
        _gridEventBus.BombCountChanged -= OnBombCountChanged;
        _gridEventBus.FlagCountChanged -= OnFlagCountChanged;
    }

    private void OnBombCountChanged(int bombCount)
    {
        bombCountLabel.Text = $"{bombCount} bombs";
    }

    private void OnFlagCountChanged(int flagCount)
    {
        flagCountLabel.Text = $"{flagCount} flags";
    }
}