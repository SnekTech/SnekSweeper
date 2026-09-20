using SnekSweeper.CellSystem;
using SnekSweeper.Constants;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

public partial class GridInputListener : Node2D
{
    public event Action<GridInput>? GridInputEmitted;
    public event Action<GridIndex?>? HoveringGridIndexChanged;

    GridSize _gridSize;
    GridInputSession _session = GridInputSession.Initial;

    /// <summary>
    /// 建图时由 <see cref="HumbleGrid"/> 把尺寸推下来。
    /// 判定"指针是否落在网格内"是翻译输入的前提——不知道网格多大的话，
    /// 这个组件就只能把鼠标位置硬算成一个"看起来合法"的格子。
    /// </summary>
    public void Init(GridSize gridSize) => _gridSize = gridSize;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouse mouseEvent) return;
        if (Translate(mouseEvent) is not { } transition) return;

        // 会话是唯一真相源: 悬停光标与意图都从它派生
        if (transition.Session.Hovered != _session.Hovered)
            HoveringGridIndexChanged?.Invoke(transition.Session.Hovered);

        _session = transition.Session;

        if (transition.Intent is { } intent) EmitGridInput(intent);
    }

    /// <summary>把 Godot 鼠标事件翻译成会话的一步；不关心的事件返回 null。</summary>
    GridInputTransition? Translate(InputEventMouse mouseEvent)
    {
        var index = GetGridIndexAt(mouseEvent.Position);

        return mouseEvent switch
        {
            InputEventMouseMotion => _session.MoveTo(index),
            InputEventMouseButton button when button.IsActionPressed(InputActions.Primary) =>
                _session.Press(GridButton.Primary, index, button.DoubleClick),
            InputEventMouseButton button when button.IsActionPressed(InputActions.Secondary) =>
                _session.Press(GridButton.Secondary, index, button.DoubleClick),
            InputEventMouseButton button when button.IsActionReleased(InputActions.Primary) =>
                _session.Release(GridButton.Primary, index),
            InputEventMouseButton button when button.IsActionReleased(InputActions.Secondary) =>
                _session.Release(GridButton.Secondary, index),
            _ => null,
        };
    }

    /// <summary>屏幕坐标 → 网格坐标；落在网格外返回 null —— "没有指向任何格子"是语义，不是错误值。</summary>
    GridIndex? GetGridIndexAt(Vector2 mousePosition)
    {
        var localMousePosition = mousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / HumbleCell.CellSizeInPixels);
        var j = Mathf.FloorToInt(localMousePosition.X / HumbleCell.CellSizeInPixels);
        var gridIndex = new GridIndex(i, j);
        return gridIndex.IsWithin(_gridSize) ? gridIndex : null;
    }

    void EmitGridInput(GridInput gridInput) => GridInputEmitted?.Invoke(gridInput);
}