using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.GridSystem;

namespace SnekSweeper.UI;

[Scene]
public partial class HUD : CanvasLayer
{
    [Node] private Label flagCountLabel = null!;
    
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
        _gridEventBus.FlagCountChanged += OnGridFlagCountChanged;
    }

    public override void _ExitTree()
    {
        _gridEventBus.FlagCountChanged -= OnGridFlagCountChanged;
    }

    private void OnGridFlagCountChanged(int flaggedCellCount)
    {
        flagCountLabel.Text = $"{flaggedCellCount} flags";
    }
}