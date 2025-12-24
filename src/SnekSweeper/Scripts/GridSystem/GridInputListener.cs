using SnekSweeper.Constants;
using SnekSweeperCore.GridSystem.Difficulty;

namespace SnekSweeper.GridSystem;

public partial class GridInputListener : Node2D
{
    public event Action<GridIndex>? PrimaryReleased;
    public event Action<GridIndex>? PrimaryDoubleClicked;
    public event Action<GridIndex>? SecondaryReleased;
    public event Action<GridIndex>? HoveringGridIndexChanged;

    private const int CellSize = CoreStats.CellSizePixels;

    private GridIndex _hoveringGridIndex = new GridIndex(0, 0);

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

    private GridIndex GetHoveringGridIndex(Vector2 globalMousePosition)
    {
        var localMousePosition = globalMousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / CellSize);
        var j = Mathf.FloorToInt(localMousePosition.X / CellSize);
        return new GridIndex(i, j);
    }
}