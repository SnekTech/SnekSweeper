using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.GridSystem;

namespace SnekSweeper.UI;

public partial class HUD : CanvasLayer
{
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;
    
    public override void _EnterTree()
    {
        _gridEventBus.FlaggedCellCountChanged += OnGridFlaggedCellCountChanged;
    }

    public override void _ExitTree()
    {
        _gridEventBus.FlaggedCellCountChanged -= OnGridFlaggedCellCountChanged;
    }

    private void OnGridFlaggedCellCountChanged(int flaggedCellCount)
    {
        GD.Print($"{flaggedCellCount} flags on grid");
    }
}