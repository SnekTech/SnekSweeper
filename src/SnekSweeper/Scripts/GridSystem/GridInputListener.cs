using SnekSweeper.CellSystem;
using SnekSweeper.Constants;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

public partial class GridInputListener : Node2D
{
    public event Action<GridInput>? GridInputEmitted;

    /// <summary>落点变了（换格 / 离开网格）；表现层据此移动悬停光标。</summary>
    public event Action<PointerTarget>? TargetChanged;

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
        if (Translate(mouseEvent) is not { } outcome) return;

        // 会话是唯一真相源: 悬停光标与意图都从它派生
        if (outcome.Session.Target != _session.Target)
            TargetChanged?.Invoke(outcome.Session.Target);

        _session = outcome.Session;

        if (outcome.Intent is { } intent) EmitGridInput(intent);
    }

    /// <summary>把 Godot 鼠标事件翻译成会话的一步；不关心的事件返回 null。</summary>
    GridInputOutcome? Translate(InputEventMouse mouseEvent)
    {
        var target = TargetAt(mouseEvent.Position);

        return mouseEvent switch
        {
            InputEventMouseMotion => _session.Move(target),
            InputEventMouseButton button when button.IsActionPressed(InputActions.Primary) =>
                _session.Press(GridButton.Primary, target, button.DoubleClick),
            InputEventMouseButton button when button.IsActionPressed(InputActions.Secondary) =>
                _session.Press(GridButton.Secondary, target, button.DoubleClick),
            InputEventMouseButton button when button.IsActionReleased(InputActions.Primary) =>
                _session.Release(GridButton.Primary, target),
            InputEventMouseButton button when button.IsActionReleased(InputActions.Secondary) =>
                _session.Release(GridButton.Secondary, target),
            _ => null,
        };
    }

    /// <summary>
    /// 屏幕坐标 → 落点；落在网格外是 <see cref="PointerTarget.OffGrid"/> ——
    /// "没有指向任何格子"是一种状态，不是错误值。
    /// </summary>
    PointerTarget TargetAt(Vector2 mousePosition)
    {
        var localMousePosition = mousePosition - GlobalPosition;
        var i = Mathf.FloorToInt(localMousePosition.Y / HumbleCell.CellSizeInPixels);
        var j = Mathf.FloorToInt(localMousePosition.X / HumbleCell.CellSizeInPixels);
        var gridIndex = new GridIndex(i, j);
        return gridIndex.IsWithin(_gridSize) ? new PointerTarget.OnGrid(gridIndex) : new PointerTarget.OffGrid();
    }

    void EmitGridInput(GridInput gridInput) => GridInputEmitted?.Invoke(gridInput);
}
