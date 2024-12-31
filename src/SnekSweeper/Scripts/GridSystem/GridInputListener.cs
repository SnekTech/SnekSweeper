using System;
using Godot;
using SnekSweeper.Constants;

namespace SnekSweeper.GridSystem;

public partial class GridInputListener : Node2D, IGridInputListener
{
    public event Action<(int i, int j)>? PrimaryReleased;
    public event Action<(int i, int j)>? PrimaryDoubleClicked;
    public event Action<(int i, int j)>? SecondaryReleased;
    public event Action<(int i, int j)>? HoveringGridIndexChanged;
    
    private const int CellSize = CoreStats.CellSizePixels;
    
    private (int i, int j) _hoveringGridIndex = (0, 0);

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouse) return;

        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            var newHoveringGridIndex = GetHoveringGridIndex(eventMouseMotion.Position);
            var hoveringGridIndexHasChanged = newHoveringGridIndex != _hoveringGridIndex;
            if (!hoveringGridIndexHasChanged)
                return;

            _hoveringGridIndex = newHoveringGridIndex;
            HoveringGridIndexChanged?.Invoke(_hoveringGridIndex);
        }
        else if (@event.IsActionReleased(InputActions.Primary))
        {
            PrimaryReleased?.Invoke(_hoveringGridIndex);
        }
        else if (@event is InputEventMouseButton eventMouseButton &&
                 eventMouseButton.IsAction(InputActions.Primary) &&
                 eventMouseButton.DoubleClick)
        {
            PrimaryDoubleClicked?.Invoke(_hoveringGridIndex);
        }
        else if (@event.IsActionReleased(InputActions.Secondary))
        {
            SecondaryReleased?.Invoke(_hoveringGridIndex);
        }
    }

    private (int i, int j) GetHoveringGridIndex(Vector2 globalMousePosition)
    {
        var localMousePosition = globalMousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / CellSize);
        var j = Mathf.FloorToInt(localMousePosition.X / CellSize);
        return (i, j);
    }
}