using SnekSweeper.CellSystem;
using SnekSweeper.Constants;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

public partial class GridInputListener : Node2D
{
    public event Action<GridInput>? GridInputEmitted;
    public event Action<GridIndex>? HoveringGridIndexChanged;

    GridIndex _hoveringGridIndex = new(0, 0);

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
            EmitGridInput(new PrimaryReleased(_hoveringGridIndex));
        }
        else if (@event is InputEventMouseButton eventMouseButton &&
                 eventMouseButton.IsAction(InputActions.Primary) &&
                 eventMouseButton.DoubleClick)
        {
            EmitGridInput(new PrimaryDoubleClicked(_hoveringGridIndex));
        }
        else if (@event.IsActionReleased(InputActions.Secondary))
        {
            EmitGridInput(new SecondaryReleased(_hoveringGridIndex));
        }
    }

    GridIndex GetHoveringGridIndex(Vector2 globalMousePosition)
    {
        var localMousePosition = globalMousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / HumbleCell.CellSizeInPixels);
        var j = Mathf.FloorToInt(localMousePosition.X / HumbleCell.CellSizeInPixels);
        return new GridIndex(i, j);
    }

    void EmitGridInput(GridInput gridInput) => GridInputEmitted?.Invoke(gridInput);
}